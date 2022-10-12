using AtesIdentityServer.Models;
using Kafka;
using Microsoft.AspNetCore.Identity;

namespace AtesIdentityServer.Business
{
	public class AtesUserManager
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly KafkaProducer _kafkaProducer;

		public AtesUserManager(
			UserManager<ApplicationUser> userManager,
			KafkaProducer kafkaProducer)
		{
			_userManager = userManager;
			_kafkaProducer = kafkaProducer;
		}

		public async Task RegisterUser(string username, string email, string role, string password)
		{
			var user = new ApplicationUser
			{
				UserName = username,
				Email = email,
				Role = role,
				EmailConfirmed = true,
			};

			var result = await _userManager.CreateAsync(user, password);

			if (!result.Succeeded)
			{
				throw new Exception(string.Join(",", result.Errors.Select(e=>e.Description).ToList()));
			}

			await _kafkaProducer.ProduceMessage("testtopic", "test message");
		}
	}
}
