using Contracts;

namespace TaskService.Models.Kafka
{
	public class TaskProcessed
	{
		public Guid EventId { get; set; }
		public int EventVersion { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
		public string Producer { get; set; }
		public TaskProcessedData Data { get; set; }
	}

	public class TaskProcessedData
	{
		public Guid PublicId { get; set; }
		public Guid PublicUserId { get; set; }
		public string TaskTitle { get; set; }
		public string TaskJiraId { get; set; }
		public string TaskDescription { get; set; }
		public TaskStatusType TaskStatus { get; set; }
	}
}
