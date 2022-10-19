using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
							var messageJson = (JObject)JsonConvert.DeserializeObject(consumer.Message.Value);

							if (messageJson != null && messageJson["EventVersion"]?.Values<string>().SingleOrDefault() == "2")
							{
								var appUser = JsonConvert.DeserializeObject<ApplicationUserProcessed>(consumer.Message.Value);
								await _userManager.AddApplicationUserAsync(appUser).ConfigureAwait(false);
							}
							else
							{
								await _userManager.AddDeadLetterAsync(consumer.Message.Value).ConfigureAwait(false);
							}
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
