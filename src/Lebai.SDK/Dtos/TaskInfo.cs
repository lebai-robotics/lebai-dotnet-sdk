using System;


namespace Lebai.SDK.Dtos
{
	public class OriginTaskInfo
	{
		public int Id { get; set; }

		public int? scene_id { get; set; }

		public int execute_count { get; set; }

		public int executed_count { get; set; }

		public string Name { get; set; }

		public TaskStatus Status { get; set; }

		public string Comment { get; set; }

		public string start_time { get; set; }

		public string end_time { get; set; }

		public long consume_time { get; set; }

		public int Mode { get; set; }

		public string create_time { get; set; }

		public string update_time { get; set; }

		public int? scene_type { get; set; }

		public object first_pose { get; set; }

		public TaskInfo ToTaskInfo()
		{
			return new TaskInfo
			{
				Id = Id,
				Comment = Comment,
				Mode = Mode,
				Name = Name,
				Status = Status,
				ConsumeTime = consume_time,
				CreationTime = create_time,
				EndTime = end_time,
				ExecuteCount = execute_count,
				ExecutedCount = executed_count,
				FirstPose = first_pose,
				SceneId = scene_id,
				SceneType = scene_type,
				StartTime = start_time,
				UpdateTime = update_time
			};
		}
	}

	public class TaskInfo
	{
		public int Id { get; set; }


		/// <summary>
		/// 场景Id
		/// </summary>
		public int? SceneId { get; set; }

		/// <summary>
		/// 执行次数
		/// </summary>
		public int? ExecuteCount { get; set; }

		/// <summary>
		/// 执行的次数
		/// </summary>
		public int? ExecutedCount { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public TaskStatus? Status { get; set; }

		/// <summary>
		/// 注释
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 开始时间
		/// </summary>
		public string StartTime { get; set; }

		/// <summary>
		/// 结束时间
		/// </summary>
		public string EndTime { get; set; }
		
		public long? ConsumeTime { get; set; }

		public int? Mode { get; set; }


		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreationTime { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		public string UpdateTime { get; set; }


		public int? SceneType { get; set; }


		public object FirstPose { get; set; }
	}
}