using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherForecastApp.Data;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Services
{
    public interface IWeatherService
    {
        Task<WeatherDataModel> AddRandomWeatherDataAsync();
        Task<List<WeatherDataModel>> GetAllWeatherDataAsync();
        Task<WeatherDataModel> GetWeatherDataByCityAsync(string city);
        Task<List<string>> GetCityNamesAsync();
        Task<PaginatedResult<WeatherDataModel>> GetPaginatedWeatherDataByCityAsync(string city, int pageNumber, int pageSize);
        Task InitializeIzmirWeatherDataAsync();
    }

    public class WeatherService : IWeatherService
    {
        private readonly WeatherDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CityCacheKey = "CityCacheKey";

        public WeatherService(WeatherDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<WeatherDataModel> AddRandomWeatherDataAsync()
        {
            // Şehir isimlerini alın
            var cityNames = await GetCityNamesAsync();
            WeatherDataModel latestWeatherData = null;

            // Hava durumu verisi eklemek için her şehir için rastgele veri oluştur
            foreach (var cityName in cityNames)
            {
                var random = new Random();

                // Şehir modelini veritabanından al
                var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityName);

                if (city != null) // Eğer şehir bulunduysa
                {
                    var weatherData = new WeatherDataModel
                    {
                        City = city, // CityModel türünde
                        Temperature = random.Next(-10, 40), // Rastgele sıcaklık
                        Rain = random.Next(0, 100), // Rastgele yağış miktarı
                        Windspeed = random.Next(0, 150), // Rastgele rüzgar hızı
                        Humidity = random.Next(0, 100), // Rastgele nem
                        Clouds = random.Next(0, 100), // Rastgele bulut oranı
                        DateTime = DateTime.UtcNow // Geçerli tarih ve saat
                    };

                    // Veritabanına ekle
                    _context.WeatherData.Add(weatherData);
                    latestWeatherData = weatherData;
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();
            return latestWeatherData;
        }


        public async Task<List<WeatherDataModel>> GetAllWeatherDataAsync()
        {
            return await _context.WeatherData.ToListAsync();
        }

        public async Task<WeatherDataModel> GetWeatherDataByCityAsync(string city)
        {
            return await _context.WeatherData
                .Where(w => w.City.Name == city)
                .OrderByDescending(w => w.DateTime)
                .Include(c => c.City)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetCityNamesAsync()
        {
            // Caching'den şehir isimlerini al
            if (!_cache.TryGetValue(CityCacheKey, out List<string> cityNames))
            {
                // Eğer cache'de yoksa, veritabanından al
                cityNames = await _context.Cities.Select(c => c.Name).Distinct().ToListAsync();

                // Cache'e ekle
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1)); // Cache süresi

                _cache.Set(CityCacheKey, cityNames, cacheEntryOptions);
            }

            return cityNames;
        }

        public async Task<PaginatedResult<WeatherDataModel>> GetPaginatedWeatherDataByCityAsync(string city, int pageNumber, int pageSize)
        {
            var query = _context.WeatherData
                .Where(w => w.City.Name == city)
                .OrderByDescending(w => w.DateTime)
                .Include(c => c.City);

            var totalCount = await query.CountAsync();

            var weatherData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<WeatherDataModel>
            {
                Data = weatherData,
                TotalCount = totalCount
            };
        }

        public async Task InitializeIzmirWeatherDataAsync()
        {
            var cityName = "İzmir";
            var existingWeatherData = await GetWeatherDataByCityAsync(cityName);

            if (existingWeatherData == null)
            {
                // Eğer İzmir için hava durumu verisi yoksa, rastgele veri ekle
                var random = new Random();

                var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityName);
                if (city != null) // Eğer şehir bulunduysa
                {
                    var weatherData = new WeatherDataModel
                    {
                        City = city,
                        Temperature = random.Next(-10, 40),
                        Rain = random.Next(0, 100),
                        Windspeed = random.Next(0, 150),
                        Humidity = random.Next(0, 100),
                        Clouds = random.Next(0, 100),
                        DateTime = DateTime.UtcNow
                    };

                    // Veritabanına ekle
                    _context.WeatherData.Add(weatherData);
                    await _context.SaveChangesAsync();
                }
            }
        }


    }
}