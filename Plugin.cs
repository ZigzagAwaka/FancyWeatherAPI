using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace FancyWeatherAPI
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("ShaosilGaming.GeneralImprovements")]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "zigzag.fancyweatherapi";
        const string NAME = "Fancy Weather API";
        const string VERSION = "1.0.0";

        public static Plugin instance;
        public static ManualLogSource logger;
        private readonly Harmony harmony = new Harmony(GUID);
        internal static Config config { get; private set; } = null!;

        internal static List<string> installedWeathers = new List<string>();

        internal static void DebugLog(object message)
        {
            if (config.DebugLogsEnabled.Value)
                logger.LogWarning(message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        void Awake()
        {
            instance = this;
            logger = Logger;
            config = new Config(Config);

            config.SetupCustomConfigs();
            harmony.PatchAll();

            logger.LogInfo($"{NAME} is loaded !");
        }
    }
}
