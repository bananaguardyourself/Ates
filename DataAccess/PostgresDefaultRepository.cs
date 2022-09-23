using Contracts.Settings;
using Dapper;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.Extensions.Options;

namespace DataAccess
{
	public class PostgresDefaultRepository : PostgresBaseRepository, IDefaultRepository
	{
		public PostgresDefaultRepository(IOptions<DbConnectionOptions> options) : base(options)
		{			
		}

		public async Task<IEnumerable<DefaultEntity>> GetAsync()
		{
			using var cnn = SimpleDbConnection();			
			return await cnn.QueryAsync<DefaultEntity>(@"SELECT * FROM public.default;");
		}
	}
}
