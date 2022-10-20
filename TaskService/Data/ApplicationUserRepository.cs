using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace TaskService.Data
{
	public class ApplicationUserRepository : BaseRepository
	{
		public ApplicationUserRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertApplicationUserAsync(IEnumerable<ApplicationUserEntity> users)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO applicationusers
				( PublicId, UserName, Role) VALUES 
				( @PublicId, @UserName, @Role);", users);
		}

		public async Task<IEnumerable<ApplicationUserEntity>> GetApplicationUsersAsync()
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<ApplicationUserEntity>(@"SELECT * FROM public.applicationusers;");
		}

		public async Task<IEnumerable<ApplicationUserEntity>> GetApplicationUsersByIdAsync(Guid publicid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<ApplicationUserEntity>(@"SELECT * FROM public.applicationusers where publicid = @publicid;", publicid);
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
