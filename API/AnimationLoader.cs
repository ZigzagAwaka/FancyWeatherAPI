using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FancyWeatherAPI.API
{
    internal class AnimationLoader
    {
        public static Dictionary<string, FancyWeatherAnimation> LoadedAnimations = new Dictionary<string, FancyWeatherAnimation>();


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
                            LoadedAnimations.TryAdd(animation.Name, animation);
                        }
                        animation = new FancyWeatherAnimation();
                        scanningParameters = true;
                        scanningFrames = false;
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
                        if (key == "name")
                            animation.Name = value;
                        else if (key == "withoverlay")
                            animation.WithOverlay = value == "true";
                    }
                    else if (scanningFrames)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string lineToAdd = line;
                            if (lineToAdd.Length < 8)
                            {
                                lineToAdd = lineToAdd.PadRight(8, ' ');
                            }
                            animation.Frames.Add(lineToAdd);
                        }
                    }
                }

                if (animation != null && animation.IsValid() && animation.Name != null)
                {
                    LoadedAnimations.TryAdd(animation.Name, animation);
                }

                return LoadedAnimations.Count;
            }
            catch (Exception ex)
            {
                Plugin.logger.LogError($"[AnimationLoader] Unexpected error while reading the file : {ex}");
                return 0;
            }
        }
    }
}
