using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<ApiResponse<List<TaskItemResponseDto>>> GetAllTasksAsync();

        Task<ApiResponse<TaskItemResponseDto>> GetTaskByIdAsync(int id);

        Task<ApiResponse<List<TaskItemResponseDto>>> SearchTasksAsync(string name);

        Task<ApiResponse<TaskItemResponseDto>> AddTaskAsync(CreateTaskItemDto dto);

        Task<ApiResponse<TaskItemResponseDto>> UpdateTaskAsync(int id, UpdateTaskItemDto dto);

        Task<ApiResponse<string>> ChangeStatusAsync(int id, ChangeStatusDto dto);

        Task<ApiResponse<string>> DeleteTaskAsync(int id);
    }
}