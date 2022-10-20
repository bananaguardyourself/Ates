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
				PublicId = user.Data.PublicId,
				Role = user.Data.Role,
				UserName = user.Data.UserName,
				Balance = 0,
				LastUpdated = DateTime.Now
			};

			try
			{
				await _userRepository.InsertAccountingUserAsync(new List<AccountingUserEntity> { userEntity });
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public async Task AddDeadLetterAsync(string message)
		{
			try
			{
				await _userRepository.InsertDeadLetterAsync(message, DateTime.Now);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public async Task<AccountingUserEntity> GetUserByPublicId(Guid publicId)
		{
			return (await _userRepository.GetAccountingUsersByIdAsync(publicId)).Single();
		}


	}
}
