using AccountingService.Business;
using AccountingService.Models.Kafka;
using Confluent.Kafka;
using System.Text.Json;

namespace AccountingService.Kafka
{
	public class UserConsumerHostedService : IHostedService
	{
		private readonly AccountingUserManager _userManager;

		public UserConsumerHostedService(
			IConfiguration configuration,
			AccountingUserManager userManager
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
				GroupId = Configuration.GetSection("AccountingUserConsumer")["GroupId"],
				BootstrapServers = Configuration.GetSection("AccountingUserConsumer")["BootstrapServers"],
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			Task.Run(async () =>
			{
				try
				{
					using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
					consumerBuilder.Subscribe(Configuration.GetSection("AccountingUserConsumer")["Topic"]);
					var cancelToken = new CancellationTokenSource();

					try
					{
						while (true)
						{
							var consumer = consumerBuilder.Consume(cancelToken.Token);
							var appUser = JsonSerializer.Deserialize<AccountingUserProcessed>(consumer.Message.Value);

							_userManager.AddAccountingUserAsync(appUser).Wait();

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
