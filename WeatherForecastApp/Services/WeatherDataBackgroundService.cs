using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherForecastApp.Services
{
    public class WeatherDataBackgroundService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public WeatherDataBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 15 dakika yerine 50 saniye olarak ayarlandı.
            _timer = new Timer(GenerateRandomWeatherData, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        private async void GenerateRandomWeatherData(object state)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();

                    // Rastgele hava durumu verilerini oluştur ve veritabanına kaydet.
                    var weatherData = await weatherService.AddRandomWeatherDataAsync();

                    // İsteğe bağlı: İşlemler sonrası yapılacak diğer işler
                }
            }
            catch (Exception ex)
            {
                // Hata günlüğü kaydedin
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
