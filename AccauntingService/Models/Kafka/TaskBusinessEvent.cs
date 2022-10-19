namespace AccountingService.Models.Kafka
{
	public class TaskBusinessEvent
	{
		public string EventName { get; set; }
		public Guid PublicId { get; set; }
		public Guid UserId { get; set; }
	}
}
