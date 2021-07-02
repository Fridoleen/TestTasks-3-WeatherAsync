using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.WeatherFromAPI.Models
{
    public class City
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public double Lat { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Lon { get; set; }

        public City(string name, double lattitude, double longitude)
        {
            Name = name;
            Lat = lattitude;
            Lon = longitude;
        }

    }
}
