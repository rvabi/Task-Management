using Microsoft.Data.SqlClient;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<TaskItemResponseDto>> GetAllTasksAsync()
        {
            var tasks = new List<TaskItemResponseDto>();

            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.CreatedDate,
                    t.UserId,
                    u.UserName
                FROM Tasks t
                INNER JOIN Users u ON t.UserId = u.UserId
                ORDER BY t.TaskId DESC";

            using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tasks.Add(new TaskItemResponseDto
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

            return tasks;
        }

        public async Task<TaskItemResponseDto?> GetTaskByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.CreatedDate,
                    t.UserId,
                    u.UserName
                FROM Tasks t
                INNER JOIN Users u ON t.UserId = u.UserId
                WHERE t.TaskId = @TaskId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TaskId", id);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new TaskItemResponseDto
                {
                    TaskId = Convert.ToInt32(reader["TaskId"]),
                    Title = reader["Title"].ToString()!,
                    Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                    Status = reader["Status"].ToString()!,
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    UserName = reader["UserName"].ToString()!
                };
            }

            return null;
        }

        public async Task<List<TaskItemResponseDto>> SearchTasksAsync(string name)
        {
            var tasks = new List<TaskItemResponseDto>();

            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.CreatedDate,
                    t.UserId,
                    u.UserName
                FROM Tasks t
                INNER JOIN Users u ON t.UserId = u.UserId
                WHERE t.Title LIKE @Name
                ORDER BY t.TaskId DESC";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tasks.Add(new TaskItemResponseDto
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

            return tasks;
        }

        public async Task<TaskItemResponseDto> AddTaskAsync(TaskItem task)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                INSERT INTO Tasks (Title, Description, Status, UserId)
                OUTPUT INSERTED.TaskId
                VALUES (@Title, @Description, @Status, @UserId)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", task.Status);
            command.Parameters.AddWithValue("@UserId", task.UserId);

            await connection.OpenAsync();

            var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

            var createdTask = await GetTaskByIdAsync(newId);

            return createdTask!;
        }

        public async Task<bool> UpdateTaskAsync(int id, TaskItem task)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                UPDATE Tasks
                SET 
                    Title = @Title,
                    Description = @Description,
                    Status = @Status,
                    UserId = @UserId
                WHERE TaskId = @TaskId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TaskId", id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", task.Status);
            command.Parameters.AddWithValue("@UserId", task.UserId);

            await connection.OpenAsync();

            var rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> ChangeStatusAsync(int id, string status)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                UPDATE Tasks
                SET Status = @Status
                WHERE TaskId = @TaskId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TaskId", id);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();

            var rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "DELETE FROM Tasks WHERE TaskId = @TaskId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TaskId", id);

            await connection.OpenAsync();

            var rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> TaskExistsAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = "SELECT COUNT(*) FROM Tasks WHERE TaskId = @TaskId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TaskId", id);

            await connection.OpenAsync();

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }
    }
}