using System;

namespace Lebai.SDK.Exceptions
{
	public class RobotException : Exception
	{
		public bool CanRetry { get; }

		public RobotException(string message, bool canRetry = false) : base(message)
		{
			CanRetry = canRetry;
		}
	}
}