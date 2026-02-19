using System.Collections.Generic;

namespace FancyWeatherAPI.API
{
    public class FancyWeatherAnimation
    {
        public string? Name { get; set; }

        public bool WithOverlay { get; set; } = false;

        public List<string> Frames { get; set; } = new List<string>();

        public FancyWeatherAnimation() { }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && Frames.Count > 0;
        }
    }
}
