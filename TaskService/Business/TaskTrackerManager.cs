using Contracts;
using TaskService.Data;
using System.Transactions;
using Kafka;
using System.Text.Json;
using TaskService.Models.Kafka;

namespace TaskService.Business
{
	public class TaskTrackerManager
	{
		private readonly TaskRepository _taskRepository;
		private readonly ApplicationUserRepository _userRepository;
		private readonly KafkaProducer _kafkaProducer;

		public TaskTrackerManager(
			TaskRepository taskRepository,
			ApplicationUserRepository userRepository)
		{
			_taskRepository = taskRepository;
			_userRepository = userRepository;
		}

		public async Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId)
		{
			return await _taskRepository.GetTasksByUserIdAsync(userId);
		}

		public async Task<bool> CloseTasksAsync(Guid userId, Guid taskId)
		{
			var userTask = (await _taskRepository.GetTasksByTaskAndUserIdAsync(userId, taskId)).SingleOrDefault();

			if (userTask is null || userTask.TaskStatus == TaskStatusType.Closed)
				return false;

			userTask.TaskStatus = TaskStatusType.Closed;

			var updatedCount = await _taskRepository.UpdateTaskAsync(new List<TaskEntity> { userTask });

			if (updatedCount < 1)
				return false;

			// cud messaage
			string message = JsonSerializer.Serialize(new TaskProcessed
			{
				EventName = "TaskUpdated",
				PublicId = userTask.PublicId,
				UserId = userTask.PublicUserId,
				TaskDescription = userTask.TaskDescription,
				TaskName = userTask.TaskName,
				TaskStatus = userTask.TaskStatus,
			});
			await _kafkaProducer.ProduceMessage("task-stream", message);


			// be message
			string beMessage = JsonSerializer.Serialize(new TaskBusinessEvent
			{
				EventName = "TaskClosed",
				PublicId = userTask.PublicId,
				UserId = userTask.PublicUserId
			});
			await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);

			return true;
		}

		public async Task<TaskEntity> AddTaskAsync(string name, string description)
		{
			var users = await _userRepository.GetApplicationUsersAsync();

			var usersToAssign = users.Where(
				u => string.Compare(u.Role, Roles.Manager, true) != 0 && string.Compare(u.Role, Roles.Admin, true) != 0);

			if (usersToAssign == null || !usersToAssign.Any())
			{
				throw new Exception("No users");
			}

			var random = new Random();
			var userId = usersToAssign.ToList()[random.Next(usersToAssign.Count())].PublicId;

			var taskEntity = new TaskEntity(description, name, userId);
			var insertedCount = await _taskRepository.InsertTasksAsync(new List<TaskEntity> { taskEntity });

			if (insertedCount != 1)
			{
				throw new Exception("sql insert error");
			}

			// cud messaage
			string message = JsonSerializer.Serialize(new TaskProcessed
			{
				EventName = "TaskCreated",
				PublicId = taskEntity.PublicId,
				UserId = taskEntity.PublicUserId,
				TaskDescription = taskEntity.TaskDescription,
				TaskName = taskEntity.TaskName,
				TaskStatus = taskEntity.TaskStatus,
			});
			await _kafkaProducer.ProduceMessage("task-stream", message);


			// be message
			string beMessage = JsonSerializer.Serialize(new TaskBusinessEvent
			{
				EventName = "TaskAdded",
				PublicId = taskEntity.PublicId,
				UserId = taskEntity.PublicUserId
			});
			await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);

			return taskEntity;
		}

		public async Task<bool> AssignTasksAsync()
		{
			var users = await _userRepository.GetApplicationUsersAsync();

			var usersToAssign = users.Where(
				u => string.Compare(u.Role, Roles.Manager, true) != 0 && string.Compare(u.Role, Roles.Admin, true) != 0);

			if (usersToAssign == null || !usersToAssign.Any())
			{
				throw new Exception("No users");
			}

			var random = new Random();
			var usersList = usersToAssign.ToList();

			var tasks = await _taskRepository.GetTasksByStatusAsync(TaskStatusType.Open);

			using (var transactionScope = new TransactionScope())
			{
				foreach (var task in tasks)
				{
					task.PublicUserId = usersList[random.Next(usersToAssign.Count())].PublicId;
				}

				var updatedCount = await _taskRepository.UpdateTaskAsync(tasks);

				if (updatedCount != tasks.Count())
					throw new Exception("Optimistic lock exception");

				transactionScope.Complete();
			}

			foreach (var task in tasks)
			{
				// cud messaage
				string message = JsonSerializer.Serialize(new TaskProcessed
				{
					EventName = "TaskUpdated",
					PublicId = task.PublicId,
					UserId = task.PublicUserId,
					TaskDescription = task.TaskDescription,
					TaskName = task.TaskName,
					TaskStatus = task.TaskStatus,
				});
				await _kafkaProducer.ProduceMessage("task-stream", message);

				// be message
				string beMessage = JsonSerializer.Serialize(new TaskBusinessEvent
				{
					EventName = "TaskAssigned",
					PublicId = task.PublicId,
					UserId = task.PublicUserId
				});
				await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);
			}

			return true;
		}
	}
}
