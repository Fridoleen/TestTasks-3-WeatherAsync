using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestTasks.WeatherFromAPI.Models
{
    public class DayWeatherInfo
    {
        [JsonProperty(PropertyName = "lat")]
        public double Lat { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Lon { get; set; }

        [JsonProperty(PropertyName = "hourly")]
        public List<HourWeather> Hourly { get; set; }

        public DayWeatherInfo(double lat, double lon, double[] temp, double[] rain)
        {
            Lat = lat;
            Lon = lon;

            Hourly = new List<HourWeather>();
            for(int i = 0; i < temp.Length; i++)
            {
                if (i < rain.Length)
                {
                    Hourly.Add(new HourWeather(temp[i], rain[i]));
                }
                else
                {
                    Hourly.Add(new HourWeather(temp[i], 0));
                }
            }
        }

        public DayWeatherInfo()
        {
            Lat = 0;
            Lon = 0;
            Hourly = new List<HourWeather>();
        }
    }
}
