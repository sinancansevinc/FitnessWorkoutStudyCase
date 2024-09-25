
using Dapper;
using FitnessWorkout.Data;
using FitnessWorkout.DTOS;
using FitnessWorkout.Models;
using System.Security.Cryptography;
using System.Text;

namespace FitnessWorkout.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnection _connection;

        public UserRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }

        private string GenerateSalt()
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public async Task<User> RegisterUser(UserDTO userDto)
        {
            var salt = GenerateSalt();
            var passwordHash = HashPassword(userDto.Password, salt);
            using (var conn = _connection.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Username", userDto.Username);
                parameters.Add("@PasswordHash", passwordHash);
                parameters.Add("@Salt", salt);

                await conn.ExecuteAsync("INSERT INTO Users (Username, PasswordHash, Salt) VALUES (@Username, @PasswordHash, @Salt)", parameters);
                return await GetUserByUsername(userDto.Username);
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            using (var conn = _connection.GetConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (var hmac = new HMACSHA256(Convert.FromBase64String(salt)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
