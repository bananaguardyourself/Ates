using Contracts;
using Contracts.Settings;
using Dapper;
using Microsoft.Extensions.Options;

namespace TaskService.Data
{
	public class TaskRepository : BaseRepository
	{
		public TaskRepository(IOptions<DbConnectionOptions> options) : base(options)
		{

		}

		public async Task<int> InsertTasksAsync(IEnumerable<TaskEntity> tasks)
		{
			foreach (var task in tasks)
			{
				task.LastUpdated = DateTime.Now;
			}

			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"INSERT INTO public.tasks
				( publicid, userid, tasktitle, taskjiraid, taskdescription, taskstatus, lastupdated) VALUES 
				( Publicid, Userid, TaskTitle, TaskJiraId, TaskDescription, TaskStatus, LastUpdated);", tasks);
		}

		public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(Guid userid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<TaskEntity>(@"SELECT * FROM public.tasks where userid = @userid;", userid);
		}

		public async Task<IEnumerable<TaskEntity>> GetTasksByTaskAndUserIdAsync(Guid userid, Guid taskid)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<TaskEntity>(@"SELECT * FROM public.tasks where userid = @userid and publicid = @taskid;", new { userid, taskid });
		}

		public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(TaskStatusType taskstatus)
		{
			using var cnn = SimpleDbConnection();
			return await cnn.QueryAsync<TaskEntity>(@"SELECT * FROM public.tasks where taskstatus = @taskstatus;", taskstatus);
		}

		public async Task<int> UpdateTaskAsync(IEnumerable<TaskEntity> tasks)
		{
			var updateTimestamp = DateTime.Now;
			using var cnn = SimpleDbConnection();
			return await cnn.ExecuteAsync(@"UPDATE public.tasks
				set (userid = @Userid, taskname = @TaskName, taskdescription = @TaskDescription, taskstatus = @TaskStatus, lastupdated = @updateTimestamp
				where publicid = @PublicId and lastupdated = @LastUpdated;", new { tasks, updateTimestamp });
		}
	}
}
