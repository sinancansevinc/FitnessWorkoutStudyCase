using FitnessWorkout.Models;

namespace FitnessWorkout.Repositories
{
    public interface IWorkoutRepository
    {
        Task<IEnumerable<BodyRegion>> GetBodyRegions();
        Task<IEnumerable<Workout>> GetFilteredWorkouts(int? duration, Difficulty? difficulty, string bodyRegion, bool useAndFilter);
        Task InsertMovement(Movement movement, int userId);
        Task<Workout> GetWorkoutById(int workoutId);
        Task<int> InsertWorkout(Workout workout, int userId);
        Task InsertWorkoutBodyRegion(int workoutId, int bodyRegionId);
    }
}