using Bep = BepInEx;
using IC = DDoor.ItemChanger;
using MUI = MagicUI;
using static MagicUI.Core.ApplyAttachedPropertyChainables;
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

        var si = System.Math.Max(0, tlog.Count - numDisplayedItems);
        var numUsedSlots = tlog.Count - si;

        for (var i = 0; i < numUsedSlots; i++)
        {
            var slot = (MUI.Elements.TextObject)layout.Children[1 + i];
            slot.Text = tlog[si + i].ItemName;
            slot.Visibility = MUI.Core.Visibility.Visible;
        }

        for (var i = numUsedSlots; i < numDisplayedItems; i++)
        {
            var slot = (MUI.Elements.TextObject)layout.Children[1 + i];
            slot.Text = "";
            slot.Visibility = MUI.Core.Visibility.Collapsed;
        }
    }

    private MUI.Core.Layout InitLayout()
    {
        if (_layout != null)
        {
            return _layout;
        }
        // We could almost use a StackLayout, but we want to set
        // a minimum width and StackLayout doesn't directly support
        // that.
        var root = new MUI.Core.LayoutRoot(true, "Recent Items Display");
        _layout = new(root, "Items List");
        for (var i = 0; i < numDisplayedItems + 1; i++)
        {
            _layout.RowDefinitions.Add(new(0, MUI.Elements.GridUnit.AbsoluteMin));
        }
        _layout.Children.Add(MakeRow(_layout, "Title", titleFontSize, "RECENT ITEMS"));
        for (var i = 0; i < numDisplayedItems; i++)
        {
            _layout.Children.Add(
                MakeRow(_layout, "Recent Item", itemFontSize, "")
                .WithProp(MUI.Elements.GridLayout.Row, i + 1));
        }
        _layout.MinWidth = displayWidth;
        _layout.HorizontalAlignment = MUI.Core.HorizontalAlignment.Right;
        _layout.VerticalAlignment = MUI.Core.VerticalAlignment.Top;
        return _layout;
    }

    private static MUI.Elements.TextObject MakeRow(MUI.Core.Layout layout, string name, int fontSize, string content)
    {
        var element = new MUI.Elements.TextObject(layout.LayoutRoot, name);
        element.Text = content;
        element.FontSize = fontSize;
        element.MaxWidth = displayWidth;
        return element;
    }

    private const float displayWidth = 300;
    private const int titleFontSize = 30;
    private const int itemFontSize = 24;
    private const int numDisplayedItems = 7;

    private MUI.Elements.GridLayout? _layout;
}
