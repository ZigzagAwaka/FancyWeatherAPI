using System;
using System.Collections.Generic;

namespace FancyWeatherAPI.API
{
    public class FancyWeatherAnimation
    {
        public string? Name { get; set; }

        public bool WithOverlay { get; set; } = false;

        public List<string> Frames { get; private set; } = new List<string>();

        private List<string> FullFrames { get; set; } = new List<string>();

        public FancyWeatherAnimation() { }


        public bool IsValid()
        {
            bool valid = !string.IsNullOrEmpty(Name) && Frames.Count > 0;

            if (valid)
            {
                Plugin.logger.LogError("azerty : " + Frames.Count);
                string fullFrame = "";
                for (int i = 0; i < Frames.Count; i++)
                {
                    fullFrame += Frames[i];

                    if ((i + 1) % 4 == 0)
                    {
                        FullFrames.Add(fullFrame);
                        fullFrame = "";
                        Plugin.logger.LogError("frame added : " + FullFrames.Count);
                    }
                    else
                    {
                        fullFrame += Environment.NewLine;
                    }
                }
            }

            return valid;
        }


        public string[] GetFullFrames()
        {
            foreach (string frame in FullFrames)
            {
                Plugin.logger.LogError(frame);
            }
            return FullFrames.ToArray();
        }
    }
}
