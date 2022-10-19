using Contracts;

namespace TaskService.Data
{
	public class TaskEntity
	{
		public TaskEntity(string description, string title, string jiraId, Guid userId)
		{
			PublicUserId = userId;
			TaskDescription = description;
			TaskTitle = title;
			TaskJiraId = jiraId;
		}

		public int Id { get; set; }
		public Guid PublicId { get; set; } = new Guid();
		public Guid PublicUserId { get; set; }
		public string TaskTitle { get; set; }
		public string TaskJiraId { get; set; }
		public string TaskDescription { get; set; }
		public TaskStatusType TaskStatus { get; set; } = TaskStatusType.Open;
		public DateTime LastUpdated { get; set; }
	}
}
