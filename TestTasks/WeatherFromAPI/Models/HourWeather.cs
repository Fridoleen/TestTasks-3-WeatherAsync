using Newtonsoft.Json;

namespace TestTasks.WeatherFromAPI.Models
{
    public class HourWeather
    {
        [JsonProperty(PropertyName = "temp")]
        public double Temp { get; set; }
        
        [JsonProperty(PropertyName = "rain")]
        public Rain rain { get; set; }

        public HourWeather(double _temp, double _rain)
        {
            Temp = _temp;
            rain = new Rain(_rain);
        }

        public HourWeather(double _temp)
        {
            Temp = _temp;
            rain = new Rain();
        }

        public HourWeather()
        {
            Temp = 0;
            rain = new Rain();
        }
    }
}
