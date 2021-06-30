using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.WeatherFromAPI.Models
{
    public class City
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public City(string name, double lattitude, double longitude)
        {
            Name = name;
            Lat = lattitude;
            Lon = longitude;
        }

    }
}
