namespace ApiDemo.Models
{
    public class Exercise
    {
        public string name { get; set; } = "";

        public string? force { get; set; }
        public string? level { get; set; }
        public string? mechanic { get; set; }
        public string? equipment { get; set; }
        public List<string> primary_muscles { get; set; } = new List<string>();
        public List<string>? secondary_muslces { get; set; } 
        public List<string>? instructions { get; set; }
        public string? category { get; set; }
    }
}
