using Contracts;
using AccountingService.Data;
using System.Transactions;
using Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AccountingService.Models.Kafka;
using SchemaRegistry;

namespace AccountingService.Business
{
	public class AccountingManager
	{
		private readonly AccountingRepository _accountingRepository;
		private readonly TaskRepository _taskRepository;
		private readonly AccountingUserRepository _accountingUserRepository;
		private readonly KafkaProducer _kafkaProducer;

		public AccountingManager(
			AccountingRepository accountingRepository,
			AccountingUserRepository accountingUserRepository,
			KafkaProducer kafkaProducer,
			TaskRepository taskRepository)
		{
			_accountingRepository = accountingRepository ?? throw new ArgumentNullException(nameof(accountingRepository)); ;
			_accountingUserRepository = accountingUserRepository ?? throw new ArgumentNullException(nameof(accountingUserRepository)); ;
			_kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
			_taskRepository = taskRepository;
		}

		public async Task<AccountingTransactionEntity> ApplyAssignTransactionAsync(TaskProcessed taskpPocessed)
		{
			var task = await _taskRepository.GetTasksByPublicIdAsync(taskpPocessed.Data.PublicId);
			var transaction = new AccountingTransactionEntity(task.PublicUserId, task.PublicId, "Task Assigned", task.TaskCostAssign, 0);

			var insertedCount = await _accountingRepository.InsertTransactionsAsync(new List<AccountingTransactionEntity> { transaction });

			if (insertedCount != 1)
			{
				throw new Exception("sql insert error");
			}

			// cud messaage
			string message = JsonConvert.SerializeObject(new AccountTransactionProcessed
			{
				EventName = "TransactionCreated",
				EventId = Guid.NewGuid(),
				Producer = "AccountingService",
				EventTime = DateTime.Now,
				EventVersion = 1,
				Data = new AccountTransactionData
				{
					PublicId = transaction.PublicId,
					PublicUserId = transaction.PublicUserId,
					PublicTaskId = transaction.PublicTaskId,
					TransactionAction = transaction.TransactionAction,
					Increase = transaction.Increase,
					Decrease = transaction.Decrease,
					CreatedAt = transaction.CreatedAt
				}
			});

			var valid = JsonSchemaRegistry.Validate(JObject.Parse(message), "TransactionCreated", 1);

			if (valid)
				await _kafkaProducer.ProduceMessage("transaction-stream", message);
			else
				throw new Exception("Invalid message schema");

			return transaction;
		}

		public async Task<AccountingTransactionEntity> ApplyCloseTransactionAsync(TaskProcessed taskpPocessed)
		{
			var task = await _taskRepository.GetTasksByPublicIdAsync(taskpPocessed.Data.PublicId);
			var transaction = new AccountingTransactionEntity(task.PublicUserId, task.PublicId, "Task Closed", 0, task.TaskCostComplete);

			var insertedCount = await _accountingRepository.InsertTransactionsAsync(new List<AccountingTransactionEntity> { transaction });

			if (insertedCount != 1)
			{
				throw new Exception("sql insert error");
			}

			// cud messaage
			string message = JsonConvert.SerializeObject(new AccountTransactionProcessed
			{
				EventName = "TransactionCreated",
				EventId = Guid.NewGuid(),
				Producer = "AccountingService",
				EventTime = DateTime.Now,
				EventVersion = 1,
				Data = new AccountTransactionData
				{
					PublicId = transaction.PublicId,
					PublicUserId = transaction.PublicUserId,
					PublicTaskId = transaction.PublicTaskId,
					TransactionAction = transaction.TransactionAction,
					Increase = transaction.Increase,
					Decrease = transaction.Decrease,
					CreatedAt = transaction.CreatedAt
				}
			});

			var valid = JsonSchemaRegistry.Validate(JObject.Parse(message), "TransactionCreated", 1);

			if (valid)
				await _kafkaProducer.ProduceMessage("transaction-stream", message);
			else
				throw new Exception("Invalid message schema");

			return transaction;
		}

		public async Task CloseBillingCycleAsync()
		{
			var lastTransaction = await _accountingRepository.GetLastBillingCloseTransactionAsync();

			var transactions = new List<AccountingTransactionEntity>();

			if (lastTransaction == null)
			{
				transactions = (await _accountingRepository.GetAllTransactionAsync()).ToList();
			}
			else
			{
				transactions = (await _accountingRepository.GetTransactionCreatedAfterAsync(lastTransaction.CreatedAt)).ToList();
			}

			var users = (await _accountingUserRepository.GetAccountingUsersAsync()).Where(p => p.Role != Roles.Manager && p.Role != Roles.Admin);

			using (var transactionScope = new TransactionScope())
			{
				foreach (var user in users)
				{

				}
			}

			//todo: отправка писем

		}

		public async Task GetUserBalanceAsync()
		{

		}

		public async Task GetManagementBalanceAsync()
		{

		}
	}
}
