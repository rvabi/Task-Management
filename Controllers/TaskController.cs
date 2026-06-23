using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: /api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var response = await _taskService.GetAllTasksAsync();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while loading tasks.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // GET: /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var response = await _taskService.GetTaskByIdAsync(id);

                if (!response.Success)
                {
                    if (response.Message == "Task not found.")
                    {
                        return NotFound(response);
                    }

                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while loading task.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // GET: /api/tasks/search?name=value
        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string? name)
        {
            try
            {
                var response = await _taskService.SearchTasksAsync(name ?? string.Empty);
                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while searching tasks.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // POST: /api/tasks
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskItemDto dto)
        {
            try
            {
                var response = await _taskService.AddTaskAsync(dto);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return CreatedAtAction(nameof(GetTaskById), new { id = response.Data!.TaskId }, response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while creating task.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // PUT: /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskItemDto dto)
        {
            try
            {
                var response = await _taskService.UpdateTaskAsync(id, dto);

                if (!response.Success)
                {
                    if (response.Message == "Task not found.")
                    {
                        return NotFound(response);
                    }

                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while updating task.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // PUT: /api/tasks/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
        {
            try
            {
                var response = await _taskService.ChangeStatusAsync(id, dto);

                if (!response.Success)
                {
                    if (response.Message == "Task not found.")
                    {
                        return NotFound(response);
                    }

                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while changing task status.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // DELETE: /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var response = await _taskService.DeleteTaskAsync(id);

                if (!response.Success)
                {
                    if (response.Message == "Task not found.")
                    {
                        return NotFound(response);
                    }

                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while deleting task.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }
    }
}