using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace AnalyticsService.Data
{
	public class AnalyticsUserRepository : BaseRepository
	{
		public AnalyticsUserRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertApplicationUserAsync(IEnumerable<AnalyticsUserEntity> users)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO applicationusers
				( PublicId, UserName, Role) VALUES 
				( @PublicId, @UserName, @Role);", users);
		}

		public async Task<IEnumerable<AnalyticsUserEntity>> GetApplicationUsersAsync()
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AnalyticsUserEntity>(@"SELECT * FROM public.applicationusers;");
		}

		public async Task<IEnumerable<AnalyticsUserEntity>> GetApplicationUsersByIdAsync(Guid publicid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<AnalyticsUserEntity>(@"SELECT * FROM public.applicationusers where publicid = @publicid;", publicid);
		}
	}
}
