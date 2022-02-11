using System.ComponentModel;

namespace Lebai.SDK.Dtos
{
	public enum RobotStatus
	{
	
		/// <summary>
		/// 已断开连接
		/// </summary>
		[Description("已断开连接")] DISCONNECTED = 0,


		/// <summary>
		/// 急停停止状态
		/// </summary>
		[Description("急停停止状态")] ESTOP = 1,


		/// <summary>
		/// 启动中
		/// </summary>
		[Description("启动中")] BOOTING = 2,


		/// <summary>
		/// 电源关闭
		/// </summary>
		[Description("电源关闭")] ROBOT_OFF = 3,


		/// <summary>
		/// 电源开启
		/// </summary>
		[Description("电源开启")] ROBOT_ON = 4,


		/// <summary>
		/// 空闲中
		/// </summary>
		[Description("空闲中")] IDLE = 5,


		/// <summary>
		/// 暂停中
		/// </summary>
		[Description("暂停中")] PAUSED = 6,


		/// <summary>
		/// 机器人运动运行中
		/// </summary>
		[Description("机器人运动运行中")] RUNNING = 7,


		/// <summary>
		/// 更新固件中
		/// </summary>
		[Description("更新固件中")] UPDATING = 8,


		/// <summary>
		/// 启动中
		/// </summary>
		[Description("启动中")] STARTING = 9,


		/// <summary>
		/// 停止中
		/// </summary>
		[Description("停止中")] STOPPING = 10,


		/// <summary>
		/// 示教中
		/// </summary>
		[Description("示教中")] TEACHING = 11,


		/// <summary>
		/// 普通停止
		/// </summary>
		[Description("普通停止")] STOP = 12,
	}
}