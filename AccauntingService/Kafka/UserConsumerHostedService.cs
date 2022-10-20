﻿using AccountingService.Business;
using AccountingService.Models.Kafka;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
							var messageJson = (JObject)JsonConvert.DeserializeObject(consumer.Message.Value);

							if (messageJson != null && messageJson["EventVersion"]?.Values<string>().SingleOrDefault() == "2")
							{
								var accUser = JsonConvert.DeserializeObject<AccountingUserProcessed>(consumer.Message.Value);
								await _userManager.AddAccountingUserAsync(accUser).ConfigureAwait(false);
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
