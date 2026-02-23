using System;
using System.Collections.Generic;

namespace FancyWeatherAPI.API
{
    /// <summary>
    /// Define a custom weather animation
    /// </summary>
    public class FancyWeatherAnimation
    {
        /// <summary>
        /// The name of the targeted weather, this is required for the animation to be valid
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The base color of the displayed frames
        /// </summary>
        public string? ColorHex { get; set; }

        /// <summary>
        /// Define the overlay key used by the animation if it is not null
        /// </summary>
        public string? OverlayKey { get; set; }

        /// <summary>
        /// The list of lines that compose the animation, each 4 lines will be considered as a single frame, this is required for the animation to be valid
        /// </summary>
        public List<string> FrameLines { get; private set; } = new List<string>();

        /// <summary>
        /// The list of full frames that compose the animation, this is generated from the FrameLines list by concatenating every 4 lines together
        /// </summary>
        internal List<string> FullFrames { private get; set; } = new List<string>();

        public FancyWeatherAnimation() { }


        /// <summary>
        /// Checks if the animation is valid to be used. If it is valid, it also generates the FullFrames list by concatenating every 4 lines from the FrameLines list
        /// </summary>
        /// <returns>True if the animation is valid, and false otherwise</returns>
        public bool IsValid()
        {
            bool valid = !string.IsNullOrEmpty(Name) && FrameLines.Count > 0;

            if (valid)
            {
                string fullFrame = "";
                for (int i = 0; i < FrameLines.Count; i++)
                {
                    fullFrame += FrameLines[i];

                    if ((i + 1) % 4 == 0)
                    {
                        FullFrames.Add(fullFrame);
                        fullFrame = "";
                    }
                    else
                    {
                        fullFrame += Environment.NewLine;
                    }
                }

                if (ColorHex == null || !UnityEngine.ColorUtility.TryParseHtmlString(ColorHex, out _))
                {
                    ColorHex = null;
                }

                Plugin.DebugLog($"[FancyWeatherAnimation] The {Name} animation is valid");
            }

            return valid;
        }


        /// <summary>
        /// Returns an array containing all full frame strings currently stored.
        /// </summary>
        /// <returns>Array of strings representing the full frames</returns>
        public string[] GetFullFrames()
        {
            return FullFrames.ToArray();
        }
    }
}
