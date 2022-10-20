using Contracts;
using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace AccountingService.Data
{
	public class TaskRepository : BaseRepository
	{
		public TaskRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertTasksAsync(IEnumerable<TaskEntity> tasks)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO public.tasks
				( publicid, publicuserid, tasktitle, taskjiraid, taskdescription, taskstatus, TaskCostAssign, TaskCostComplete) VALUES 
				( @Publicid, @PublicUserId, @TaskTitle, @TaskJiraId, @TaskDescription, @TaskStatus, @TaskCostAssign, @TaskCostComplete);", tasks);
		}

		public async Task<TaskEntity> GetTasksByPublicIdAsync(Guid publicId)
		{
			using var cnn = SimpleDbConnection();
			return (await cnn.QueryAsync<TaskEntity>(@"SELECT * FROM public.tasks where publicid = @publicId;", publicId)).SingleOrDefault();
		}

		public async Task<int> UpdateTaskAsync(IEnumerable<TaskEntity> tasks)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"UPDATE public.tasks
				set (publicid = @Publicid, publicuserid = @PublicUserId, tasktitle = @TaskTitle, taskjiraid = @TaskJiraId, taskdescription = @TaskDescription, taskstatus = @TaskStatus, TaskCostAssign = @TaskCostAssign, TaskCostComplete = @TaskCostComplete
				where publicid = @PublicId;", new { tasks });
		}

		public async Task<int> InsertDeadLetterAsync(string message, DateTime timestanmp)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO taskdeadletters
				( message, timereceived) VALUES 
				( @message, @timestanmp);", new { message, timestanmp });
		}
	}
}
