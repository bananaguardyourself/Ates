using IdentityServer4.Models;

namespace AtesIdentityServer.IdentityConfiguration
{
	public class Resources
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new[]
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResources.Email(),
				new IdentityResource
				{
					Name = "role",
					UserClaims = new List<string> {"role"}
				}
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new[]
			{
				new ApiResource
				{
					Name = "tasktracker",
					DisplayName = "Task Tracker",
					Description = "Allow the application to access Task Tracker on your behalf",
					Scopes = new List<string> { "atesscope"},
					ApiSecrets = new List<Secret> {new Secret("secret".Sha256())},
					UserClaims = new List<string> {"role"}					
				}
			};
		}
	}
}
