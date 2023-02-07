using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Sirensong;
using Sirensong.IoC;
using Wholist.CommandHandling;
using Wholist.Configuration;
using Wholist.Resources.Localization;
using Wholist.UserInterface;
using XivCommon;

namespace Wholist.Common
{
    /// <summary>
    /// Provides access to necessary instances and services.
    /// </summary>
    internal sealed class Services
    {
        // Dalamud services
        [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ClientState ClientState { get; private set; } = null!;
        [PluginService] internal static ObjectTable ObjectTable { get; private set; } = null!;
        [PluginService] internal static Dalamud.Game.Command.CommandManager Commands { get; private set; } = null!;

        // Plugin services
        internal static WindowManager WindowManager { get; private set; } = null!;
        internal static XivCommonBase XivCommon { get; private set; } = null!;
        internal static PluginConfiguration PluginConfiguration { get; private set; } = null!;

        // Additional services
        private static readonly MiniServiceContainer ServiceContainer = new();

        /// <summary>
        /// Initializes the service class.
        /// </summary>
        internal static void Initialize(DalamudPluginInterface pluginInterface)
        {
            BetterLog.Debug("Initializing services.");

            SirenCore.InjectServices<Services>();
            pluginInterface.Create<Services>();

            PluginConfiguration = PluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
            XivCommon = new XivCommonBase();
            ServiceContainer.GetOrCreateService<LocalizationManager>();
            WindowManager = ServiceContainer.GetOrCreateService<WindowManager>();
            ServiceContainer.CreateService<CommandManager>();
        }

        /// <summary>
        /// Disposes of the service class.
        /// </summary>
        internal static void Dispose()
        {
            ServiceContainer.Dispose();
            XivCommon.Dispose();
        }
    }
}