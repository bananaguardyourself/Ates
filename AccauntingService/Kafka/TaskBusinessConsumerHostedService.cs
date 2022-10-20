using AccountingService.Business;
using AccountingService.Models.Kafka;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AccountingService.Kafka
{
	public class TaskBusinessConsumerHostedService : IHostedService
	{
		private readonly AccountingManager _manager;
		private readonly AccountingTaskManager _taskManager;		

		public TaskBusinessConsumerHostedService(
			IConfiguration configuration,
			AccountingManager manager,
			AccountingTaskManager taskManager
			)
		{
			Configuration = configuration;			
			_manager = manager;
			_taskManager = taskManager;
		}

		public IConfiguration Configuration { get; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{
				GroupId = Configuration.GetSection("AccountingTaskBEConsumer")["GroupId"],
				BootstrapServers = Configuration.GetSection("AccountingTaskBEConsumer")["BootstrapServers"],
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			Task.Run(async () =>
			{
				try
				{
					using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
					consumerBuilder.Subscribe(Configuration.GetSection("AccountingTaskBEConsumer")["Topic"]);
					var cancelToken = new CancellationTokenSource();

					try
					{
						while (true)
						{
							var consumer = consumerBuilder.Consume(cancelToken.Token);
							var messageJson = (JObject)JsonConvert.DeserializeObject(consumer.Message.Value);

							if (messageJson != null && messageJson["EventVersion"]?.Values<string>().SingleOrDefault() == "2")
							{
								var task = JsonConvert.DeserializeObject<TaskProcessed>(consumer.Message.Value);

								switch (task.EventName)
								{
									case "TaskCreated":
										await _manager.ApplyAssignTransactionAsync(task).ConfigureAwait(false);
										break;
									case "TaskClosed":
										await _manager.ApplyCloseTransactionAsync(task).ConfigureAwait(false);
										break;
									case "TaskAssigned":
										await _manager.ApplyAssignTransactionAsync(task).ConfigureAwait(false);
										break;
									default:
										await _taskManager.AddDeadLetterAsync(consumer.Message.Value).ConfigureAwait(false);
										break;
								}								
							}
							else
							{
								await _taskManager.AddDeadLetterAsync(consumer.Message.Value).ConfigureAwait(false);
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
