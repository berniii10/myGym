namespace ApiDemo.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Force { get; set; }
        public string Level { get; set; }
        public string Mechanic { get; set; }
        public string Equipment { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public List<Instruction> Instructions { get; set; }
        public List<Muscle> PrimaryMuscles { get; set; }
        public List<Muscle> SecondaryMuscles { get; set; }

        public Exercise()
        {
            Instructions = new List<Instruction>();
            PrimaryMuscles = new List<Muscle>();
            SecondaryMuscles = new List<Muscle>();
        }
    }
}
