using GeneralImprovements.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FancyWeatherAPI.API
{
    internal class AnimationLoader
    {
        /// <summary>
        /// Dictionary that contains all the loaded animations, the key is the name of the weather, and the value is the corresponding FancyWeatherAnimation object
        /// </summary>
        public static Dictionary<string, FancyWeatherAnimation> LoadedAnimations = new Dictionary<string, FancyWeatherAnimation>();

        /// <summary>
        /// Dictionary that contains all the loaded overlays, the key is the name of the overlay and the value is the corresponding FancyWeatherAnimation object
        /// </summary>
        public static Dictionary<string, FancyWeatherAnimation> LoadedOverlays = new Dictionary<string, FancyWeatherAnimation>();


        /// <summary>
        /// Load all the animation files located in the ASCII_Anim folder, and add them to the LoadedAnimations dictionary.
        /// </summary>
        internal static void LoadAllAnimationFiles()
        {
            string[]? jsonFiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ASCII_Anim"),
                $"*.txt", SearchOption.AllDirectories);

            if (jsonFiles == null)
            {
                Plugin.logger.LogError("[AnimationLoader] ASCII_Anim folder not found");
                return;
            }

            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileName(filePath);

                Plugin.DebugLog($"[AnimationLoader] Found animation file, now loading: {fileName}");

                int nbAnim = LoadAnimationFromFile(filePath);

                Plugin.DebugLog($"[AnimationLoader] The file {fileName} has loaded {nbAnim} custom animations");
            }
        }


        private static int LoadAnimationFromFile(string filePath)
        {
            try
            {
                string text = File.ReadAllText(filePath);
                if (text == null || text.Length <= 0)
                {
                    Plugin.logger.LogError("[AnimationLoader] The file is empty or null");
                    return 0;
                }

                int nbAnim = 0;
                bool isOverlay = false;
                bool scanningParameters = false;
                bool scanningFrames = false;
                FancyWeatherAnimation animation = new FancyWeatherAnimation();

                foreach (string line in text.Split('\n'))
                {
                    string trimmedLine = line.ToLower().Trim();
                    if (trimmedLine == "parameters")
                    {
                        if (animation != null && animation.IsValid() && animation.Name != null)
                        {
                            if (!isOverlay)
                            {
                                if (LoadedAnimations.TryAdd(animation.Name, animation))
                                    nbAnim++;
                            }
                            else
                            {
                                if (LoadedOverlays.TryAdd(animation.Name, animation))
                                    nbAnim++;
                            }
                        }
                        animation = new FancyWeatherAnimation();
                        scanningParameters = true;
                        scanningFrames = false;
                        isOverlay = false;
                    }
                    else if (trimmedLine == "animation")
                    {
                        scanningParameters = false;
                        scanningFrames = true;
                    }
                    else if (scanningParameters)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length != 2)
                            continue;
                        string key = parts[0].ToLower().Trim();
                        string value = parts[1].Trim();
                        switch (key)
                        {
                            case "name": animation.Name = value; break;
                            case "color": animation.ColorHex = value; break;
                            case "overlayname": animation.OverlayKey = value; break;
                            case "isoverlay": isOverlay = value.ToLower() == "true"; break;
                            default: break;
                        }
                    }
                    else if (scanningFrames)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string lineToAdd = line.Replace("\r", "").Replace("\n", "");
                            animation.FrameLines.Add(lineToAdd);
                        }
                    }
                }

                if (animation != null && animation.IsValid() && animation.Name != null)
                {
                    if (!isOverlay)
                    {
                        if (LoadedAnimations.TryAdd(animation.Name, animation))
                            nbAnim++;
                    }
                    else
                    {
                        if (LoadedOverlays.TryAdd(animation.Name, animation))
                            nbAnim++;
                    }
                }

                return nbAnim;
            }
            catch (Exception ex)
            {
                Plugin.logger.LogError($"[AnimationLoader] Unexpected error while reading the file : {ex}");
                return 0;
            }
        }


        /// <summary>
        /// Load all the predefined animations that are hardcoded by GeneralImprovements, and add them to the LoadedAnimations dictionary.
        /// </summary>
        internal static void LoadAllPredefinedAnimations()
        {
            if (!LoadedAnimations.ContainsKey("None"))
            {
                LoadedAnimations.Add("None", new FancyWeatherAnimation()
                {
                    Name = "None",
                    FullFrames = WeatherASCIIArt.ClearAnimations.ToList(),
                });
            }
            if (!LoadedAnimations.ContainsKey("Rainy"))
            {
                LoadedAnimations.Add("Rainy", new FancyWeatherAnimation()
                {
                    Name = "Rainy",
                    FullFrames = WeatherASCIIArt.RainAnimations.ToList(),
                });
            }
            if (!LoadedAnimations.ContainsKey("Stormy"))
            {
                LoadedAnimations.Add("Stormy", new FancyWeatherAnimation()
                {
                    Name = "Stormy",
                    OverlayKey = "Stormy",
                    FullFrames = WeatherASCIIArt.RainAnimations.ToList(),
                });
            }
            if (!LoadedAnimations.ContainsKey("Flooded"))
            {
                LoadedAnimations.Add("Flooded", new FancyWeatherAnimation()
                {
                    Name = "Flooded",
                    FullFrames = WeatherASCIIArt.FloodedAnimations.ToList(),
                });
            }
            if (!LoadedAnimations.ContainsKey("Foggy"))
            {
                LoadedAnimations.Add("Foggy", new FancyWeatherAnimation()
                {
                    Name = "Foggy",
                    FullFrames = WeatherASCIIArt.FoggyAnimations.ToList(),
                });
            }
            if (!LoadedAnimations.ContainsKey("Eclipsed"))
            {
                LoadedAnimations.Add("Eclipsed", new FancyWeatherAnimation()
                {
                    Name = "Eclipsed",
                    FullFrames = WeatherASCIIArt.EclipsedAnimations.ToList(),
                });
            }
            if (!LoadedOverlays.ContainsKey("Stormy"))
            {
                LoadedOverlays.Add("Stormy", new FancyWeatherAnimation()
                {
                    Name = "Stormy",
                    ColorHex = "#ffe100",
                    FullFrames = WeatherASCIIArt.LightningOverlays.ToList(),
                });
            }
        }
    }
}
