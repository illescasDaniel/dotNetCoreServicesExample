using System;
using System.ComponentModel.DataAnnotations;

namespace myMicroservice
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [Required] // automatically validates stuff?
        public string? Summary { get; set; }
    }
}
