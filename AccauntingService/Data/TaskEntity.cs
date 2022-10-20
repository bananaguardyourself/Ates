using Contracts;

namespace AccountingService.Data
{
	public class TaskEntity
	{
		public TaskEntity(
			Guid publicId,
			Guid publicUserId,			
			string title,
			string jiraId,
			string description,
			TaskStatusType status
			)
		{
			PublicId = publicId;
			PublicUserId = publicUserId;			
			TaskTitle = title;
			TaskJiraId = jiraId;
			TaskDescription = description;
			TaskStatus = status;
		}

		public int Id { get; set; }
		public Guid PublicId { get; set; }
		public Guid PublicUserId { get; set; }
		public string TaskTitle { get; set; }
		public string TaskJiraId { get; set; }
		public string TaskDescription { get; set; }
		public TaskStatusType TaskStatus { get; set; }
		public int TaskCostAssign { get; set; } = Random.Shared.Next(10, 20);
		public int TaskCostComplete { get; set; } = Random.Shared.Next(20, 40);
	}
}
