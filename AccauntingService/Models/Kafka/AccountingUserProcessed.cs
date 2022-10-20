namespace AccountingService.Models.Kafka
{
	public class AccountingUserProcessed
	{
		public Guid EventId { get; set; }
		public int EventVersion { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
		public string Producer { get; set; }
		public AccountingUserData Data { get; set; }
	}

	public class AccountingUserData
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
	}
}
