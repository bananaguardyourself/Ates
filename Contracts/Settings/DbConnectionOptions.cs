using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Settings
{
	public class DbConnectionOptions
	{
		public const string DbConnection = "DbConnection";

		public string ConnectionString { get; set; } = string.Empty;		
	}
}
