using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks.WeatherFromAPI.Models
{
    public class HourWeather
    {
        public double Temp { get; set; }
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
