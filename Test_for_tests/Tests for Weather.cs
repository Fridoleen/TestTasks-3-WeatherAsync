using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI;
using TestTasks.WeatherFromAPI.Models;

namespace Test_for_tests
{
    [TestClass]
    public class WeatherTesting
    {
        [TestMethod]
        public void SumRainTest()
        {
            var hourly = new List<HourWeather>();
            hourly.Add(new HourWeather(14.2, 2));
            hourly.Add(new HourWeather(14.5, 1.9));
            hourly.Add(new HourWeather(15.12, 3));
            hourly.Add(new HourWeather(13.4, 3.5));
            hourly.Add(new HourWeather(9.8, 4.71));
            hourly.Add(new HourWeather(14.9, 2.398));
            hourly.Add(new HourWeather(22.7, 3.435));
            hourly.Add(new HourWeather(11.2, 1.091));

            var weather = new WeatherManager();
            Assert.AreEqual(weather.SumRainVolume(hourly.ToArray()), 22.034);
        }

        [TestMethod]
        public void AvgTempTest()
        {
            var hourly = new List<HourWeather>();
            hourly.Add(new HourWeather(14.2, 2));
            hourly.Add(new HourWeather(14.5, 1.9));
            hourly.Add(new HourWeather(15.12, 3));
            hourly.Add(new HourWeather(13.4, 3.5));
            hourly.Add(new HourWeather(9.8, 4.71));
            hourly.Add(new HourWeather(14.9, 2.398));
            hourly.Add(new HourWeather(22.7, 3.435));
            hourly.Add(new HourWeather(11.2, 1.091));

            var weather = new WeatherManager();
            Assert.AreEqual(weather.AvgTemperature(hourly.ToArray()), 14.4775);
        }

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
        public async Task RainDataCheck()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            City cityOne = new City("London", 51.5085, -0.1257);
            DayWeatherInfo dayWeather = await weather.GetWeatherDayHistory(cityOne, 1624914865);
            bool result = false;
            double RainVolume = 0;
            foreach (var hr in dayWeather.Hourly)
            {
                if (hr.rain != null)
                    if (hr.rain.rainVolume > 0)
                    {
                        RainVolume = hr.rain.rainVolume;
                        result = true;
                        break;
                    }
            }

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public async Task GetCityWeatherCorrectDataTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            City city = new City("London", 51.5085, -0.1257);
            DayWeatherInfo dayWeather = await weather.GetWeatherDayHistory(city, 1624914865);

            Assert.AreEqual(dayWeather.Hourly.Count, 24);            
        }

        [TestMethod]
        public async Task GetDailyCityWeatherTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            City city = new City("London", 51.5085, -0.1257);
            DayWeatherInfo dayWeather = await weather.GetWeatherDayHistory(city, 1624914865);

            Assert.AreEqual(weather.SumRainVolume(dayWeather.Hourly), 3.03);

            Assert.AreEqual(weather.AvgTemperature(dayWeather.Hourly), 290.394583);
        }

        [TestMethod]
        public async Task GetTodayWeatherTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            City city = new City("London", 51.5085, -0.1257);

            var dayWeather = await weather.GetTodayWeather(city);

            Assert.AreEqual((dayWeather.Hourly.Count > 0), true);
        }

        [TestMethod]
        public async Task CompareWeatherDaysCitiesTest()
        {
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            City cityOne = new City("London", 51.5085, -0.1257);
            DayWeatherInfo dayWeatherOne = await weather.GetWeatherDayHistory(cityOne, 1624914865);
            City cityTwo = await weather.GetCityByName("Kyiv");
            DayWeatherInfo dayWeatherTwo = await weather.GetWeatherDayHistory(cityTwo, 1624914865);

            Assert.AreEqual(weather.AvgTemperature(dayWeatherOne.Hourly), 290.394583);
            Assert.AreEqual(weather.SumRainVolume(dayWeatherOne.Hourly), 3.03);

            Assert.AreEqual(weather.AvgTemperature(dayWeatherTwo.Hourly), 294.869167);
            Assert.AreEqual(weather.SumRainVolume(dayWeatherTwo.Hourly), 0);

            Assert.AreEqual(weather.FirstDayIsWarmer(dayWeatherOne, dayWeatherTwo), false);
            Assert.AreEqual(weather.FirstDayIsRainier(dayWeatherOne, dayWeatherTwo), true);
        }

        [TestMethod]
        public async Task CompareWeatherCitiesTest()
        {
            int days = 3;
            var weather = new WeatherManager();
            ApiHelper.InitializeClient();
            var cityOne = "Kyiv";

            var cityTwo = "London";

            var compared = await weather.CompareWeather(cityTwo, cityOne, days);
            Assert.AreEqual(compared.RainierDaysCount, 2);

            compared = await weather.CompareWeather(cityOne, cityTwo, days);
            Assert.AreEqual(compared.RainierDaysCount, 1);

            compared = await weather.CompareWeather(cityOne, cityTwo, days);
            Assert.AreEqual(compared.WarmerDaysCount, 3);

            compared = await weather.CompareWeather(cityTwo, cityOne, days);
            Assert.AreEqual(compared.WarmerDaysCount, 0);
        }


    }
}
