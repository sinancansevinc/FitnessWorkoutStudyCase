using Dapper;
using FitnessWorkout.Data;
using FitnessWorkout.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace FitnessWorkout.Repositories
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly DatabaseConnection _connection;

        public WorkoutRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> InsertWorkout(Workout workout, int userId)
        {
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_Name", workout.Name);
                parameters.Add("@p_Description", workout.Description);
                parameters.Add("@p_DurationMinutes", workout.DurationMinutes);
                parameters.Add("@p_Difficulty", workout.Difficulty.ToString());
                parameters.Add("@p_CreatedBy", userId);

                var result = await conn.QuerySingleAsync<int>("InsertWorkout", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<Workout> GetWorkoutById(int workoutId)
        {
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_WorkoutId", workoutId);

                var workout = await conn.QuerySingleOrDefaultAsync<Workout>(
                    "SELECT * FROM Workouts WHERE Id = @p_WorkoutId", parameters);

                if (workout != null)
                {
                    workout.Movements = (await conn.QueryAsync<Movement>(
                        "SELECT * FROM Movements WHERE WorkoutId = @p_WorkoutId", parameters)).ToList();

                    workout.TargetedRegions = (await conn.QueryAsync<BodyRegion>(
                       @"SELECT br.Id, br.Name 
                          FROM BodyRegions br
                          JOIN WorkoutBodyRegions wbr ON br.Id = wbr.BodyRegionId
                          WHERE wbr.WorkoutId = @p_WorkoutId", parameters)).ToList();
                }

                return workout;
            }
        }

        public async Task InsertMovement(Movement movement, int userId)
        {
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_Name", movement.Name);
                parameters.Add("@p_Description", movement.Description);
                parameters.Add("@p_WorkoutId", movement.WorkoutId);
                parameters.Add("@p_CreatedBy", userId);

                await conn.ExecuteAsync("InsertMovement", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task InsertWorkoutBodyRegion(int workoutId, int bodyRegionId)
        {
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_WorkoutId", workoutId);
                parameters.Add("@p_BodyRegionId", bodyRegionId);

                await conn.ExecuteAsync("InsertWorkoutBodyRegion", parameters, commandType: CommandType.StoredProcedure);
            }
        }



        public async Task<IEnumerable<Workout>> GetFilteredWorkouts(int? duration, Difficulty? difficulty, string bodyRegion, bool useAndFilter)
        {
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_Duration", duration.HasValue ? (object)duration.Value : DBNull.Value);
                parameters.Add("@p_Difficulty", difficulty.HasValue ? difficulty.Value.ToString() : null);
                parameters.Add("@p_BodyRegion", !string.IsNullOrEmpty(bodyRegion) ? bodyRegion : null);
                parameters.Add("@p_UseAndFilter", useAndFilter);

                var workouts = await conn.QueryAsync<Workout>("GetFilteredWorkouts", parameters, commandType: CommandType.StoredProcedure);

                return workouts;
            }
        }
        public async Task<IEnumerable<BodyRegion>> GetBodyRegions()
        {
            using (var conn = _connection.GetConnection())
            {
                return await conn.QueryAsync<BodyRegion>("SELECT Id, Name FROM BodyRegions");
            }
        }
    }
}
