using System.Linq;
using Content.Shared.Ghost.Roles;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
    [GenerateTypedNameReferences]
    public sealed partial class GhostRolesWindow : DefaultWindow
    {
        public event Action<GhostRoleInfo>? OnRoleRequestButtonClicked;
        public event Action<GhostRoleInfo>? OnRoleRulesButtonClicked;
        public event Action<GhostRoleInfo>? OnRoleFollow;

        private Dictionary<string, Collapsible> _collapsibleBoxes = new();
        private HashSet<string> _uncollapsedStates = new();

        public GhostRolesWindow()
        {
            RobustXamlLoader.Load(this);
        }

        public void ClearEntries()
        {
            NoRolesMessage.Visible = true;
            EntryContainer.DisposeAllChildren();
            _collapsibleBoxes.Clear();
        }

        public void SaveCollapsibleBoxesStates()
        {
            _uncollapsedStates.Clear();
            foreach (var (key, collapsible) in _collapsibleBoxes)
            {
                if (collapsible.BodyVisible)
                {
                    _uncollapsedStates.Add(key);
                }
            }
        }

        public void RestoreCollapsibleBoxesStates()
        {
            foreach (var (key, collapsible) in _collapsibleBoxes)
            {
                collapsible.BodyVisible = _uncollapsedStates.Contains(key);
            }
        }

        public void AddEntry(string name, string description, string prototypeId, bool hasAccess, FormattedMessage? reason, IEnumerable<GhostRoleInfo> roles, SpriteSystem spriteSystem)
        {
            NoRolesMessage.Visible = false;

            var ghostRoleInfos = roles.ToList();
            var rolesCount = ghostRoleInfos.Count;

            var info = new GhostRoleInfoBox(name, description);
            var buttons = new GhostRoleButtonsBox(hasAccess, reason, ghostRoleInfos, spriteSystem);
            buttons.OnRoleSelected += OnRoleRequestButtonClicked;
            buttons.OnRoleRules += OnRoleRulesButtonClicked;
            buttons.OnRoleFollow += OnRoleFollow;

            EntryContainer.AddChild(info);

            if (rolesCount > 1)
            {
                var buttonHeading = new CollapsibleHeading(Loc.GetString("ghost-roles-window-available-button", ("rolesCount", rolesCount)));

                buttonHeading.AddStyleClass(ContainerButton.StyleClassButton);
                buttonHeading.Label.HorizontalAlignment = HAlignment.Center;
                buttonHeading.Label.HorizontalExpand = true;
                buttonHeading.Margin = new Thickness(8, 0, 8, 2);

                var body = new CollapsibleBody
                {
                    Margin = new Thickness(0, 5, 0, 0),
                };

                var collapsible = new Collapsible(buttonHeading, body)
                {
                    Orientation = BoxContainer.LayoutOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 8),
                };

                body.AddChild(buttons);

                EntryContainer.AddChild(collapsible);
                _collapsibleBoxes.Add(prototypeId, collapsible);
            }
            else
            {
                EntryContainer.AddChild(buttons);
            }
        }
    }
}
