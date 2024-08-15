using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherForecastApp.Models;
using WeatherForecastApp.Services;

namespace WeatherForecastApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherDataController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherDataController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // Hava durumu verilerini rastgele oluşturmak için endpoint
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateRandomWeatherData()
        {
            await _weatherService.AddRandomWeatherDataAsync();
            return Ok("Rastgele hava durumu verisi eklendi.");
        }

        // Tüm hava durumu verilerini listelemek için endpoint
        [HttpGet]
        public async Task<ActionResult<List<WeatherDataModel>>> GetAllWeatherData()
        {
            var weatherData = await _weatherService.GetAllWeatherDataAsync();
            return Ok(weatherData);
        }

        // Belirli bir şehir için hava durumu verisini almak için endpoint
        [HttpGet("{city}")]
        public async Task<ActionResult<WeatherDataModel>> GetWeatherDataByCity(string city)
        {
            var weatherData = await _weatherService.GetWeatherDataByCityAsync(city);
            if (weatherData == null)
            {
                return NotFound("Hava durumu verisi bulunamadı.");
            }
            return Ok(weatherData);
        }

        [HttpGet("city/{city}/page/{pageNumber}/size/{pageSize}")]
        public async Task<ActionResult<PaginatedResult<WeatherDataModel>>> GetPaginatedWeatherDataByCityAsync(string city, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _weatherService.GetPaginatedWeatherDataByCityAsync(city, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("initialize-izmir")]
        public async Task<IActionResult> InitializeIzmirWeather()
        {
            await _weatherService.InitializeIzmirWeatherDataAsync();
            return Ok("İzmir hava durumu verileri başarıyla eklendi.");
        }

        [HttpGet("current/{city}")]
        public async Task<ActionResult<WeatherDataModel>> GetCurrentWeatherData(string city)
        {
            // Belirtilen şehir için son hava durumu verisini al
            var currentWeather = await _weatherService.GetWeatherDataByCityAsync(city);

            if (currentWeather == null)
            {
                return NotFound("Güncel hava durumu verisi bulunamadı.");
            }

            return Ok(currentWeather);
        }


    }
}
