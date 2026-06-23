using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _userService.GetAllUsersAsync();
                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while loading users.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);

                if (!response.Success)
                {
                    if (response.Message == "User not found.")
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
                    Message = "Server error occurred while loading user.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // POST: /api/users
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var response = await _userService.AddUserAsync(dto);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return CreatedAtAction(nameof(GetUserById), new { id = response.Data!.UserId }, response);
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Server error occurred while creating user.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        // GET: /api/users/{id}/tasks
        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetUserWithTasks(int id)
        {
            try
            {
                var response = await _userService.GetUserWithTasksAsync(id);

                if (!response.Success)
                {
                    if (response.Message == "User not found.")
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
                    Message = "Server error occurred while loading user tasks.",
                    Data = null,
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }
    }
}