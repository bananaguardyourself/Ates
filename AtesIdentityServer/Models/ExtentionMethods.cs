namespace AtesIdentityServer.Models
{
	public static class ModelsExtentions
	{
		public static ApplicationUserStream ToStream(this ApplicationUser user, string eventName)
		{
			return new ApplicationUserStream
			{
				EventId = Guid.NewGuid(),
				EventTime = DateTime.Now,
				EventVersion = 2,
				Producer = "AtesIdentityServer",
				EventName = eventName,
				Data = new ApplicationUserData()
				{
					PublicId = user.PublicId,
					UserName = user.UserName,
					Email = user.Email,
					Role = user.Role
				}
			};			
		}
	}
}
