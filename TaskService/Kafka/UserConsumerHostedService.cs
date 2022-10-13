using Confluent.Kafka;
using System.Text.Json;
using TaskService.Business;
using TaskService.Models.Kafka;

namespace TaskService.Kafka
{
	public class UserConsumerHostedService : IHostedService
	{
		private readonly ApplicationUserManager _userManager;

		public UserConsumerHostedService(
			IConfiguration configuration,
			ApplicationUserManager userManager
			)
		{
			Configuration = configuration;
			_userManager = userManager;
		}

		public IConfiguration Configuration { get; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{
				GroupId = Configuration.GetSection("ApplicationUserConsumer")["GroupId"],
				BootstrapServers = Configuration.GetSection("ApplicationUserConsumer")["BootstrapServers"],
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			Task.Run(async () =>
			{
				try
				{
					using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
					consumerBuilder.Subscribe(Configuration.GetSection("ApplicationUserConsumer")["Topic"]);
					var cancelToken = new CancellationTokenSource();

					try
					{
						while (true)
						{
							var consumer = consumerBuilder.Consume(cancelToken.Token);
							var appUser = JsonSerializer.Deserialize<ApplicationUserProcessed>(consumer.Message.Value);

							_userManager.AddApplicationUserAsync(appUser).Wait();

						}
					}
					catch (OperationCanceledException)
					{
						consumerBuilder.Close();
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
			});

			return Task.CompletedTask;
		}
		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
