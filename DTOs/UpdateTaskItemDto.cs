namespace TaskManagementAPI.DTOs
{
    public class UpdateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Todo";

        public int UserId { get; set; }
    }
}