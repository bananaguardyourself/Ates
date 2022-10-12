using IdentityServer4.Models;
using IdentityServer4;

namespace AtesIdentityServer.IdentityConfiguration
{
	public class Clients
	{
		public static IEnumerable<Client> Get()
		{
			return new List<Client>
			{
				new Client
				{
					ClientId = "tasktracker",
					ClientName = "Task Tracker",
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					ClientSecrets = new List<Secret> {new Secret("secret".Sha256())},
					AllowedScopes = new List<string> { "atesscope" },
					AllowAccessTokensViaBrowser = true,
				},
				new Client
				{
					ClientName = "Postman",
					AllowOfflineAccess = true,
					AllowedScopes = new[]
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"atesscope"
					},
					RedirectUris = new[]
					{
						"https://www.getpostman.com/oauth2/callback"
					},
					Enabled = true,
					ClientId = "postman",
					ClientSecrets = new []
					{
						new Secret("secret".Sha256())
					},
					PostLogoutRedirectUris = new []
					{
						"http://localhost:5001/signout-callback-oidc"
					},
					ClientUri = null,
					AllowedGrantTypes = new []
					{
						GrantType.ResourceOwnerPassword
					}
				}
			};
		}
	}
}
