using TaskService.Data;
using TaskService.Models.Kafka;

namespace TaskService.Business
{
	public class ApplicationUserManager
	{
		private readonly ApplicationUserRepository _userRepository;

		public ApplicationUserManager(
			ApplicationUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task AddApplicationUserAsync(ApplicationUserProcessed user)
		{
			var userEntity = new ApplicationUserEntity
			{
				PublicId = user.Data.PublicId,
				Role = user.Data.Role,
				UserName = user.Data.UserName
			};

			try
			{
				await _userRepository.InsertApplicationUserAsync(new List<ApplicationUserEntity> { userEntity });
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

		public async Task<ApplicationUserEntity> GetUserByPublicId(Guid publicId)
		{
			return (await _userRepository.GetApplicationUsersByIdAsync(publicId)).Single();			
		}
	}
}
