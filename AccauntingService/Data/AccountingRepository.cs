using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace AccountingService.Data
{
	public class AccountingRepository : BaseRepository
	{
		public AccountingRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertTransactionsAsync(IEnumerable<AccountingTransactionEntity> accountingTransactions)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO public.accountingtransactions
				( publicid, publicuserid, publictaskid, action, increase, decrease, createdat) VALUES 
				( @Publicid, @PublicUserId, @PublicTaskId, @Action, @Increase, @Decrease, @CreatedAt);", accountingTransactions);
		}

		public async Task<AccountingTransactionEntity?> GetLastBillingCloseTransactionAsync()
		{
			using var cnn = SimpleDbConnection();
			return (await cnn.QueryAsync<AccountingTransactionEntity>(@"SELECT * FROM public.accountingtransactions
				where action = 'Close Billing Cycle'
				order by createdat desc
				limit 1;")).SingleOrDefault();
		}

		public async Task<IEnumerable<AccountingTransactionEntity>> GetAllTransactionAsync()
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingTransactionEntity>(@"SELECT * FROM public.accountingtransactions");
		}

		public async Task<IEnumerable<AccountingTransactionEntity>> GetTransactionCreatedAfterAsync(DateTime createdAt)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingTransactionEntity>(@"SELECT * FROM public.accountingtransactions where createdat > @createdAt'", createdAt);
		}
	}
}
