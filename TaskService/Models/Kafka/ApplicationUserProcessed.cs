namespace TaskService.Models.Kafka
{
	public class ApplicationUserProcessed
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
	}
}
