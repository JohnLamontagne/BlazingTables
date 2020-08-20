using System;

namespace BlazingTables.Test.Data
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public Description Summary { get; set; }

        public enum Description
        {
            Sunny,
            Rainy,
            Overcast
        }
    }
}