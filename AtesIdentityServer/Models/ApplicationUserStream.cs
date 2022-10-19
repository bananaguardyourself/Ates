using static IdentityServer4.Models.IdentityResources;

namespace AtesIdentityServer.Models
{
	public class ApplicationUserStream
	{
		public Guid EventId { get; set; }
		public int EventVersion { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
		public string Producer { get; set; }
		public ApplicationUserData Data { get; set; }
	}

	public class ApplicationUserData
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
	}
}
