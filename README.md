# Lebai Robot DotNet SDK

[![NuGet](https://img.shields.io/nuget/v/Lebai.SDK.svg)](https://www.nuget.org/packages/Lebai.SDK/)

Nuget包地址：https://www.nuget.org/packages/Lebai.SDK

Api 文档：https://www.fuget.org/packages/Lebai.SDK/latest/lib/net5.0/Lebai.SDK.dll/Lebai.SDK/LebaiRobotClient

乐白机器人开发手册：https://www.lebai.ltd/product/document/1/2

平台：.NET 5.0、.Net 6.0

## 示例

```C#
using System.Threading.Tasks;
using Robotc;
using Lebai.SDK;

namespace Lebai.SDK
{
	public class Sample
	{
		public static async Task Main(string[] args)
		{
			var client = new LebaiRobotClient("192.168.3.227");			
			// 启动
			await client.StartSys();
			// 获取机器人数据
			var robotData = await client.GetRobotData();
			// 获取IO数据
			await client.GetDIO(new IOPin
			{
				Pin = 1
			});
			// 运行场景直到做完
			var taskResult2 = await client.RunSceneUntilDone(10001);
		}
	}
}
```
