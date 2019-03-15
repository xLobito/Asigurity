using System;
using System.Threading.Tasks;
using System.Net.Http;
using Android.Util;
using AsigurityLightweight.Interfaces;
using AsigurityLightweight.Utilities;
using Newtonsoft.Json;

namespace AsigurityLightweight.Implementations
{
    public class Weather : IWeather
    {
        public async Task<WeatherStats> GetWeatherAndForecastAsync(string CityName)
        {
            string WeatherKey = "99e2eb1ae41b89b6fe1ccd5be89886df";
            string WeatherQuery = "https://api.openweathermap.org/data/2.5/forecast?q=" + CityName + ",cl&appid=" + WeatherKey + "&units=metric&lang=es";
            dynamic WeatherResultQuery;
            WeatherStats WeatherDailyForecast;

            WeatherResultQuery = await GetWeatherFromService(WeatherQuery).ConfigureAwait(false);
            if (WeatherResultQuery["list"] != null)
            {
                WeatherDailyForecast = new WeatherStats();
                WeatherDailyForecast.City = CityName;
                for (int i = 0; i < 5; i++)
                {
                    WeatherDailyForecast.Temperature[i] = (string)WeatherResultQuery["list"][i]["main"]["temp"] + " °C";
                    WeatherDailyForecast.MinTemperature[i] = "Mínima: " + (string)WeatherResultQuery["list"][i]["main"]["temp_min"] + " °C";
                    WeatherDailyForecast.MaxTemperature[i] = "Máxima: " + (string)WeatherResultQuery["list"][i]["main"]["temp_max"] + " °C";
                    WeatherDailyForecast.Humidity[i] = "Humedad Relativa: " + (string)WeatherResultQuery["list"][i]["main"]["humidity"] + "%";
                    WeatherDailyForecast.WindSpeed[i] = "Velocidad del Viento: " + (string)WeatherResultQuery["list"][i]["wind"]["speed"] + " Km/h";
                    WeatherDailyForecast.WeatherCondition[i].MainCondition = (string)WeatherResultQuery["list"][i]["weather"][0]["main"];
                    WeatherDailyForecast.WeatherCondition[i].DescriptionForCondition = (string)WeatherResultQuery["list"][i]["weather"][0]["description"];
                    WeatherDailyForecast.WeatherCondition[i].IconCode = (string)WeatherResultQuery["list"][i]["weather"][0]["id"];
                    WeatherDailyForecast.WeatherCondition[i].Icon = (string)WeatherResultQuery["list"][i]["weather"][0]["icon"];
                }
                return WeatherDailyForecast;
            }
            else
                return null;
        }

        public async Task<dynamic> GetWeatherFromService(string WeatherQuery)
        {
            dynamic DataRetrieved = null;

            try
            {
                using (HttpClient WebClient = new HttpClient())
                {
                    var WebResponse = await WebClient.GetAsync(WeatherQuery);
                    if(WebResponse != null)
                        DataRetrieved = JsonConvert.DeserializeObject(WebResponse.Content.ReadAsStringAsync().Result);
                }
                return DataRetrieved;
            }
            catch (Exception ex)
            {
                Log.Debug("Asigurity Weather System", ex.Message);
                return null;
            }
        }
    }
}