using AccountingService.Business;
using AccountingService.Models.Kafka;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AccountingService.Kafka
{
	public class TaskConsumerHostedService : IHostedService
	{
		private readonly AccountingTaskManager _taskManager;

		public TaskConsumerHostedService(
			IConfiguration configuration,
			AccountingTaskManager taskManager
			)
		{
			Configuration = configuration;
			_taskManager = taskManager;
		}

		public IConfiguration Configuration { get; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{
				GroupId = Configuration.GetSection("AccountingTaskCUDConsumer")["GroupId"],
				BootstrapServers = Configuration.GetSection("AccountingTaskCUDConsumer")["BootstrapServers"],
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

			Task.Run(async () =>
			{
				try
				{
					using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
					consumerBuilder.Subscribe(Configuration.GetSection("AccountingTaskCUDConsumer")["Topic"]);
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
										await _taskManager.CreateTaskAsync(task).ConfigureAwait(false);
										break;
									case "TaskUpdated":
										await _taskManager.UpdateTaskAsync(task).ConfigureAwait(false);
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
