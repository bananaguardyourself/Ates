using AccountingService.Data;
using AccountingService.Models.Kafka;

namespace AccountingService.Business
{
	public class AccountingTaskManager
	{
		private readonly TaskRepository _taskRepository;

		public AccountingTaskManager(TaskRepository taskRepository)
		{
			_taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
		}

		public async Task CreateTaskAsync(TaskProcessed task)
		{
			var taskEntity = new TaskEntity(task.Data.PublicId, task.Data.PublicUserId, task.Data.TaskTitle,
				task.Data.TaskJiraId, task.Data.TaskDescription, task.Data.TaskStatus);

			try
			{
				await _taskRepository.InsertTasksAsync(new List<TaskEntity> { taskEntity });
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		public async Task UpdateTaskAsync(TaskProcessed updatedTask)
		{
			var task = await _taskRepository.GetTasksByPublicIdAsync(updatedTask.Data.PublicId);

			if (task == null)
			{
				throw new Exception(string.Format($"Task not found, id: {0}", updatedTask.Data.PublicId));
			}

			task.PublicId = updatedTask.Data.PublicId;
			task.PublicUserId = updatedTask.Data.PublicUserId;
			task.TaskTitle = updatedTask.Data.TaskTitle;
			task.TaskJiraId = updatedTask.Data.TaskJiraId;
			task.TaskDescription = updatedTask.Data.TaskDescription;
			task.TaskStatus = updatedTask.Data.TaskStatus;

			try
			{
				await _taskRepository.UpdateTaskAsync(new List<TaskEntity> { task });
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
				await _taskRepository.InsertDeadLetterAsync(message, DateTime.Now);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}
	}
}
