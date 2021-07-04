using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI;
using TestTasks.WeatherFromAPI.Models;

namespace Test_for_tests
{
    [TestClass]
    public class WeatherTesting
    {
        [TestMethod]
        public async Task CityGetTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();

            string cityName = "kyiv,ua";            
            var city = await weather.GetCityByName(cityName);
            Assert.AreEqual(city.Name, "Kyiv");
            Assert.AreEqual(city.Lat, 50.4333);
            Assert.AreEqual(city.Lon, 30.5167);

            cityName = "London,uk";
            city = await weather.GetCityByName(cityName);
            Assert.AreEqual(city.Name, "London");
            Assert.AreEqual(city.Lat, 51.5085);
            Assert.AreEqual(city.Lon, -0.1257);

            cityName = "aleppo";
            city = await weather.GetCityByName(cityName);
            Assert.AreEqual(city.Name, "Aleppo Governorate");
            Assert.AreEqual(city.Lat, 36.25);
            Assert.AreEqual(city.Lon, 37.5);
        }

        [TestMethod]
        public void GetAvgMeasurmentsTest()
        {
            var weather = new WeatherManager();
            var hourly = new HourWeather[]
            {
                new HourWeather(141, 0),
                new HourWeather(100, 0.6),
                new HourWeather(101, 0.131),
                new HourWeather(102, 0.859),
                new HourWeather(104, 2.231),
                new HourWeather(142, 4.6666),
                new HourWeather(109, 2.77777),
                new HourWeather(118, 1.000001),
                new HourWeather(126, 2.238),
                new HourWeather(9, 0)
            };

            (double warmth, double rain) = weather.GetAvgTempNRain(hourly);

            Assert.AreEqual(warmth, 105.2);
            Assert.AreEqual(rain, 14.503371 );
        }

        [TestMethod]
        public void CompareDays()
        {
            var weather = new WeatherManager();

            var hourly = new HourWeather[]
            {
                new HourWeather(141, 0),
                new HourWeather(100, 0.6),
                new HourWeather(101, 0.131),
                new HourWeather(102, 0.859),
                new HourWeather(104, 2.231),
                new HourWeather(142, 4.6666),
                new HourWeather(109, 2.77777),
                new HourWeather(118, 1.000001),
                new HourWeather(126, 2.238),
                new HourWeather(9, 0)
            };
            DayWeatherInfo dayOne = new DayWeatherInfo(0, 1, hourly);

            hourly = new HourWeather[]
            {
                new HourWeather(140, 0),
                new HourWeather(99, 1.6),
                new HourWeather(90, 0.131),
                new HourWeather(89, 2.859),
                new HourWeather(22, 2.231),
                new HourWeather(33, 3.6666),
                new HourWeather(97, 5.77777),
                new HourWeather(120, 4.000001),
                new HourWeather(100, 3.238),
                new HourWeather(15, 2)
            };
            DayWeatherInfo dayTwo = new DayWeatherInfo(1, 0, hourly);

            (bool warmer, bool rainier) = weather.FirstDayIsWarmerNRainier(dayOne, dayTwo);

            Assert.AreEqual(warmer, true);
            Assert.AreEqual(rainier, false);

        }

        [TestMethod]
        public async Task GetDayWeatherTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();

            City city = new City("Kyiv", 50.4333, 30.5167);
            var cityWeatherToday = await weather.GetHourlyWeatherDay(city, 0);
            (double temp, double rain) = weather.GetAvgTempNRain(cityWeatherToday.Hourly);
            Assert.AreEqual(cityWeatherToday.Hourly.Count > DateTime.Now.Hour , true);
            Assert.AreEqual(rain > 0, true);

            city = new City("London", 51.5085, -0.1257);
            cityWeatherToday = await weather.GetHourlyWeatherDay(city, 2);
            (temp, rain) = weather.GetAvgTempNRain(cityWeatherToday.Hourly);
            Assert.AreEqual(cityWeatherToday.Hourly.Count, 24);
            Assert.AreEqual(temp > 0, true);
            Assert.AreEqual(rain > 0, true); //there was rain in London day before yesterday

            city = new City("aleppo", 36.25, 37.5);
            cityWeatherToday = await weather.GetHourlyWeatherDay(city, 3);
            (temp, rain) = weather.GetAvgTempNRain(cityWeatherToday.Hourly);
            Assert.AreEqual(cityWeatherToday.Hourly.Count, 24);
            Assert.AreEqual(temp > 0, true);
            Assert.AreEqual(rain > 0, false); //there were no rain in Aleppo 3 days ago

        }        

        [TestMethod]
        public async Task CompareWeatherCitiesTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();

            var compared = await weather.CompareWeather("London", "Aleppo", 3);
            Assert.AreEqual(compared.RainierDaysCount, 3);

            compared = await weather.CompareWeather("Kyiv", "London", 2);
            Assert.AreEqual(compared.RainierDaysCount, 0);

            compared = await weather.CompareWeather("Aleppo", "Kyiv", 4);
            Assert.AreEqual(compared.WarmerDaysCount, 4);

            compared = await weather.CompareWeather("Aleppo", "London", 2);
            Assert.AreEqual(compared.WarmerDaysCount, 2);
        }


    }
}
