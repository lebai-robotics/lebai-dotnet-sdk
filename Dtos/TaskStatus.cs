using System.ComponentModel;

namespace Lebai.SDK.Dtos
{
	public enum TaskStatus
	{
		/// <summary>
		///    空闲
		/// </summary>
		[Description("空闲")]
		Idea = 0,

		/// <summary>
		///    运行
		/// </summary>
		[Description("运行")]
		Running = 1,

		/// <summary>
		///    暂停
		/// </summary>
		[Description("暂停")]
		Pause = 2,

		/// <summary>
		///    运行成功
		/// </summary>
		[Description("运行成功")]
		RunSuccess = 3,

		/// <summary>
		///    手动停止
		/// </summary>
		[Description("手动停止")]
		ManualStop = 4,

		/// <summary>
		///    异常停止
		/// </summary>
		[Description("异常停止")]
		AbnormalStop = 5
	}
}