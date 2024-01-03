using Bep = BepInEx;
using IC = DDoor.ItemChanger;
using MUI = MagicUI;
using Collections = System.Collections.Generic;

[Bep.BepInPlugin("deathsdoor.recentitemsdisplay", "RecentItemsDisplay", "1.0.0.0")]
[Bep.BepInDependency("deathsdoor.magicui", "1.8")]
[Bep.BepInDependency("deathsdoor.itemchanger", "1.0.1")]
internal class RecentItemsDisplayPlugin : Bep.BaseUnityPlugin
{
    public void Start()
    {
        IC.SaveData.OnTrackerLogUpdate += UpdateHUD;
    }

    private void UpdateHUD(Collections.List<IC.TrackerLogEntry>? tlog)
    {
        var layout = InitLayout();
        layout.Children.Clear();

        if (tlog == null)
        {
            return;
        }

        AddRow(layout, "Title", "RECENT ITEMS");

        for (var i = System.Math.Max(0, tlog.Count - numDisplayedItems);
             i < tlog.Count; i++)
        {
            AddRow(layout, "Recent Item", tlog[i].ItemName);
        }
    }

    private MUI.Core.Layout InitLayout()
    {
        if (_layout == null)
        {
            var root = new MUI.Core.LayoutRoot(true, "Recent Items Display");
            _layout = new(root, "Items List");
            _layout.Orientation = MUI.Core.Orientation.Vertical;
            _layout.HorizontalAlignment = MUI.Core.HorizontalAlignment.Right;
            _layout.VerticalAlignment = MUI.Core.VerticalAlignment.Top;
        }
        return _layout;
    }

    private static void AddRow(MUI.Core.Layout layout, string name, string content)
    {
        var element = new MUI.Elements.TextObject(layout.LayoutRoot, name);
        element.Text = content;
        element.MaxWidth = displayWidth;
        layout.Children.Add(element);
    }

    private const float displayWidth = 300;
    private const int numDisplayedItems = 7;

    private MUI.Elements.StackLayout? _layout;
}
