using FitnessWorkout.DTOS;
using FitnessWorkout.Models;

namespace FitnessWorkout.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsername(string username);
        Task<User> RegisterUser(UserDTO userDto);
    }
}
