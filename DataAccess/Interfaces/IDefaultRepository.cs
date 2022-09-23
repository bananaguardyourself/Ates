using DataAccess.Entities;

namespace DataAccess.Interfaces
{
	public interface IDefaultRepository
	{
		Task<IEnumerable<DefaultEntity>> GetAsync();
	}
}
