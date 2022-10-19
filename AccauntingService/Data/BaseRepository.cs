using Npgsql;
using Microsoft.Extensions.Options;
using Contracts.Settings;

namespace AccountingService.Data
{
	public class BaseRepository
	{
		private readonly DbConnectionOptions _options;

		public BaseRepository(IOptions<DbConnectionOptions> options)
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
