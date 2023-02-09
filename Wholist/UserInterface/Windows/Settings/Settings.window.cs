using Dalamud.Interface.Windowing;
using ImGuiNET;
using Wholist.Resources.Localization;
using Wholist.UserInterface.Windows.Settings.TableParts;

namespace Wholist.UserInterface.Windows.Settings
{
    internal sealed class SettingsWindow : Window
    {
        /// <inheritdoc/>
        public SettingsLogic Logic { get; } = new();

        /// <inheritdoc/>
        public SettingsWindow() : base(Strings.Windows_Settings_Title)
        {
            this.Size = new(600, 400);
            this.SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(600, 400),
                MaximumSize = new(1200, 700),
            };
            this.SizeCondition = ImGuiCond.FirstUseEver;
            this.Flags = ImGuiWindowFlags.NoScrollbar;
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            if (ImGui.BeginTable("PluginSettings", 2, ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableSetupColumn("PluginSettingsSidebar", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X * 0.3f);
                ImGui.TableSetupColumn("PluginSettingsList", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X * 0.7f);
                ImGui.TableNextRow();

                // Sidebar
                ImGui.TableNextColumn();
                if (ImGui.BeginChild("PluginSettingsSidebarChild"))
                {
                    SettingsSidebar.Draw(this.Logic);
                }
                ImGui.EndChild();

                // Listings
                ImGui.TableNextColumn();
                if (ImGui.BeginChild("PluginSettingsListChild"))
                {
                    SettingsActive.Draw(this.Logic);
                }
                ImGui.EndChild();

                ImGui.EndTable();
            }
        }
    }
}