using Contracts;
using TaskService.Data;
using System.Transactions;
using Kafka;
using System.Text.Json;
using TaskService.Models.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SchemaRegistry;

namespace TaskService.Business
{
	public class TaskTrackerManager
	{
		private readonly TaskRepository _taskRepository;
		private readonly ApplicationUserRepository _userRepository;
		private readonly KafkaProducer _kafkaProducer;

		public TaskTrackerManager(
			TaskRepository taskRepository,
			ApplicationUserRepository userRepository,
			KafkaProducer kafkaProducer)
		{
			_taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_kafkaProducer =  kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
		}

		public async Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId)
		{
			return await _taskRepository.GetTasksByUserIdAsync(userId);
		}

		public async Task<TaskEntity> AddTaskAsync(string title, string jiraId, string description)
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

			var taskEntity = new TaskEntity(description, title, jiraId, userId);
			var insertedCount = await _taskRepository.InsertTasksAsync(new List<TaskEntity> { taskEntity });

			if (insertedCount != 1)
			{
				throw new Exception("sql insert error");
			}

			// cud messaage
			string message = JsonConvert.SerializeObject(new TaskProcessed
			{
				EventName = "TaskCreated",
				EventId = Guid.NewGuid(),
				Producer = "TaskTrackerService",
				EventTime = DateTime.Now,
				EventVersion = 2,
				Data = new TaskProcessedData
				{
					PublicId = taskEntity.PublicId,
					UserId = taskEntity.PublicUserId,
					TaskDescription = taskEntity.TaskDescription,
					TaskTitle = taskEntity.TaskTitle,
					TaskJiraId = taskEntity.TaskJiraId,
					TaskStatus = taskEntity.TaskStatus
				}
			});

			var valid = JsonSchemaRegistry.Validate(JObject.Parse(message), "TaskCreated", 2);

			if (valid)
				await _kafkaProducer.ProduceMessage("task-stream", message);
			else
				//we can save message for example, or log this error and alert or use some other way of handling
				throw new Exception("Invalid message schema");

			// be message
			string beMessage = JsonConvert.SerializeObject(new TaskBusinessEvent
			{
				EventName = "TaskAdded",
				EventId = Guid.NewGuid(),
				Producer = "TaskTrackerService",
				EventTime = DateTime.Now,
				EventVersion = 2,
				Data = new TaskBusinessEventData
				{
					PublicId = taskEntity.PublicId,
					UserId = taskEntity.PublicUserId
				}
			});

			var bevalid = JsonSchemaRegistry.Validate(JObject.Parse(beMessage), "TaskAdded", 2);

			if (bevalid)
				await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);
			else
				throw new Exception("Invalid message schema");

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
				string message = JsonConvert.SerializeObject(new TaskProcessed
				{
					EventName = "TaskUpdated",
					EventId = Guid.NewGuid(),
					Producer = "TaskTrackerService",
					EventTime = DateTime.Now,
					EventVersion = 2,
					Data = new TaskProcessedData
					{
						PublicId = task.PublicId,
						UserId = task.PublicUserId,
						TaskDescription = task.TaskDescription,
						TaskTitle = task.TaskTitle,
						TaskJiraId = task.TaskJiraId,
						TaskStatus = task.TaskStatus
					}
				});

				var valid = JsonSchemaRegistry.Validate(JObject.Parse(message), "TaskUpdated", 2);

				if (valid)
					await _kafkaProducer.ProduceMessage("task-stream", message);
				else
					throw new Exception("Invalid message schema");

				// be message
				string beMessage = JsonConvert.SerializeObject(new TaskBusinessEvent
				{
					EventName = "TaskAssigned",
					EventId = Guid.NewGuid(),
					Producer = "TaskTrackerService",
					EventTime = DateTime.Now,
					EventVersion = 2,
					Data = new TaskBusinessEventData
					{
						PublicId = task.PublicId,
						UserId = task.PublicUserId
					}
				});

				var bevalid = JsonSchemaRegistry.Validate(JObject.Parse(beMessage), "TaskAssigned", 2);

				if (bevalid)
					await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);
				else
					throw new Exception("Invalid message schema");
			}

			return true;
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
			string message = JsonConvert.SerializeObject(new TaskProcessed
			{
				EventName = "TaskUpdated",
				EventId = Guid.NewGuid(),
				Producer = "TaskTrackerService",
				EventTime = DateTime.Now,
				EventVersion = 2,
				Data = new TaskProcessedData
				{
					PublicId = userTask.PublicId,
					UserId = userTask.PublicUserId,
					TaskDescription = userTask.TaskDescription,
					TaskTitle = userTask.TaskTitle,
					TaskJiraId = userTask.TaskJiraId,
					TaskStatus = userTask.TaskStatus
				}
			});

			var valid = JsonSchemaRegistry.Validate(JObject.Parse(message), "TaskUpdated", 2);

			if (valid)
				await _kafkaProducer.ProduceMessage("task-stream", message);
			else
				throw new Exception("Invalid message schema");

			// be message
			string beMessage = JsonConvert.SerializeObject(new TaskBusinessEvent
			{
				EventName = "TaskClosed",
				EventId = Guid.NewGuid(),
				Producer = "TaskTrackerService",
				EventTime = DateTime.Now,
				EventVersion = 2,
				Data = new TaskBusinessEventData
				{
					PublicId = userTask.PublicId,
					UserId = userTask.PublicUserId
				}
			});

			var bevalid = JsonSchemaRegistry.Validate(JObject.Parse(beMessage), "TaskClosed", 2);

			if (bevalid)
				await _kafkaProducer.ProduceMessage("task-lifecycle", beMessage);
			else
				throw new Exception("Invalid message schema");

			return true;
		}
	}
}
