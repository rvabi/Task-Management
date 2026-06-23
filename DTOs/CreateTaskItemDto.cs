namespace TaskManagementAPI.DTOs
{
    public class CreateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Todo";

        public int UserId { get; set; }
    }
}