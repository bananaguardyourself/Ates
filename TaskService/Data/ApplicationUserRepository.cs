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
	}
}
