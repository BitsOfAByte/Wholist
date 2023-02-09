using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Sirensong.Game.Utility;
using Sirensong.UserInterface;
using Wholist.Common;
using Wholist.DataStructures;
using Wholist.Resources.Localization;

namespace Wholist.UserInterface.Windows.NearbyPlayers
{
    internal sealed class NearbyPlayersWindow : Window
    {
        private readonly NearbyPlayersLogic logic = new();

        /// <summary>
        /// Creates a new instance of the <see cref="NearbyPlayersWindow" />.
        /// </summary>
        internal NearbyPlayersWindow() : base(Strings.Windows_Who_Title.Format(Constants.PluginName))
        {
            this.Size = new Vector2(450, 400);
            this.SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override bool DrawConditions()
        {
            if (NearbyPlayersLogic.Configuration.NearbyPlayers.HideInCombat && Services.Condition[ConditionFlag.InCombat])
            {
                return false;
            }

            if (NearbyPlayersLogic.Configuration.NearbyPlayers.HideInInstance && (ConditionUtil.IsBoundByDuty() || ConditionUtil.IsInIslandSanctuary()))
            {
                return false;
            }

            if (Services.ClientState.IsPvP)
            {
                return false;
            }

            return true;
        }

        private readonly Vector2 childSize = new(0, -60);

        /// <summary>
        /// Draws the window.
        /// </summary>
        public override void Draw()
        {
            this.Flags = NearbyPlayersLogic.ApplyFlagConfiguration(this.Flags);
            this.RespectCloseHotkey = !NearbyPlayersLogic.DisableEscClose;

            var playersToDraw = this.logic.GetNearbyPlayers();
            if (ImGui.BeginChild("##NearbyChild", this.childSize, true))
            {
                DrawNearbyPlayersTable(playersToDraw);
            }
            ImGui.EndChild();

            // Draw the search box.
            DrawSearchBar(ref this.logic.SearchText);

            // Draw the total players text.
            DrawTotalPlayers(playersToDraw.Count);
        }

        /// <summary>
        /// Draws the nearby players table.
        /// </summary>
        /// <param name="playersToDraw"></param>
        private static void DrawNearbyPlayersTable(List<PlayerInfoSlim> playersToDraw)
        {
            if (ImGui.BeginTable("##NearbyTable", 4, ImGuiTableFlags.ScrollY | ImGuiTableFlags.BordersInner | ImGuiTableFlags.Hideable | ImGuiTableFlags.Reorderable))
            {
                ImGui.TableSetupColumn(Strings.UserInterface_NearbyPlayers_Players_Name, ImGuiTableColumnFlags.WidthStretch, 250);
                ImGui.TableSetupColumn(Strings.UserInterface_NearbyPlayers_Players_Company, ImGuiTableColumnFlags.WidthStretch, 100);
                ImGui.TableSetupColumn(Strings.UserInterface_NearbyPlayers_Players_Level, ImGuiTableColumnFlags.WidthStretch, 50);
                ImGui.TableSetupColumn(Strings.UserInterface_NearbyPlayers_Players_Class, ImGuiTableColumnFlags.WidthStretch, 150);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                // Draw players.
                foreach (var obj in playersToDraw)
                {
                    DrawPlayer(obj);
                }
                ImGui.EndTable();
            }
        }

        /// <summary>
        /// Draws the total players text.
        /// </summary>
        /// <param name="totalPlayers"></param>
        private static void DrawTotalPlayers(int totalPlayers) => ImGuiHelpers.CenteredText(Strings.UserInterface_NearbyPlayers_Players_Total.Format(totalPlayers));

        /// <summary>
        /// Draws the search bar.
        /// </summary>
        /// <param name="searchText"></param>
        private static void DrawSearchBar(ref string searchText)
        {
            ImGui.SetNextItemWidth(-1);
            ImGui.InputTextWithHint("##NearbySearch", Strings.UserInterface_NearbyPlayers_Search, ref searchText, 100);
        }

        /// <summary>
        /// Draws a player in the table.
        /// </summary>
        /// <param name="obj"></param>
        private static void DrawPlayer(PlayerInfoSlim obj)
        {
            // Name.
            ImGui.TableNextColumn();
            SiGui.Text(obj.Name);

            // Context menu.
            DrawContextMenu(obj);
            ImGui.TableNextColumn();

            // Company.
            SiGui.Text(obj.CompanyTag);
            ImGui.TableNextColumn();

            // Level.
            SiGui.Text(obj.Level.ToString());
            ImGui.TableNextColumn();

            // Class.
            SiGui.TextColoured(obj.Class.RoleColour, obj.Class.Name);
        }

        /// <summary>
        /// Draws the context menu for a player.
        /// </summary>
        /// <param name="obj"></param>
        private static void DrawContextMenu(PlayerInfoSlim obj)
        {
            if (ImGui.BeginPopupContextItem($"{obj.Name}##WholistPopContext"))
            {
                // Heading.
                SiGui.Heading(Strings.UserInterface_NearbyPlayers_Players_Submenu_Heading.Format($"{obj.Name}@{obj.Homeworld.Name}"));

                // Options.
                if (ImGui.Selectable(Strings.UserInterface_NearbyPlayers_Players_Submenu_Examine))
                {
                    obj.OpenExamine();
                }
                if (ImGui.Selectable(Strings.UserInterface_NearbyPlayers_Players_AdventurePlate))
                {
                    obj.OpenCharaCard();
                }
                if (ImGui.Selectable(Strings.UserInterface_NearbyPlayers_Players_Submenu_Target))
                {
                    obj.Target();
                }
                if (ImGui.Selectable(Strings.UserInterface_NearbyPlayers_Players_Submenu_Tell))
                {
                    NearbyPlayersLogic.SetChatTellTarget(obj.Name, obj.Homeworld.Name);
                }

                ImGui.EndPopup();
            }
        }
    }
}