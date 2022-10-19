using Contracts;
using AccountingService.Data;
using System.Transactions;
using Kafka;
using System.Text.Json;

namespace AccountingService.Business
{
	public class AccountingManager
	{
		private readonly AccountingRepository _accountingRepository;
		private readonly AccountingUserRepository _accountingUserRepository;
		private readonly KafkaProducer _kafkaProducer;

		public AccountingManager(
			AccountingRepository accountingRepository,
			AccountingUserRepository accountingUserRepository,
			KafkaProducer kafkaProducer)
		{
			_accountingRepository = accountingRepository ?? throw new ArgumentNullException(nameof(accountingRepository)); ;
			_accountingUserRepository = accountingUserRepository ?? throw new ArgumentNullException(nameof(accountingUserRepository)); ;
			_kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
		}
	}
}
