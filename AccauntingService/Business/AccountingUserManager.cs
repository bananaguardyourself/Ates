using AccountingService.Data;
using AccountingService.Models.Kafka;

namespace AccountingService.Business
{
	public class AccountingUserManager
	{
		private readonly AccountingUserRepository _userRepository;

		public AccountingUserManager(
			AccountingUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task AddAccountingUserAsync(AccountingUserProcessed user)
		{
			var userEntity = new AccountingUserEntity
			{
				PublicId = user.PublicId,
				Role = user.Role,
				UserName = user.UserName
			};

			try
			{
				await _userRepository.InsertApplicationUserAsync(new List<AccountingUserEntity> { userEntity });
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public async Task<AccountingUserEntity> GetUsersByPublicId(Guid publicId)
		{
			return (await _userRepository.GetApplicationUsersByIdAsync(publicId)).Single();			
		}
	}
}
