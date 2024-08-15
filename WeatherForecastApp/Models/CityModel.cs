namespace WeatherForecastApp.Models
{
    public class CityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<WeatherDataModel> WeatherData { get; set; }
        public double Latitude { get; set; } // Yeni eklenen özellik
        public double Longitude { get; set; } // Yeni eklenen özellik
    }
}
