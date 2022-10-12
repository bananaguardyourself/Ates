using IdentityServer4.Models;

namespace AtesIdentityServer.IdentityConfiguration
{
	public class Scopes
	{
		public static IEnumerable<ApiScope> GetApiScopes()
		{
			return new[]
			{
				new ApiScope("atesscope", "Ates scope"),
			};
		}
	}
}
