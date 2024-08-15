using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<CityModel> Cities { get; set; }
        public DbSet<WeatherDataModel> WeatherData { get; set; }
    }
}