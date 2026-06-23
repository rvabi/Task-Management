using Microsoft.Data.SqlClient;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT UserId, UserName, Email FROM Users";

            using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    UserName = reader["UserName"].ToString()!,
                    Email = reader["Email"].ToString()!
                });
            }

            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT UserId, UserName, Email FROM Users WHERE UserId = @UserId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", id);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    UserName = reader["UserName"].ToString()!,
                    Email = reader["Email"].ToString()!
                };
            }

            return null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                INSERT INTO Users (UserName, Email)
                OUTPUT INSERTED.UserId
                VALUES (@UserName, @Email)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Email", user.Email);

            await connection.OpenAsync();

            var newId = await command.ExecuteScalarAsync();

            user.UserId = Convert.ToInt32(newId);

            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(*) FROM Users WHERE UserId = @UserId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }

        public async Task<UserWithTasksDto?> GetUserWithTasksAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    u.UserId,
                    u.UserName,
                    u.Email,
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.CreatedDate
                FROM Users u
                LEFT JOIN Tasks t ON u.UserId = t.UserId
                WHERE u.UserId = @UserId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", id);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            UserWithTasksDto? userWithTasks = null;

            while (await reader.ReadAsync())
            {
                if (userWithTasks == null)
                {
                    userWithTasks = new UserWithTasksDto
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = reader["UserName"].ToString()!,
                        Email = reader["Email"].ToString()!,
                        Tasks = new List<TaskItemResponseDto>()
                    };
                }

                if (reader["TaskId"] != DBNull.Value)
                {
                    userWithTasks.Tasks.Add(new TaskItemResponseDto
                    {
                        TaskId = Convert.ToInt32(reader["TaskId"]),
                        Title = reader["Title"].ToString()!,
                        Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                        Status = reader["Status"].ToString()!,
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = reader["UserName"].ToString()!
                    });
                }
            }

            return userWithTasks;
        }
    }
}