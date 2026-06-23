namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Todo";

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
    }
}