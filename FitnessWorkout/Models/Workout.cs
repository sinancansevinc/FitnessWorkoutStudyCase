namespace FitnessWorkout.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<BodyRegion> TargetedRegions { get; set; } = new List<BodyRegion>();
        public List<Movement> Movements { get; set; } = new List<Movement>();
        public int CreatedBy { get; set; } 
        public int UpdatedBy { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
