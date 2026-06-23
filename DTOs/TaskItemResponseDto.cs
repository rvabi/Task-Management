namespace TaskManagementAPI.DTOs
{
    public class TaskItemResponseDto
    {
        public int TaskId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;
    }
}