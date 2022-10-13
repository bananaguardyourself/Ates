using Contracts;

namespace TaskService.Data
{
	public class TaskEntity
	{

		public TaskEntity(string description, string name, Guid userId)
		{
			UserId = userId;
			TaskDescription = description;
			TaskName = name;
		}

		public int Id { get; set; }
		public Guid PublicId { get; set; } = new Guid();
		public Guid UserId { get; set; }
		public string TaskName { get; set; }
		public string TaskDescription { get; set; }
		public TaskStatusType TaskStatus { get; set; } = TaskStatusType.Open;
		public DateTime LastUpdated { get; set; }
	}
}
