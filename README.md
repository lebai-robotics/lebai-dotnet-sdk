# Lebai Robot DotNet SDK

[![NuGet](https://img.shields.io/nuget/v/Lebai.SDK.svg)](https://www.nuget.org/packages/Lebai.SDK/)

Nuget包地址：https://www.nuget.org/packages/Lebai.SDK

Api 文档：https://www.fuget.org/packages/Lebai.SDK/latest/lib/net5.0/Lebai.SDK.dll/Lebai.SDK/LebaiRobotClient

乐白机器人开发手册：https://docs.lebai.ltd/lmaster/api-doc/

平台：.NET 5.0、.Net 6.0

## 示例

```C#
using Lebai.SDK;
using Lebai.SDK.Dtos;
using Robotc;

var client = new LebaiRobotClient("192.168.4.75" /* 乐白机械臂 IP 地址 */);

// 启动
await client.StartSys();

// 等待执行完毕（例如：启动、急停等）
await client.Sync();

// 获取机器人数据
var robotData = await client.GetRobotData();

// 获取IO数据
await client.GetDIO(new IOPin
{
   Pin = 1
});

// 运行场景直到做完
var taskResult2 = await client.RunSceneUntilDone(10002 /*场景Id*/);

// 运行场景，返回的对象包含 任务Id
var runResult = await client.RunScene(10002);

// 等待指定任务做完
await client.WaitTaskRunCompleted(runResult.Id);

// 查询任务信息
var taskInfo = await client.GetTask(runResult.Id);

// 获取任务信息列表
var taskInfos = await client.GetTasks(new GetTasksInput
{
   PageIndex = 0, // 页索引
   PageSize = 10 // 页大小
});
```
