using Bep = BepInEx;
using IC = DDoor.ItemChanger;
using MUI = MagicUI;
using GL = MagicUI.Elements.GridLayout;
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
            var icon = IC.ItemIcons.Get(tlog[si + i].ItemIcon);
            if (_rows[i].Icon is {} img)
            {
                img.Sprite = icon;
            }
            else
            {
                img = new MUI.Elements.Image(layout.LayoutRoot, icon, "Recent Item Icon")
                    .WithProp(GL.Row, i + 1)
                    .WithProp(GL.Column, 0);
                img.Width = itemFontSize * 2;
                img.Height = itemFontSize * 2;
                img.PreserveAspectRatio = true;
                _rows[i].Icon = img;
                layout.Children.Add(img);
            }
            var slot = (MUI.Elements.TextObject)layout.Children[1 + i];
            slot.Text = tlog[si + i].ItemName;
            slot.Visibility = MUI.Core.Visibility.Visible;
        }

        for (var i = numUsedSlots; i < numDisplayedItems; i++)
        {
            _rows[i].Name!.Visibility = MUI.Core.Visibility.Collapsed;
            if (_rows[i].Icon is {} img)
            {
                img.Visibility = MUI.Core.Visibility.Collapsed;
            }
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
        _layout.ColumnDefinitions.Add(new(itemFontSize, MUI.Elements.GridUnit.AbsoluteMin));
        _layout.ColumnDefinitions.Add(new(displayWidth - itemFontSize, MUI.Elements.GridUnit.AbsoluteMin));
        for (var i = 0; i < numDisplayedItems + 1; i++)
        {
            _layout.RowDefinitions.Add(new(0, MUI.Elements.GridUnit.AbsoluteMin));
        }
        _layout.Children.Add(
            MakeRow(_layout, "Title", titleFontSize, "RECENT ITEMS")
            .WithProp(GL.Row, 0)
            .WithProp(GL.Column, 0)
            .WithProp(GL.ColumnSpan, 2));
        _rows = new Row[numDisplayedItems];
        for (var i = 0; i < numDisplayedItems; i++)
        {
            var name = MakeRow(_layout, "Recent Item", itemFontSize, "")
                .WithProp(GL.Row, i + 1)
                .WithProp(GL.Column, 1);
            _rows[i].Name = name;
            _layout.Children.Add(name);
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

    private Row[] _rows = new Row[0];

    private struct Row
    {
        internal MUI.Elements.Image? Icon;
        internal MUI.Elements.TextObject? Name;
    }

    private const float displayWidth = 300;
    private const int titleFontSize = 36;
    private const int itemFontSize = 28;
    private const int numDisplayedItems = 7;

    private MUI.Elements.GridLayout? _layout;
}
