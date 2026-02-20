using FancyWeatherAPI.API;
using GeneralImprovements.Utilities;
using HarmonyLib;
using UnityEngine;
using WeatherRegistry;

namespace FancyWeatherAPI.Patches
{
    [HarmonyPatch(typeof(MonitorsHelper))]
    internal class MonitorPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdateWeatherMonitors")]
        public static void InjectNewWeatherAnimations()
        {
            if (Settings.SetupFinished && MonitorsHelper._fancyWeatherMonitorTexts.Count > 0 && StartOfRound.Instance.currentLevel != null)
            {
                string weatherName = WeatherManager.GetCurrentLevelWeather().Name;

                if (AnimationLoader.LoadedAnimations.TryGetValue(weatherName, out FancyWeatherAnimation weatherAnimation))
                {
                    Plugin.DebugLog("[MonitorPatch] Injecting weather animation for " + weatherName);
                    MonitorsHelper._curWeatherAnimations = weatherAnimation.GetFullFrames();
                    MonitorsHelper._weatherHasOverlays = weatherAnimation.WithOverlay;

                    if (weatherAnimation.WithOverlay)
                    {
                        MonitorsHelper._weatherOverlayTimer = 0;
                        MonitorsHelper._weatherOverlayCycle = Random.Range(0.1f, 3);
                        MonitorsHelper._curWeatherOverlays = WeatherASCIIArt.LightningOverlays;
                        MonitorsHelper._curWeatherOverlayIndex = Random.Range(0, MonitorsHelper._curWeatherOverlays.Length);
                    }

                    MonitorsHelper._weatherShowingOverlay = false;
                    MonitorsHelper._curWeatherAnimIndex = 0;
                    MonitorsHelper._weatherAnimTimer = 0;
                }
            }
        }
    }
}
