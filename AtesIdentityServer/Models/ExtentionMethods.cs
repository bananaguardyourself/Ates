namespace AtesIdentityServer.Models
{
	public static class ModelsExtentions
	{
		public static ApplicationUserStream ToStream(this ApplicationUser user)
		{
			return new ApplicationUserStream
			{
				PublicId = user.PublicId,
				UserName = user.UserName,
				Email = user.Email,
				Role = user.Role
			};			
		}
	}
}
