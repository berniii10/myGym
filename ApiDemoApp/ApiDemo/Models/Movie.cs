using System.Text.Json.Serialization;
using System;

namespace ApiDemo.Models
{
    public class Movie
    {
        public int Id { get; set; }
        
        public string title { get; set; } = string.Empty;
        
        public int release_year { get; set; }
        
        // Mirar com serialitzar
        // [JsonConverter(typeof(StringEnumConverter<Genre>))]
        public List<String> genre { get; set; } = new List<String>();
        
        public double rating { get; set; }
        
        public string summary { get; set; } = string.Empty;
        
        public List<Actor> actors { get; set; } = new List<Actor>();
    }
}
