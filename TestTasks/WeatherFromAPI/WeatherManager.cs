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

            int warmerDays = 0;
            int rainierDays = 0;

            City firstCity = await GetCityByName(cityA);
            City secondCity = await GetCityByName(cityB);                      

            for (int i = 0; i < dayCount; i++)
            {
                DayWeatherInfo dayCityOne = await GetHourlyWeather(firstCity, i);
                DayWeatherInfo dayCityTwo = await GetHourlyWeather(secondCity, i);

                (bool warmer, bool rainier) = FirstDayIsWarmerNRainier(dayCityOne, dayCityTwo);
                if (warmer) warmerDays++;
                if (rainier) rainierDays++;
            }

            return new WeatherComparisonResult(cityA, cityB, warmerDays, rainierDays);
        }

        public async Task<City> GetCityByName(string cityName)
        {
            string url = geoCityUrl + $"q={cityName}&limit=1&appid=" + APIkey;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
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

        public async Task<DayWeatherInfo> GetHourlyWeather(City city, int daysAgo)
        {
            string url;

            if(daysAgo == 0)
            {
                url = todayWeatherUrl + $"lat={city.Lat}&lon={city.Lon}&appid=" + APIkey;
            }
            else
            {
                long time = DateTimeOffset.Now.ToUnixTimeSeconds() - 86400 * daysAgo;

                url = pastWeatherUrl + $"lat={city.Lat}&lon={city.Lon}" +
                            $"&dt={time}&exclude=current&appid=" + APIkey;
            }

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
       
        public (bool, bool) FirstDayIsWarmerNRainier(DayWeatherInfo dayOne, DayWeatherInfo dayTwo)
        {
            (double avgTempOne, double rainVolOne) = GetAvgTempNRain(dayOne.Hourly);
            (double avgTempTwo, double rainVolTwo) = GetAvgTempNRain(dayTwo.Hourly);

            return ((avgTempOne > avgTempTwo), (rainVolOne) > (rainVolTwo));
        }

        /// <summary>
        /// Returns daily averaged data
        /// </summary>
        /// <param name="hourly"></param>
        /// <returns>(average_hourly_temperature, daily_amount_of_rain)</returns>
        public (double, double) GetAvgTempNRain(ICollection<HourWeather> hourly)
        {
            double AvgTemp = 0;
            double rainVol = 0;

            foreach(var hour in hourly)
            {
                rainVol += hour.rain.rainVolume;
                AvgTemp += hour.Temp;
            }

            if (hourly.Count > 0) AvgTemp /= hourly.Count;
            return (Math.Round(AvgTemp, 6), Math.Round(rainVol, 6));            
        }
    }
}
