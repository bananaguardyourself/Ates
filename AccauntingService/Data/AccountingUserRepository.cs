using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace AccountingService.Data
{
	public class AccountingUserRepository : BaseRepository
	{
		public AccountingUserRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertAccountingUserAsync(IEnumerable<AccountingUserEntity> users)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO accountingusers
				( PublicId, UserName, Role, Balance) VALUES 
				( @PublicId, @UserName, @Role, @Balance);", users);
		}

		public async Task<IEnumerable<AccountingUserEntity>> GetAccountingUsersAsync()
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingUserEntity>(@"SELECT * FROM public.accountingusers;");
		}

		public async Task<IEnumerable<AccountingUserEntity>> GetAccountingUsersByIdAsync(Guid publicid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingUserEntity>(@"SELECT * FROM public.accountingusers where publicid = @publicid;", publicid);
		}

		public async Task<int> UpdateAccountingUserAsync(IEnumerable<AccountingUserEntity> users)
		{
			var updateTimestamp = DateTime.Now;
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"UPDATE public.accountingusers
				set (publicid = @PublicId, username = @UserName, role = @role, balance = @balance, lastupdated = @updateTimestamp
				where publicid = @PublicId and lastupdated = @LastUpdated;", new { users, updateTimestamp });
		}

		public async Task<int> InsertDeadLetterAsync(string message, DateTime timestanmp)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO userdeadletters
				( message, timereceived) VALUES 
				( @message, @timestanmp);", new { message, timestanmp });
		}
	}
}
