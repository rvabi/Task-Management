using System.Text.RegularExpressions;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;

namespace TaskManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return new ApiResponse<List<User>>
            {
                Success = true,
                Message = "Users loaded successfully.",
                Data = users,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = new List<string> { "UserId must be a positive number." }
                };
            }

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                    Errors = new List<string> { "No user exists with the given id." }
                };
            }

            return new ApiResponse<User>
            {
                Success = true,
                Message = "User loaded successfully.",
                Data = user,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<User>> AddUserAsync(CreateUserDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                errors.Add("UserName is required.");
            }
            else if (dto.UserName.Length > 100)
            {
                errors.Add("UserName cannot exceed 100 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                errors.Add("Email is required.");
            }
            else if (dto.Email.Length > 100)
            {
                errors.Add("Email cannot exceed 100 characters.");
            }
            else if (!IsValidEmail(dto.Email))
            {
                errors.Add("Valid email is required.");
            }
            else if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                errors.Add("Email already exists.");
            }

            if (errors.Any())
            {
                return new ApiResponse<User>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = errors
                };
            }

            var user = new User
            {
                UserName = dto.UserName.Trim(),
                Email = dto.Email.Trim()
            };

            var createdUser = await _userRepository.AddUserAsync(user);

            return new ApiResponse<User>
            {
                Success = true,
                Message = "User created successfully.",
                Data = createdUser,
                Errors = new List<string>()
            };
        }

        public async Task<ApiResponse<UserWithTasksDto>> GetUserWithTasksAsync(int id)
        {
            if (id <= 0)
            {
                return new ApiResponse<UserWithTasksDto>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Errors = new List<string> { "UserId must be a positive number." }
                };
            }

            var userWithTasks = await _userRepository.GetUserWithTasksAsync(id);

            if (userWithTasks == null)
            {
                return new ApiResponse<UserWithTasksDto>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                    Errors = new List<string> { "No user exists with the given id." }
                };
            }

            return new ApiResponse<UserWithTasksDto>
            {
                Success = true,
                Message = "User with tasks loaded successfully.",
                Data = userWithTasks,
                Errors = new List<string>()
            };
        }

        private bool IsValidEmail(string email)
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
    }
}