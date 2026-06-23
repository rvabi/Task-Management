using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface IUserService
    {
        Task<ApiResponse<List<User>>> GetAllUsersAsync();

        Task<ApiResponse<User>> GetUserByIdAsync(int id);

        Task<ApiResponse<User>> AddUserAsync(CreateUserDto dto);

        Task<ApiResponse<UserWithTasksDto>> GetUserWithTasksAsync(int id);
    }
}