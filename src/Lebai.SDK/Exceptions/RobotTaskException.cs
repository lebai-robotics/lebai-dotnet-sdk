using Lebai.SDK.Dtos;

namespace Lebai.SDK.Exceptions
{
	public class RobotTaskException : RobotException
	{
		public TaskStatus TaskStatus { get; }

		public RobotTaskException(TaskStatus taskStatus) : base(EnumExtension.GetEnumDescription(taskStatus))
		{
			TaskStatus = taskStatus;
		}
	}
}