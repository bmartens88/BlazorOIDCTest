using System.Net.Http.Json;

namespace Blazor.Client.Weather;

internal sealed class ClientWeatherForecaster(HttpClient httpClient) : IWeatherForecaster
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast")
               ?? throw new IOException("No weather forecast!");
    }
}