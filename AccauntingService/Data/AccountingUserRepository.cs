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

		public async Task<int> InsertApplicationUserAsync(IEnumerable<AccountingUserEntity> users)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO applicationusers
				( PublicId, UserName, Role) VALUES 
				( @PublicId, @UserName, @Role);", users);
		}

		public async Task<IEnumerable<AccountingUserEntity>> GetApplicationUsersAsync()
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingUserEntity>(@"SELECT * FROM public.applicationusers;");
		}

		public async Task<IEnumerable<AccountingUserEntity>> GetApplicationUsersByIdAsync(Guid publicid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AccountingUserEntity>(@"SELECT * FROM public.applicationusers where publicid = @publicid;", publicid);
		}
	}
}
