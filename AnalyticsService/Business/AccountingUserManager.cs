using AnalyticsService.Data;
using AnalyticsService.Models.Kafka;

namespace AnalyticsService.Business
{
	public class AnalyticsUserManager
	{
		private readonly AnalyticsUserRepository _userRepository;

		public AnalyticsUserManager(
			AnalyticsUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task AddAccountingUserAsync(AnalyticsUserProcessed user)
		{
			var userEntity = new AnalyticsUserEntity
			{
				PublicId = user.PublicId,
				Role = user.Role,
				UserName = user.UserName
			};

			try
			{
				await _userRepository.InsertApplicationUserAsync(new List<AnalyticsUserEntity> { userEntity });
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public async Task<AnalyticsUserEntity> GetUsersByPublicId(Guid publicId)
		{
			return (await _userRepository.GetApplicationUsersByIdAsync(publicId)).Single();			
		}
	}
}
