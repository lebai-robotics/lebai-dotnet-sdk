using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lebai.SDK.Dtos;
using Lebai.SDK.Exceptions;
using TaskStatus = Lebai.SDK.Dtos.TaskStatus;
#if NET5_0||NET6_0
using System.Net.Http.Json;

#endif

namespace Lebai.SDK
{
   public class EnumExtension
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="tField"></param>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      public static string GetEnumDescription<T>(T tField)
      {
         var description = string.Empty; //结果
         var inputType = tField.GetType(); //输入的类型
         var descType = typeof(DescriptionAttribute); //目标查找的描述类型

         var fieldStr = tField.ToString(); //输入的字段字符串
         var field = inputType.GetField(fieldStr); //目标字段

         var isDefined = field.IsDefined(descType, false); //判断描述是否在字段的特性
         if (isDefined)
         {
            var enumAttributes = (DescriptionAttribute[]) field //得到特性信息
               .GetCustomAttributes(descType, false);
            description = enumAttributes.FirstOrDefault()?.Description ?? string.Empty;
         }

         return description;
      }
   }

   public partial class LebaiRobotClient
   {
      public static Dictionary<int, string> CodeMessage = new()
      {
         [2001] = "系统异常",
         [2002] = "登录授权码错误",
         [2003] = "登录授权码已失效",
         [2004] = "机器人控制系统异常",
         [2005] = "404",
         [2006] = "参数错误",
         [2007] = "数据不存在",
         [2009] = "请登录",
         [2010] = "同一时间只能有一个用户登录",
         [2011] = "队列任务执行报错",
         [2012] = "机器人任务运行中不能运行其他任务",
         [2015] = "场景导入失败，导入文件格式错误",
         [2021] = "数据库异常",
         [2022] = "签名失败",
         [2023] = "任务队列恢复失败，手臂当前位置与即将运行轨迹的首个位置数据校验失败",
         [2024] = "无效机器人操作命令",
         [2025] = "机器人当前状态没有满足执行当前指令的预期（废弃）",
         [2026] = "请求超时",
         [2027] = "网络配置中，不能频繁进行操作",
         [2028] = "条件任务执行超时",

         [2029] = "机器人控制系统故障，请重启机器人后再试",
         [2030] = "机器人通信故障，请检查机器人是否已正确连接",
         [2031] = "机器人初始化中，请稍候再试",
         [2032] = "机器人更新中，请稍候再试",
         [2033] = "机器人启动中，请稍候再试",
         [2034] = "机器人停止中，请稍候再试",
         [2035] = "请结束示教操作后再试",
         [2036] = "请先停止任务历史中的当前任务后再执行相应操作",
         [2037] = "仿真模式暂不支持该功能"
      };

      /// <summary>
      ///    获取指定任务信息
      /// </summary>
      /// <param name="id"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async ValueTask<TaskInfo> GetTask(int id, CancellationToken cancellationToken = default)
      {
         var response = await _httpClient.GetAsync($"/public/task?id={id}", cancellationToken);

         var r = await response.Content.ReadFromJsonAsync<LebaiHttpResult<OriginTaskInfo>>(
            cancellationToken: cancellationToken);
         HandleResult(r, $"场景Id：{id}");
         return r?.Data.ToTaskInfo();
      }

      /// <summary>
      ///    获取任务信息
      /// </summary>
      /// <param name="input"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async ValueTask<TasksResult> GetTasks(GetTasksInput input,
         CancellationToken cancellationToken = default)
      {
         var response = _httpClient
            .GetAsync($"/public/tasks?pi={input.PageIndex}&ps={input.PageSize}", cancellationToken).Result;
         var r = await response.Content.ReadFromJsonAsync<LebaiHttpResult<OriginTasksResult>>(
            cancellationToken: cancellationToken);

         HandleResult(r);
         return r?.Data?.ToResult();
      }

      /// <summary>
      ///    检测是否能运行任务（机器人是否正在运行其他任务）
      /// </summary>
      /// <returns></returns>
      public virtual async ValueTask<bool> GetIsCanRunTask(CancellationToken cancellationToken = default)
      {
         var result = await GetTasks(new GetTasksInput {PageIndex = 1, PageSize = 1}, cancellationToken);
         var first = result.Items.FirstOrDefault();
         return first == null || first.Status != TaskStatus.Running && first.Status != TaskStatus.Pause;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="cancellationToken"></param>
      /// <exception cref="Exception"></exception>
      protected virtual async ValueTask CheckRobotStatus(CancellationToken cancellationToken = default)
      {
         if (!await GetIsCanRunTask(cancellationToken)) throw new Exception("机器人正在执行其他任务!");
      }

      /// <summary>
      ///    运行任务
      /// </summary>
      /// <param name="id">任务Id</param>
      /// <param name="executeCount">执行次数</param>
      /// <param name="clear">是否强制停止正在运行的任务</param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<TaskExecuteResult> RunTask(int id, int executeCount = 1,
         bool clear = true, CancellationToken cancellationToken = default)
      {
         await CheckRobotStatus(cancellationToken);
         var response = _httpClient.PostAsJsonAsync("/public/task", new
         {
            execute_count = executeCount,
            clear = clear ? 1 : 0,
            task_id = id
         }, cancellationToken).Result;
         var r = await response.Content.ReadFromJsonAsync<LebaiHttpResult<TaskExecuteResult>>(
            cancellationToken: cancellationToken);
         HandleResult(r);
         return r?.Data;
      }

      /// <summary>
      ///    运行场景
      /// </summary>
      /// <param name="id">场景Id</param>
      /// <param name="executeCount">运行次数</param>
      /// <param name="clear">是否强制停止正在运行的场景</param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<TaskExecuteResult> RunScene(int id, int executeCount = 1,
         bool clear = false, CancellationToken cancellationToken = default)
      {
         await CheckRobotStatus(cancellationToken);

         var response = _httpClient.PostAsJsonAsync("/public/task", new
         {
            execute_count = executeCount,
            clear = clear ? 1 : 0,
            scene_id = id
         }, cancellationToken).Result;
         var r = await response.Content.ReadFromJsonAsync<LebaiHttpResult<TaskExecuteResult>>(
            cancellationToken: cancellationToken);
         HandleResult(r, $"场景Id：{id}");
         return r?.Data;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="result"></param>
      /// <param name="message"></param>
      /// <typeparam name="T"></typeparam>
      /// <exception cref="HttpRequestException"></exception>
      protected void HandleResult<T>(LebaiHttpResult<T> result, string message = "")
      {
         if (result.Code != 0)
            throw new HttpRequestException($"调用失败，{message}，Code：{result.Code}" +
                                           (CodeMessage.ContainsKey(result.Code)
                                              ? $"，{CodeMessage[result.Code]}"
                                              : ""));
      }

      /// <summary>
      ///    等待任务运行完成
      /// </summary>
      /// <param name="id">任务Id</param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      /// <exception cref="OperationCanceledException"></exception>
      /// <exception cref="RobotTaskException"></exception>
      public virtual async ValueTask<TaskInfo> WaitTaskRunCompleted(int id,
         CancellationToken cancellationToken = default)
      {
         var taskInfo = await GetTask(id, cancellationToken);
         while (true)
         {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

            if (taskInfo.Status is TaskStatus.Running or TaskStatus.Idea)
            {
               await Task.Delay(100, cancellationToken);
               taskInfo = await GetTask(id, cancellationToken);
            }
            else if (taskInfo.Status is TaskStatus.RunSuccess)
            {
               break;
            }
            else
            {
               throw new RobotTaskException(taskInfo.Status);
            }
         }

         return taskInfo;
      }

      /// <summary>
      ///    运行场景直到运行完成
      /// </summary>
      /// <param name="id">场景Id</param>
      /// <param name="executeCount">执行次数</param>
      /// <param name="clear">是否强制停止正在运行的场景</param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<TaskInfo> RunSceneUntilDone(int id, int executeCount = 1,
         bool clear = false, CancellationToken cancellationToken = default)
      {
         var r = await RunScene(id, executeCount, clear, cancellationToken);

         return await WaitTaskRunCompleted(r.Id, cancellationToken);
      }

      /// <summary>
      ///    执行Lua 代码
      /// </summary>
      /// <param name="luaCode"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public virtual async Task<TaskExecuteResult> ExecuteLua(string luaCode,
         CancellationToken cancellationToken = default)
      {
         var response =
            await _httpClient.PostAsync("/public/executor/lua", new StringContent(luaCode), cancellationToken);
         var r = await response.Content.ReadFromJsonAsync<LebaiHttpResult<TaskExecuteResult>>(
            cancellationToken: cancellationToken);

         HandleResult(r);
         return r?.Data;
      }
   }
}