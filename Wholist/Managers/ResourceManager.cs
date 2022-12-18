using System;
using System.IO;
using System.Reflection;
using CheapLoc;
using Dalamud.Logging;
using Wholist.Base;

namespace Wholist.Managers
{
    /// <summary>
    ///     Sets up and manages the plugin's resources and localization.
    /// </summary>
    internal sealed class ResourceManager : IDisposable
    {
        /// <summary>
        ///     Initializes the ResourceManager and associated resources.
        /// </summary>
        internal ResourceManager()
        {
            PluginLog.Debug("ResourceManager(Constructor): Initializing...");

            this.Setup(PluginService.PluginInterface.UiLanguage);
            PluginService.PluginInterface.LanguageChanged += this.Setup;

            PluginLog.Debug("ResourceManager(Constructor): Initialization complete.");
        }

        /// <summary>
        //      Disposes of the ResourceManager and associated resources.
        /// </summary>
        public void Dispose()
        {
            PluginService.PluginInterface.LanguageChanged -= this.Setup;

            PluginLog.Debug("ResourceManager(Dispose): Successfully disposed.");
        }

        /// <summary>
        ///     Sets up the plugin's resources.
        /// </summary>
        /// <param name="language">The new language 2-letter code.</param>
        private void Setup(string language)
        {
            try
            {
                using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Wholist.Resources.Localization.Plugin.{language}.json");

                if (resource == null)
                {
                    throw new FileNotFoundException($"ResourceManager(Setup): Could not find resource file for language {language}.");
                }

                using var reader = new StreamReader(resource);
                Loc.Setup(reader.ReadToEnd());
                PluginLog.Information($"ResourceManager(Setup): Resource file for language {language} loaded successfully.");
            }
            catch
            {
                PluginLog.Information("ResourceManager(Setup): Something went wrong while setting up localization (possibily missing a resource file or resource file is corrupt), falling back to English.");
                Loc.SetupWithFallbacks();
            }
        }
    }
}
