namespace AtesIdentityServer.Models
{
	public class ApplicationUserStream
	{
		public Guid PublicId { get; set; }
		public string UserName { get; set; }		
		public string  Email { get; set; }
		public string Role { get; set; }
	}
}
