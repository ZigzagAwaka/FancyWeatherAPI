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
        /// <summary>
        /// Patch the UpdateWeatherMonitors method to inject our custom weather animations. This will replace the existing weather monitor text with our own animations if available.
        /// </summary>
        /// <returns>Return false to skip the original code</returns>
        [HarmonyPrefix]
        [HarmonyPatch("UpdateWeatherMonitors")]
        public static bool InjectNewWeatherAnimations()
        {
            if (MonitorsHelper._weatherMonitorTexts.Count > 0)
            {
                if (MonitorsHelper.UpdateGenericTextList(MonitorsHelper._weatherMonitorTexts, $"WEATHER:\n{(StartOfRound.Instance.currentLevel ? StartOfRound.Instance.currentLevel.currentWeather.ToString() : "???")}"))
                {
                    GeneralImprovements.Plugin.MLS.LogInfo("Updated basic weather monitors");
                }
            }

            if (Settings.SetupFinished && MonitorsHelper._fancyWeatherMonitorTexts.Count > 0 && StartOfRound.Instance.currentLevel != null)
            {
                string weatherName = WeatherManager.GetCurrentLevelWeather().Name;

                bool animationFound = AnimationLoader.LoadedAnimations.TryGetValue(weatherName, out FancyWeatherAnimation weatherAnimation);

                if (animationFound)
                {
                    Plugin.DebugLog("[MonitorPatch] Injecting weather animation for " + weatherName);
                }

                MonitorsHelper._curWeatherAnimations = animationFound ? weatherAnimation.GetFullFrames() : WeatherASCIIArt.UnknownAnimations;
                MonitorsHelper._weatherHasOverlays = animationFound && weatherAnimation.WithLightningOverlay;

                if (MonitorsHelper._weatherHasOverlays)
                {
                    MonitorsHelper._weatherOverlayTimer = 0;
                    MonitorsHelper._weatherOverlayCycle = Random.Range(0.1f, 3);
                    MonitorsHelper._curWeatherOverlays = WeatherASCIIArt.LightningOverlays;
                    MonitorsHelper._curWeatherOverlayIndex = Random.Range(0, MonitorsHelper._curWeatherOverlays.Length);
                }

                MonitorsHelper._weatherShowingOverlay = false;
                MonitorsHelper._curWeatherAnimIndex = 0;
                MonitorsHelper._weatherAnimTimer = 0;

                if (MonitorsHelper.UpdateGenericTextList(MonitorsHelper._fancyWeatherMonitorTexts, MonitorsHelper._curWeatherAnimations[MonitorsHelper._curWeatherAnimIndex]))
                {
                    GeneralImprovements.Plugin.MLS.LogInfo("Updated fancy weather monitors");
                }

            }

            return false;  // Skip original method since we handled the animation injection
        }
    }
}
