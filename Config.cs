using BepInEx.Configuration;

namespace FancyWeatherAPI
{
    class Config
    {
        public readonly ConfigEntry<bool> DebugLogsEnabled;

        public Config(ConfigFile cfg)
        {
            cfg.SaveOnConfigSet = false;

            DebugLogsEnabled = cfg.Bind("Debug", "Debug logs", false, "Enable more explicit logs in the console (for debug reasons).");

            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }

        public void SetupCustomConfigs() { }
    }
}
