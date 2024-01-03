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
        if (tlog == null)
        {
            if (_layout != null)
            {
                _layout.Visibility = MUI.Core.Visibility.Collapsed;
            }
            return;
        }

        var layout = InitLayout();
        layout.Visibility = MUI.Core.Visibility.Visible;

        // Reuse TextObjects whenever possible so that the list doesn't
        // flicker when we update it.
        var si = System.Math.Max(0, tlog.Count - numDisplayedItems);
        var numUsedSlots = tlog.Count - si;
        // excluding the title
        var numAvailableSlots = layout.Children.Count - 1;

        while (numAvailableSlots < numUsedSlots)
        {
            AddRow(layout, "Recent Item", itemFontSize, "");
            numAvailableSlots++;
        }

        for (var i = 0; i < numUsedSlots; i++)
        {
            var slot = (MUI.Elements.TextObject)layout.Children[1 + i];
            slot.Text = tlog[si + i].ItemName;
            slot.Visibility = MUI.Core.Visibility.Visible;
        }

        for (var i = numUsedSlots; i < numAvailableSlots; i++)
        {
            var slot = (MUI.Elements.TextObject)layout.Children[1 + i];
            slot.Text = "";
            slot.Visibility = MUI.Core.Visibility.Collapsed;
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
            AddRow(_layout, "Title", titleFontSize, "RECENT ITEMS");
        }
        return _layout;
    }

    private static void AddRow(MUI.Core.Layout layout, string name, int fontSize, string content)
    {
        var element = new MUI.Elements.TextObject(layout.LayoutRoot, name);
        element.Text = content;
        element.FontSize = fontSize;
        element.MaxWidth = displayWidth;
        layout.Children.Add(element);
    }

    private const float displayWidth = 300;
    private const int titleFontSize = 30;
    private const int itemFontSize = 24;
    private const int numDisplayedItems = 7;

    private MUI.Elements.StackLayout? _layout;
}
