using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TestTasks.WeatherFromAPI.Models;

namespace TestTasks.WeatherFromAPI
{
    public class WeatherManager
    {
        private const string APIkey = "ea39974ee15009777124d9c22a2a2caa";
        private const string geoCityUrl = "geo/1.0/direct?";
        private const string pastWeatherUrl = "data/2.5/onecall/timemachine?";
        private const string todayWeatherUrl = "data/2.5/onecall?";

        public async Task<WeatherComparisonResult> CompareWeather(string cityA, string cityB, int dayCount)
        {
            if (dayCount < 1 || dayCount > 5) throw new ArgumentException();

            uint currentTime = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

            int warmerDays = 0;
            int rainierDays = 0;

            City firstCity = await GetCityByName(cityA);
            City secondCity = await GetCityByName(cityB);                      

            for (int i = 1; i < dayCount; i++)
            {
                uint dayAtUnixTime = (uint) (currentTime - 86400 * i);
                DayWeatherInfo dayCityOne = await GetWeatherDayHistory(firstCity, dayAtUnixTime);
                DayWeatherInfo dayCityTwo = await GetWeatherDayHistory(secondCity, dayAtUnixTime);

                if (FirstDayIsWarmer(dayCityOne, dayCityTwo)) warmerDays++;
                if (FirstDayIsRainier(dayCityOne, dayCityTwo)) rainierDays++;
            }

            bool temp = await TodayFirstWasWarmer(firstCity, secondCity);
            if (temp) warmerDays++;

            temp = await TodayFirstWasRainier(firstCity, secondCity);
            if (temp) rainierDays++;

            var result = new WeatherComparisonResult(cityA, cityB, warmerDays, rainierDays);

            return await Task.Run(() => result);
        }

        public async Task<bool> TodayFirstWasWarmer(City cityOne, City cityTwo)
        {
            DayWeatherInfo dayCityOne = await GetTodayWeather(cityOne);
            DayWeatherInfo dayCityTwo = await GetTodayWeather(cityTwo);

            return FirstDayIsWarmer(dayCityOne, dayCityTwo);
        }

        public async Task<bool> TodayFirstWasRainier(City cityOne, City cityTwo)
        {
            DayWeatherInfo dayCityOne = await GetTodayWeather(cityOne);
            DayWeatherInfo dayCityTwo = await GetTodayWeather(cityTwo);

            return FirstDayIsRainier(dayCityOne, dayCityTwo);
        }

        public async Task<DayWeatherInfo> GetTodayWeather(City city)
        {
            string url = todayWeatherUrl + $"lat={city.Lat}&lon={city.Lon}&appid=" + APIkey;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    DayWeatherInfo result = await response.Content.ReadAsAsync<DayWeatherInfo>();

                    return result;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public async Task<City> GetCityByName(string cityName)
        {
            string url = geoCityUrl + $"q={cityName}&limit=1&appid=" + APIkey;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if(response.IsSuccessStatusCode)
                {
                    City[] result = await response.Content.ReadAsAsync<City[]>();

                    return result[0];
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public async Task<DayWeatherInfo> GetWeatherDayHistory(City city, uint time)
        {
            string url = pastWeatherUrl + $"lat={city.Lat}&lon={city.Lon}"+
                            $"&dt={time}&exclude=current&appid=" + APIkey;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    DayWeatherInfo result = await response.Content.ReadAsAsync<DayWeatherInfo>();

                    return result;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

        }

        public bool FirstDayIsWarmer(DayWeatherInfo dayOne, DayWeatherInfo dayTwo)
        {
            bool result = (AvgTemperature(dayOne.Hourly) > AvgTemperature(dayTwo.Hourly));

            return result;
        }

        public bool FirstDayIsRainier(DayWeatherInfo dayOne, DayWeatherInfo dayTwo)
        {
            bool result = (SumRainVolume(dayOne.Hourly) > SumRainVolume(dayTwo.Hourly));

            return result;
        }

        public double SumRainVolume(ICollection<HourWeather> hourly)
        {
            double result = 0;

            foreach(var hour in hourly)
            {
                result += hour.rain.rainVolume;
            }

            //For test purposes easier to round
            return Math.Round(result, 6);      
            //return result;
        }

        public double AvgTemperature(ICollection<HourWeather> hourly)
        {
            double result = 0;

            foreach(var hour in hourly)
            {
                result += hour.Temp;
            }

            if(hourly.Count > 0) result /= hourly.Count;

            //for test purposes easier to round
            return Math.Round(result, 6);
            //return result;
        }
    }
}
