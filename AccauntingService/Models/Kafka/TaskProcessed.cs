using Contracts;

namespace AccountingService.Models.Kafka
{
	public class TaskProcessed
	{
		public string EventName { get; set; }
		public Guid PublicId { get; set; }
		public Guid UserId { get; set; }
		public string TaskName { get; set; }
		public string TaskDescription { get; set; }
		public TaskStatusType TaskStatus { get; set; }
	}
}
