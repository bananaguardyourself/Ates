namespace AnalyticsService.Models.Kafka
{
	public class AnalyticsUserProcessed
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Role { get; set; }
	}
}
