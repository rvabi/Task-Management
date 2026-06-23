using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItemResponseDto>> GetAllTasksAsync();

        Task<TaskItemResponseDto?> GetTaskByIdAsync(int id);

        Task<List<TaskItemResponseDto>> SearchTasksAsync(string name);

        Task<TaskItemResponseDto> AddTaskAsync(TaskItem task);

        Task<bool> UpdateTaskAsync(int id, TaskItem task);

        Task<bool> ChangeStatusAsync(int id, string status);

        Task<bool> DeleteTaskAsync(int id);

        Task<bool> TaskExistsAsync(int id);
    }
}