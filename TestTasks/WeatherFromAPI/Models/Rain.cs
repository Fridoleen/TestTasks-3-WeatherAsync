using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.WeatherFromAPI.Models
{
    public class Rain
    {
        [JsonProperty(PropertyName = "1h")]
        public double rainVolume { get; set; }

        public Rain(double r)
        {
            rainVolume = r;
        }

        public Rain()
        {
        }
    }
}
