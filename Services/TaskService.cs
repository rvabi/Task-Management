using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;

namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        private readonly List<string> _validStatuses = new List<string>
        {
            "Todo",
            "In Progress",
            "Done"
        };

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<List<TaskItemResponseDto>>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();

            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = true,
                Message = "Tasks loaded successfully.",
                Data = tasks,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<TaskItemResponseDto>> GetTaskByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = new List<string> { "TaskId must be a positive number." }
                };
            }

            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Task not found.",
                    Data = null,
                    Errors = new List<string> { "No task exists with the given id." }
                };
            }

            return new ApiResponse<TaskItemResponseDto>
            {
                Success = true,
                Message = "Task loaded successfully.",
                Data = task,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<List<TaskItemResponseDto>>> SearchTasksAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var allTasks = await _taskRepository.GetAllTasksAsync();

                return new ApiResponse<List<TaskItemResponseDto>>
                {
                    Success = true,
                    Message = "All tasks loaded successfully.",
                    Data = allTasks,
                    Errors = new List<string>()
                };
            }

            var tasks = await _taskRepository.SearchTasksAsync(name.Trim());

            return new ApiResponse<List<TaskItemResponseDto>>
            {
                Success = true,
                Message = "Search completed successfully.",
                Data = tasks,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<TaskItemResponseDto>> AddTaskAsync(CreateTaskItemDto dto)
        {
            var errors = await ValidateTaskData(dto.Title, dto.Description, dto.Status, dto.UserId);

            if (errors.Any())
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                };
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                Status = dto.Status.Trim(),
                UserId = dto.UserId
            };

            var createdTask = await _taskRepository.AddTaskAsync(task);

            return new ApiResponse<TaskItemResponseDto>
            {
                Success = true,
                Message = "Task created successfully.",
                Data = createdTask,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<TaskItemResponseDto>> UpdateTaskAsync(int id, UpdateTaskItemDto dto)
        {
            if (id <= 0)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = new List<string> { "TaskId must be a positive number." }
                };
            }

            var taskExists = await _taskRepository.TaskExistsAsync(id);

            if (!taskExists)
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Task not found.",
                    Data = null,
                    Errors = new List<string> { "No task exists with the given id." }
                };
            }

            var errors = await ValidateTaskData(dto.Title, dto.Description, dto.Status, dto.UserId);

            if (errors.Any())
            {
                return new ApiResponse<TaskItemResponseDto>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                };
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                Status = dto.Status.Trim(),
                UserId = dto.UserId
            };

            await _taskRepository.UpdateTaskAsync(id, task);

            var updatedTask = await _taskRepository.GetTaskByIdAsync(id);

            return new ApiResponse<TaskItemResponseDto>
            {
                Success = true,
                Message = "Task updated successfully.",
                Data = updatedTask,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<string>> ChangeStatusAsync(int id, ChangeStatusDto dto)
        {
            var errors = new List<string>();

            if (id <= 0)
            {
                errors.Add("TaskId must be a positive number.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                errors.Add("Status is required.");
            }
            else if (!_validStatuses.Contains(dto.Status.Trim()))
            {
                errors.Add("Status must be Todo, In Progress, or Done.");
            }

            if (errors.Any())
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                };
            }

            var taskExists = await _taskRepository.TaskExistsAsync(id);

            if (!taskExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Task not found.",
                    Data = null,
                    Errors = new List<string> { "No task exists with the given id." }
                };
            }

            await _taskRepository.ChangeStatusAsync(id, dto.Status.Trim());

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Task status updated successfully.",
                Data = "Status changed to " + dto.Status.Trim(),
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<string>> DeleteTaskAsync(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = new List<string> { "TaskId must be a positive number." }
                };
            }

            var taskExists = await _taskRepository.TaskExistsAsync(id);

            if (!taskExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Task not found.",
                    Data = null,
                    Errors = new List<string> { "No task exists with the given id." }
                };
            }

            await _taskRepository.DeleteTaskAsync(id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Task deleted successfully.",
                Data = "Deleted",
                Errors = new List<string>()
            };
        }

        private async Task<List<string>> ValidateTaskData(string title, string? description, string status, int userId)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(title))
            {
                errors.Add("Title is required.");
            }
            else if (title.Length > 200)
            {
                errors.Add("Title cannot exceed 200 characters.");
            }

            if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            {
                errors.Add("Description cannot exceed 500 characters.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                errors.Add("Status is required.");
            }
            else if (!_validStatuses.Contains(status.Trim()))
            {
                errors.Add("Status must be Todo, In Progress, or Done.");
            }

            if (userId <= 0)
            {
                errors.Add("UserId is required.");
            }
            else
            {
                var userExists = await _userRepository.UserExistsAsync(userId);

                if (!userExists)
                {
                    errors.Add("Selected user does not exist.");
                }
            }

            return errors;
        }
    }
} 