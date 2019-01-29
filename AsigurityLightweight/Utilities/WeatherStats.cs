using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AsigurityLightweight.Utilities
{
    public class WeatherCondition
    {
        public string MainCondition { get; set; }
        public string DescriptionForCondition { get; set; }
        public string IconCode { get; set; }
        public string Icon { get; set; }
    }

    public class WeatherStats
    {
        public string City { get; set; }
        public string[] Temperature { get; set; }
        public string[] MinTemperature { get; set; }
        public string[] MaxTemperature { get; set; }
        public string[] Humidity { get; set; }
        public string[] WindSpeed { get; set; }
        public WeatherCondition[] WeatherCondition { get; set; }

        public WeatherStats()
        {
            Temperature = new string[5];
            MinTemperature = new string[5];
            MaxTemperature = new string[5];
            Humidity = new string[5];
            WindSpeed = new string[5];
            WeatherCondition = new WeatherCondition[5] { new WeatherCondition(), new WeatherCondition(), new WeatherCondition(), new WeatherCondition(), new WeatherCondition() };
            for (int i = 0; i < 5; i++)
            {
                WeatherCondition[i].MainCondition = string.Empty;
                WeatherCondition[i].DescriptionForCondition = string.Empty;
                WeatherCondition[i].IconCode = string.Empty;
                WeatherCondition[i].Icon = string.Empty;
            }
        }
    }
}