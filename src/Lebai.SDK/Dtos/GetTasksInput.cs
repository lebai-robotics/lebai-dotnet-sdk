namespace Lebai.SDK.Dtos
{
	public class GetTasksInput
	{
		/// <summary>
		/// 页索引
		/// </summary>
		public int PageIndex { get; set; } = 1;

		/// <summary>
		/// 页大小
		/// </summary>
		public int PageSize { get; set; } = 10;
	}
}