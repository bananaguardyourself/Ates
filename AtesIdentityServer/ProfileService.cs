using AtesIdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AtesIdentityServer
{
	public class ProfileService : IProfileService
	{
		protected UserManager<ApplicationUser> _userManager;

		public ProfileService(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var user = await _userManager.GetUserAsync(context.Subject);

			var claims = new List<Claim>
			{
				new Claim("PublicId", user.PublicId.ToString()),
			};

			context.IssuedClaims.AddRange(claims);
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			//>Processing
			var user = await _userManager.GetUserAsync(context.Subject);

			context.IsActive = user != null;
		}
	}
}
