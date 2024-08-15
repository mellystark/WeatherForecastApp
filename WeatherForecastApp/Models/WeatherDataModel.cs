namespace WeatherForecastApp.Models
{
    public class WeatherDataModel
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public CityModel City { get; set; }
        public float Temperature { get; set; }
        public float Rain { get; set; }
        public float Windspeed { get; set; }
        public int Humidity { get; set; }
        public int Clouds { get; set; }
        public DateTime DateTime { get; set; }
    }
}
