using AccountingService.Business;

namespace AccountingService.Kafka
{
	public class CloseBillingCycleHostedService : IHostedService
	{
		private readonly AccountingManager _manager;

		public CloseBillingCycleHostedService(
			IConfiguration configuration,
			AccountingManager manager
			)
		{
			Configuration = configuration;
			_manager = manager;
		}

		public IConfiguration Configuration { get; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Task.Run(async () =>
			{
				try
				{
					var closedToday = false;
					while (true)
					{
						Thread.Sleep(60000);

						// примитивное условие проверки конца дня
						if (DateTime.Now.Hour >= 19 && !closedToday)
						{
							closedToday = true;
							await _manager.CloseBillingCycleAsync();
						}
						{
							closedToday = false;
						}

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
