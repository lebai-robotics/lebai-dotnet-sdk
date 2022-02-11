namespace Lebai.SDK.Dtos
{
	public class LebaiHttpResult<T>
	{
		/// <summary>
		/// 数据
		/// </summary>
		public T Data { get; set; }

		/// <summary>
		/// 错误码
		/// </summary>
		public int Code { get; set; }
	}
}