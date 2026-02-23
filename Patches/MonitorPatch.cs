using FancyWeatherAPI.API;
using GeneralImprovements.Utilities;
using HarmonyLib;
using System.Linq;
using System.Text;
using UnityEngine;
using WeatherRegistry;

namespace FancyWeatherAPI.Patches
{
    /// <summary>
    /// Patches of the MonitorsHelper class: https://github.com/Shaosil/LethalCompanyMods-GeneralImprovements/blob/5959f0ecff6553630a368aa21b2d83034c0b2750/Utilities/MonitorsHelper.cs#L801
    /// </summary>
    [HarmonyPatch(typeof(MonitorsHelper))]
    internal class MonitorPatch
    {
        internal static string? _curWeatherColorHex;

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

                _curWeatherColorHex = animationFound ? weatherAnimation.ColorHex : null;
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

                if (MonitorsHelper.UpdateGenericTextList(MonitorsHelper._fancyWeatherMonitorTexts,
                    _curWeatherColorHex != null ? $"<color={_curWeatherColorHex}>{MonitorsHelper._curWeatherAnimations[MonitorsHelper._curWeatherAnimIndex]}</color>" : MonitorsHelper._curWeatherAnimations[MonitorsHelper._curWeatherAnimIndex]))
                {
                    GeneralImprovements.Plugin.MLS.LogInfo("Updated fancy weather monitors");
                }

            }

            return false;  // Skip original method since we handled the animation injection
        }



        [HarmonyPrefix]
        [HarmonyPatch("AnimateSpecialMonitors")]
        public static bool AnimateSpecialMonitorsFromAPI()
        {
            if (MonitorsHelper._fancyWeatherMonitorTexts.Count > 0 && MonitorsHelper._curWeatherAnimations.Length >= 2)
            {
                MonitorsHelper._weatherAnimTimer += Time.deltaTime;
                if (MonitorsHelper._weatherAnimTimer >= MonitorsHelper._weatherAnimCycle)
                {
                    MonitorsHelper._curWeatherAnimIndex = (MonitorsHelper._curWeatherAnimIndex + 1) % MonitorsHelper._curWeatherAnimations.Length;
                    MonitorsHelper._weatherAnimTimer = 0;
                    DrawCustomWeatherAnimation();
                }

                if (MonitorsHelper._weatherHasOverlays)
                {
                    MonitorsHelper._weatherOverlayTimer += Time.deltaTime;
                    if (MonitorsHelper._weatherOverlayTimer >= (MonitorsHelper._weatherShowingOverlay ? 0.5f : MonitorsHelper._weatherOverlayCycle))
                    {
                        MonitorsHelper._weatherOverlayTimer = 0;
                        MonitorsHelper._weatherShowingOverlay = !MonitorsHelper._weatherShowingOverlay;

                        if (!MonitorsHelper._weatherShowingOverlay)
                        {
                            MonitorsHelper._weatherOverlayCycle = Random.Range(0.1f, 3);
                            MonitorsHelper._curWeatherOverlayIndex = Random.Range(0, MonitorsHelper._curWeatherOverlays.Length);
                        }

                        DrawCustomWeatherAnimation();
                    }
                }
            }

            /// DON'T TOUCH THE CODE BELOW
            /// It's not related to our custom animations, it's here to not break the existing behavior of the original method

            if (MonitorsHelper._salesMonitorTexts.Count > 0 && MonitorsHelper._curSalesAnimations.Count >= 2)
            {
                MonitorsHelper._salesAnimTimer += Time.deltaTime;
                if (MonitorsHelper._salesAnimTimer >= MonitorsHelper._salesAnimCycle)
                {
                    MonitorsHelper._salesAnimTimer = 0;
                    MonitorsHelper._curSalesAnimIndex = (1 + MonitorsHelper._curSalesAnimIndex) % MonitorsHelper._curSalesAnimations.Count;
                    string firstLine = MonitorsHelper._salesMonitorTexts.First().text.Split('\n')[0];
                    MonitorsHelper.UpdateGenericTextList(MonitorsHelper._salesMonitorTexts, $"{firstLine}\n{MonitorsHelper._curSalesAnimations[MonitorsHelper._curSalesAnimIndex]}");
                }
            }

            if ((MonitorsHelper._playerHealthMonitorTexts.Count > 0 || MonitorsHelper._playerExactHealthMonitorTexts.Count > 0) && MonitorsHelper._curPlayerHealthAnimations.Count >= 2)
            {
                MonitorsHelper._playerHealthAnimTimer += Time.deltaTime;
                if (MonitorsHelper._playerHealthAnimTimer >= MonitorsHelper._playerHealthAnimCycle)
                {
                    MonitorsHelper._playerHealthAnimTimer = 0;
                    MonitorsHelper._curPlayerHealthAnimIndex = (1 + MonitorsHelper._curPlayerHealthAnimIndex) % MonitorsHelper._curPlayerHealthAnimations.Count;
                    if (MonitorsHelper._playerHealthMonitorTexts.Count > 0)
                        MonitorsHelper.UpdateGenericTextList(MonitorsHelper._playerHealthMonitorTexts, MonitorsHelper._curPlayerHealthAnimations[MonitorsHelper._curPlayerHealthAnimIndex]);
                    if (MonitorsHelper._playerExactHealthMonitorTexts.Count > 0)
                        MonitorsHelper.UpdateGenericTextList(MonitorsHelper._playerExactHealthMonitorTexts, MonitorsHelper._curPlayerExactHealthAnimations[MonitorsHelper._curPlayerHealthAnimIndex]);
                }
            }

            return false;  // Skip original method since we handled the new animations
        }



        /// <summary>
        /// Draw the current weather animation frame on the monitor, matching any displayed characters with their corresponding overlay if it exists
        /// </summary>
        private static void DrawCustomWeatherAnimation()
        {
            var sb = new StringBuilder();
            string[] animLines = MonitorsHelper._curWeatherAnimations[MonitorsHelper._curWeatherAnimIndex].Split(System.Environment.NewLine);
            string[] overlayLines = (MonitorsHelper._weatherShowingOverlay ? MonitorsHelper._curWeatherOverlays[MonitorsHelper._curWeatherOverlayIndex] : string.Empty).Split(System.Environment.NewLine);

            for (int l = 0; l < animLines.Length; l++)
            {
                string curAnimLine = animLines[l];
                string overlayLine = overlayLines.ElementAtOrDefault(l);

                for (int c = 0; c < curAnimLine.Length; c++)
                {
                    bool isOverlayChar = !string.IsNullOrWhiteSpace(overlayLine) && overlayLine.Length > c && overlayLine[c] != ' ';
                    sb.Append(isOverlayChar ? $"<color=#ffe100>{overlayLine[c]}</color>" : (_curWeatherColorHex != null ? $"<color={_curWeatherColorHex}>{curAnimLine[c]}</color>" : $"{curAnimLine[c]}"));
                }
                sb.AppendLine();
            }

            MonitorsHelper.UpdateGenericTextList(MonitorsHelper._fancyWeatherMonitorTexts, sb.ToString());
        }
    }
}
