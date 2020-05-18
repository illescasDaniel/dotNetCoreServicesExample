using System;
using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Api.v1.Models
{
    public class WeatherForecastDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int TemperatureC { get; set; }

        [Required]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [Required]
        public string? Summary { get; set; }
    }
}
