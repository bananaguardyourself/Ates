namespace AccountingService.Data
{
	public class AccountingUserEntity
	{
		public long Id { get; set; }
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
		//In our case all costs are integers, so we dont need double or decimal etc
		public int Balance { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
