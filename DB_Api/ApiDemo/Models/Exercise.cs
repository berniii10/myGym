namespace ApiDemo.Models
{
    public class Exercise
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? force { get; set; }
        public string? level { get; set; }
        public string? mechanic { get; set; }
        public string? equipment { get; set; }
        public string? category { get; set; }
        public string? image_url { get; set; }
        public List<Instruction> instructions { get; set; }
        public List<Muscle> primary_muscles { get; set; }
        public List<Muscle>? secondary_muscles { get; set; }

        public Exercise()
        {
            instructions = new List<Instruction>();
            primary_muscles = new List<Muscle>();
            secondary_muscles = new List<Muscle>();
        }
    }
}
