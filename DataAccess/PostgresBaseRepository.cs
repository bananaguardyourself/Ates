using Npgsql;
using Microsoft.Extensions.Options;
using Contracts.Settings;

namespace DataAccess
{
	public class PostgresBaseRepository
	{
		private readonly DbConnectionOptions _options;

		public PostgresBaseRepository(IOptions<DbConnectionOptions> options)
		{
			_options = options.Value;
		}

		public string DbPath
		{
			get { return _options.ConnectionString; }
		}

		public NpgsqlConnection SimpleDbConnection()
		{
			return new NpgsqlConnection(DbPath);
		}
	}
}
