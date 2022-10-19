namespace TaskService.Models.Kafka
{
	public class TaskBusinessEvent
	{
		public Guid EventId { get; set; }
		public int EventVersion { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
		public string Producer { get; set; }
		public TaskBusinessEventData Data { get; set; }
	}

	public class TaskBusinessEventData
	{
		public Guid PublicId { get; set; }
		public Guid UserId { get; set; }
	}
}
