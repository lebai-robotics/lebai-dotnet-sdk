using Lebai.SDK.Dtos;

namespace Lebai.SDK.Exceptions
{
	public class RobotStatusException : RobotException
	{
		private RobotStatus RobotStatus { get; }

		public RobotStatusException(RobotStatus robotStatus) : base(EnumExtension.GetEnumDescription(robotStatus))
		{
			RobotStatus = robotStatus;
		}
	}
}