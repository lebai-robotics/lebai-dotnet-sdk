﻿using Lebai.SDK;
using Lebai.SDK.Dtos;
using Robotc;

var client = new LebaiRobotClient("192.168.3.227" /* 乐白机械臂 IP 地址 */);

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
var taskResult2 = await client.RunSceneUntilDone(10001 /*场景Id*/);

// 运行场景，返回的对象包含 任务Id
var (taskId) = await client.RunScene(10001);

// 等待指定任务做完
await client.WaitTaskRunCompleted(taskId);

// 查询任务信息
var taskInfo = await client.GetTask(taskId);

// 获取任务信息列表
var taskInfos = await client.GetTasks(new GetTasksInput
{
   PageIndex = 0, // 页索引
   PageSize = 10 // 页大小
});