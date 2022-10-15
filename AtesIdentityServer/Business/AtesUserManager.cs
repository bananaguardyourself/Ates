using AtesIdentityServer.Models;
using Kafka;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

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

			string message = JsonSerializer.Serialize(user.ToStream());

			await _kafkaProducer.ProduceMessage("users-stream", message);
		}
	}
}
