using AsigurityLightweight.Utilities;
using System.Threading.Tasks;

namespace AsigurityLightweight.Interfaces
{
    public interface IWeather
    {
        Task<WeatherStats> GetWeatherAndForecastAsync(string CityName);
    }
}