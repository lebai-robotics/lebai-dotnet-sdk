using System.Linq;

namespace Lebai.SDK.Dtos
{
	public class OriginTasksResult
	{
		public int pi { get; set; }

		public int ps { get; set; }

		public int total { get; set; }

		public OriginTaskInfo[] records { get; set; }

		public TasksResult ToResult()
		{
			return new TasksResult
			{
				Items = records.Select(n=>n.ToTaskInfo()).ToArray(),
				PageIndex = pi,
				PageSize = ps,
				TotalCount = total
			};
		}
	}

	public class TasksResult
	{
		/// <summary>
		/// 页索引
		/// </summary>
		public int PageIndex { get; set; }

		/// <summary>
		/// 页大小
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// 总数量
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// 任务列表
		/// </summary>
		public TaskInfo[] Items { get; set; }
	}
}