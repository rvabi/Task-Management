using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int id);

        Task<User> AddUserAsync(User user);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> UserExistsAsync(int userId);

        Task<UserWithTasksDto?> GetUserWithTasksAsync(int id);
    }
}