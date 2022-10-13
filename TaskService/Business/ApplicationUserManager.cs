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
				PublicId = user.PublicId,
				Role = user.Role,
				UserName = user.UserName
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

		public async Task<ApplicationUserEntity> GetUsersByPublicId(Guid publicId)
		{
			return (await _userRepository.GetApplicationUsersByIdAsync(publicId)).Single();			
		}
	}
}
