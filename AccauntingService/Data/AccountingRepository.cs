using Contracts;
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
	}
}
