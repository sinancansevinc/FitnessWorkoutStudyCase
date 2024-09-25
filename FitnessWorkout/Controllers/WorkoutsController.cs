using FitnessWorkout.Models;
using FitnessWorkout.Repositories;
using FitnessWorkout.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace FitnessWorkout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly CacheService _cacheService;

        public WorkoutsController(IWorkoutRepository workoutRepository, CacheService cacheService)
        {
            _workoutRepository = workoutRepository;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkouts(
        [FromQuery] int? duration,
        [FromQuery] Difficulty? difficulty,
        [FromQuery] string bodyRegion,
        [FromQuery] bool useAndFilter = true)
        {
            string cacheKey = $"FilterWorkout_Duration_{duration}_Difficulty_{difficulty}_BodyRegion_{bodyRegion}_UseAndFilter_{useAndFilter}";

            try
            {
                var workouts = _cacheService.GetOrCreate(cacheKey, () => _workoutRepository.GetFilteredWorkouts(duration, difficulty, bodyRegion, useAndFilter).Result);
                return Ok(workouts);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving workouts.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkout(int id)
        {
            try
            {
                var workout = await _workoutRepository.GetWorkoutById(id);
                if (workout == null)
                {
                    return NotFound(new { Message = "Workout not found." });
                }

                return Ok(workout);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the workout.", Error = ex.Message });
            }
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] int numberOfWorkouts = 10000)
        {
            var random = new Random();
            var userId = GetUserId(); // Implement this method to get the current user's ID

            try
            {
                // Get body regions and difficulties
                var bodyRegions = await _workoutRepository.GetBodyRegions();
                var bodyRegionIds = bodyRegions.Select(br => br.Id).ToList();
                var difficulties = Enum.GetValues(typeof(Difficulty));

                for (int i = 0; i < numberOfWorkouts; i++)
                {
                    // Create workout
                    var workout = new Workout
                    {
                        Name = $"Workout {i + 1}",
                        Description = $"Description for Workout {i + 1}",
                        DurationMinutes = random.Next(5, 61),
                        Difficulty = (Difficulty)difficulties.GetValue(random.Next(difficulties.Length)),
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };

                    var workoutId = await _workoutRepository.InsertWorkout(workout, userId);

                    // Random of region
                    int numberOfRegions = random.Next(1, 8);

                    //This is for unique selection
                    var selectedRegions = new HashSet<int>();


                    while (selectedRegions.Count < numberOfRegions)
                    {
                        // Random selected regionId added to selectedRegions
                        var regionId = bodyRegionIds[random.Next(bodyRegionIds.Count())];
                        selectedRegions.Add(regionId);
                    }

                    foreach (var regionId in selectedRegions)
                    {
                        // Adding workout - bodyregion relationship
                        await _workoutRepository.InsertWorkoutBodyRegion(workoutId, regionId);
                    }

                    for (int j = 0; j < random.Next(5, 21); j++)
                    {
                        var movement = new Movement
                        {
                            Name = $"Movement {j + 1} for Workout {i + 1}",
                            Description = $"Description for Movement {j + 1} for Workout {i + 1}",
                            WorkoutId = workoutId,
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _workoutRepository.InsertMovement(movement, userId);
                    }
                }

                return Ok(new { Message = $"{numberOfWorkouts} workouts inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while inserting workouts.", Error = ex.Message });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}
