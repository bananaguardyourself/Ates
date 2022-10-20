namespace AccountingService.Models.Kafka
{
	public class AccountTransactionProcessed
	{
		public Guid EventId { get; set; }
		public int EventVersion { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
		public string Producer { get; set; }
		public AccountTransactionData Data { get; set; }
	}

	public class AccountTransactionData
	{
		public Guid PublicId { get; set; } = new Guid();
		public Guid PublicUserId { get; set; }
		public Guid? PublicTaskId { get; set; }
		public string TransactionAction { get; set; }
		public int Increase { get; set; }
		public int Decrease { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
