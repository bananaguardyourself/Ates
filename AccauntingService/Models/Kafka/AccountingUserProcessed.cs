namespace AccountingService.Models.Kafka
{
	public class AccountingUserProcessed
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
	}
}
