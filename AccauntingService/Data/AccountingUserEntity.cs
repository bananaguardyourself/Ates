namespace AccountingService.Data
{
	public class AccountingUserEntity
	{
		public long Id { get; set; }
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
	}
}
