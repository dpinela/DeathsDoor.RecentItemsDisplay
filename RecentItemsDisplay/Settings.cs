using BepConfig = BepInEx.Configuration;

namespace DDoor.RecentItemsDisplay;

internal class Settings
{
    private BepConfig.ConfigEntry<int> maxNumEntries;
    private BepConfig.ConfigEntry<bool> showAreaName;

    public Settings(BepConfig.ConfigFile f)
    {
        maxNumEntries = f.Bind("", "MaxNumEntries", 7,
            new BepConfig.ConfigDescription(
                "The maximum number of items shown", 
                new BepConfig.AcceptableValueRange<int>(0, 100),
                new ConfigurationManagerAttributes()));
        maxNumEntries.SettingChanged += (_, _) => OnSettingsChanged();

        showAreaName = f.Bind("", "ShowAreaName", true);
        showAreaName.SettingChanged += (_, _) => OnSettingsChanged();
    }

    public int MaxNumEntries => maxNumEntries.Value;
    public bool ShowAreaName => showAreaName.Value;

    public event System.Action OnSettingsChanged = () => {};

    // Used to keep numeric settings from displaying as % slider
    // in the BepInEx configuration manager.
    // Has no effect if that mod is not present.
    private class ConfigurationManagerAttributes
    {
        public bool? ShowRangeAsPercent = false;
    }
}