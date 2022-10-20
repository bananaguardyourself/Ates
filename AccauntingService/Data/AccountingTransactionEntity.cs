namespace AccountingService.Data
{
	public class AccountingTransactionEntity
	{
		public AccountingTransactionEntity(Guid publicUserId, Guid? publicTaskId, string transactionAction, int increase, int decrease)
		{
			PublicUserId = publicUserId;
			PublicTaskId = publicTaskId;
			TransactionAction = transactionAction;
			Increase = increase;
			Decrease = decrease;
			CreatedAt = DateTime.UtcNow;
		}

		public int Id { get; set; }
		public Guid PublicId { get; set; } = new Guid();
		public Guid PublicUserId { get; set; }
		public Guid? PublicTaskId { get; set; }
		public string TransactionAction { get; set; }
		public int Increase { get; set; }
		public int Decrease { get; set; }
		public DateTime CreatedAt { get; set; }

	}
}
