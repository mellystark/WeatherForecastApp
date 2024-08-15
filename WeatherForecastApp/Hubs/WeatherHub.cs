using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Hubs
{
    [AllowAnonymous]
    public class WeatherHub : Hub
    {
        // İstemcilere hava durumu verilerini göndermek için bir metod.
        public async Task SendWeatherData(WeatherDataModel weatherData)
        {
            await Clients.All.SendAsync("ReceiveWeatherData", weatherData);
        }
    }
}
