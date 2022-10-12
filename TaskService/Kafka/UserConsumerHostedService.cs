using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace TaskService.Kafka
{
	public class UserConsumerHostedService : IHostedService
	{
		public UserConsumerHostedService(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{
				GroupId =  Configuration.GetSection("Kafka")["GroupId"],
				BootstrapServers = Configuration.GetSection("Kafka")["BootstrapServers"],
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			try
			{
				using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
				{
					consumerBuilder.Subscribe(Configuration.GetSection("Kafka")["Topic"]);
					var cancelToken = new CancellationTokenSource();

					try
					{
						while (true)
						{
							var consumer = consumerBuilder.Consume(cancelToken.Token);

							//var orderRequest = JsonSerializer.Deserialize<OrderProcessingRequest>(consumer.Message.Value);
							//Debug.WriteLine($"Processing Order Id:	{orderRequest.OrderId}");
						}
					}
					catch (OperationCanceledException)
					{
						consumerBuilder.Close();
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			return Task.CompletedTask;
		}
		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
