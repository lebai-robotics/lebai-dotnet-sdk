using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Robotc;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Lebai.SDK
{
    public partial class LebaiRobotClient
    {
        public static readonly Dictionary<string, LebaiRobotClient> LebaiRobotClients =
            new Dictionary<string, LebaiRobotClient>();

        private readonly HttpClient _httpClient;

        private RobotController.RobotControllerClient _robotControllerClient;
        private RobotPrivateController.RobotPrivateControllerClient _robotPrivateControllerClient;

        private string IP { get; }
        private int RobotControllerClientPort { get; }
        private int RobotPrivateControllerPort { get; }
        private GrpcChannelOptions GrpcChannelOptions { get; }

        public Action<RpcException> OnRpcException { get; set; }

        public LebaiRobotClient(string ip, int robotControllerClientPort = 5181, int robotPrivateControllerPort = 5182,
            GrpcChannelOptions grpcChannelOptions = null)
        {
            IP = ip;
            RobotControllerClientPort = robotControllerClientPort;
            RobotPrivateControllerPort = robotPrivateControllerPort;
            GrpcChannelOptions = grpcChannelOptions;
            _httpClient = new HttpClient {BaseAddress = new Uri($"http://{ip}")};
        }

        public static LebaiRobotClient Get(string ip, int robotControllerClientPort = 5181,
            int robotPrivateControllerPort = 5182)
        {
            if (LebaiRobotClients.ContainsKey(ip))
            {
                return LebaiRobotClients[ip];
            }

            LebaiRobotClients[ip] = new LebaiRobotClient(ip);
            return LebaiRobotClients[ip];
        }

        private RobotController.RobotControllerClient RobotControllerClient =>
            LazyInitializer.EnsureInitialized(ref _robotControllerClient,
                () =>
                {
                    var r = new RobotController.RobotControllerClient(
                        GrpcChannel.ForAddress(new Uri($"http://{IP}:{RobotControllerClientPort}"),
                            GrpcChannelOptions ??
                            new GrpcChannelOptions
                            {
                                MaxRetryAttempts = 5
                            })
                    );
                    return r;
                });

        private RobotPrivateController.RobotPrivateControllerClient RobotPrivateControllerClient =>
            LazyInitializer.EnsureInitialized(ref _robotPrivateControllerClient,
                () => new RobotPrivateController.RobotPrivateControllerClient(
                    GrpcChannel.ForAddress(new Uri($"http://{IP}:{RobotPrivateControllerPort}"), GrpcChannelOptions ??
                        new GrpcChannelOptions
                        {
                            MaxRetryAttempts = 5
                        })));

        /// <summary>
        /// 启用/禁用 设备
        /// </summary>
        /// <param name="externalIoState"></param>
        public virtual async ValueTask ConnectExternalIO(ExternalIOState externalIoState)
        {
            await RobotPrivateControllerClient.ConnectExternalIOAsync(externalIoState);
        }

        /// <summary>
        /// 关闭电源
        /// </summary>
        public virtual async ValueTask PowerDown()
        {
            await RobotControllerClient.PowerDownAsync(new Empty());
        }

        /// <summary>
        /// 等待，单位毫秒
        /// </summary>
        /// <param name="sleepRequest"></param>
        public virtual async ValueTask Sleep(SleepRequest sleepRequest)
        {
            await RobotControllerClient.SleepAsync(sleepRequest);
        }

        /// <summary>
        /// 同步，等待命令执行完成
        /// </summary>
        public virtual async ValueTask Sync()
        {
            try
            {
                await RobotControllerClient.SyncAsync(new Empty());
            }
            catch (RpcException e)
            {
                OnRpcException?.Invoke(e);
            }
        }

        public virtual async ValueTask SyncFor(SyncRequest syncRequest)
        {
            await RobotControllerClient.SyncForAsync(syncRequest);
        }

        /// <summary>
        /// 开启示教模式
        /// </summary>
        public virtual async ValueTask TeachMode()
        {
            await RobotControllerClient.TeachModeAsync(new Empty());
        }

        /// <summary>
        /// 关闭示教模式
        /// </summary>
        public virtual async ValueTask EndTeachMode()
        {
            await RobotControllerClient.EndTeachModeAsync(new Empty());
        }

        /// <summary>
        /// 设置速度因子（0-100）
        /// </summary>
        /// <param name="factor"></param>
        public virtual async ValueTask SetVelocityFactor(Factor factor)
        {
            await RobotControllerClient.SetVelocityFactorAsync(factor);
        }

        /// <summary>
        /// 获取速度因子（0-100）
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Factor> GetVelocityFactor()
        {
            return RobotControllerClient.GetVelocityFactorAsync(new Empty());
        }

        /// <summary>
        /// 设置重力方向
        /// </summary>
        /// <param name="coordinate"></param>
        public virtual async ValueTask SetGravity(Coordinate coordinate)
        {
            await RobotControllerClient.SetGravityAsync(coordinate);
        }

        /// <summary>
        /// 获取重力方向
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Coordinate> GetGravity()
        {
            return RobotControllerClient.GetGravityAsync(new Empty());
        }

        /// <summary>
        /// 设置负载
        /// </summary>
        /// <param name="payload"></param>
        public virtual async ValueTask SetPayload(Payload payload)
        {
            await RobotControllerClient.SetPayloadAsync(payload);
        }

        /// <summary>
        /// 获取负载
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Payload> GetPayload()
        {
            return RobotControllerClient.GetPayloadAsync(new Empty());
        }

        /// <summary>
        /// 设置负责质量
        /// </summary>
        /// <param name="payloadMass"></param>
        public virtual async ValueTask SetPayloadMass(PayloadMass payloadMass)
        {
            await RobotControllerClient.SetPayloadMassAsync(payloadMass);
        }

        /// <summary>
        /// 获取负责质量
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<PayloadMass> GetPayloadMass()
        {
            return RobotControllerClient.GetPayloadMassAsync(new Empty());
        }

        /// <summary>
        /// 设置负载质心
        /// </summary>
        /// <param name="payloadCog"></param>
        public virtual async ValueTask SetPayloadCog(PayloadCog payloadCog)
        {
            await RobotControllerClient.SetPayloadCogAsync(payloadCog);
        }

        /// <summary>
        /// 获取负责质心
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<PayloadCog> GetPayloadCog()
        {
            return RobotControllerClient.GetPayloadCogAsync(new Empty());
        }

        /// <summary>
        /// 设置TCP
        /// </summary>
        /// <param name="pR"></param>
        public virtual async ValueTask SetTcp(PR pR)
        {
            await RobotControllerClient.SetTcpAsync(pR);
        }

        /// <summary>
        /// 获取TCP
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<PR> GetTcp()
        {
            return RobotControllerClient.GetTcpAsync(new Empty());
        }

        /// <summary>
        /// 设置手爪幅度：0-100 double
        /// </summary>
        /// <param name="amplitude"></param>
        public virtual async ValueTask SetClawAmplitude(Amplitude amplitude)
        {
            await RobotControllerClient.SetClawAmplitudeAsync(amplitude);
        }

        /// <summary>
        /// 获得手爪幅度：0-100 double
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Amplitude> GetClawAmplitude()
        {
            return RobotControllerClient.GetClawAmplitudeAsync(new Empty());
        }

        /// <summary>
        /// 获得手爪目前是否夹紧物体状态1表示夹紧，0为松开
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<HoldOn> GetClawHoldOn()
        {
            return RobotControllerClient.GetClawHoldOnAsync(new Empty());
        }

        /// <summary>
        /// 设置手爪力度：0-100 double
        /// </summary>
        /// <param name="force"></param>
        public virtual async ValueTask SetClawForce(Force force)
        {
            await RobotControllerClient.SetClawForceAsync(force);
        }

        /// <summary>
        /// 获得手爪称重结果
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Weight> GetClawWeight()
        {
            return RobotControllerClient.GetClawWeightAsync(new Empty());
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ForceTorque> GetTcpForce()
        {
            return RobotControllerClient.GetTcpForceAsync(new Empty());
        }

        /// <summary>
        /// 设置手爪
        /// </summary>
        /// <param name="clawInfo"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ClawInfo> SetClaw(ClawInfo clawInfo)
        {
            return RobotControllerClient.SetClawAsync(clawInfo);
        }

        /// <summary>
        /// 获取手爪
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ClawInfo> GetClaw()
        {
            return RobotControllerClient.GetClawAsync(new Empty());
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="jPose"></param>
        public virtual async ValueTask SetPos(JPose jPose)
        {
            await RobotControllerClient.SetPosAsync(jPose);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="speedJRequest"></param>
        public virtual async ValueTask SpeedJ(SpeedJRequest speedJRequest)
        {
            await RobotControllerClient.SpeedJAsync(speedJRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="speedLRequest"></param>
        public virtual async ValueTask SpeedL(SpeedLRequest speedLRequest)
        {
            await RobotControllerClient.SpeedLAsync(speedLRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="stopJRequest"></param>
        public virtual async ValueTask StopJ(StopJRequest stopJRequest)
        {
            await RobotControllerClient.StopJAsync(stopJRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="stopLRequest"></param>
        public virtual async ValueTask StopL(StopLRequest stopLRequest)
        {
            await RobotControllerClient.StopLAsync(stopLRequest);
        }

        /// <summary>
        /// 停止当前移动
        /// </summary>
        public virtual async ValueTask StopMove()
        {
            await RobotControllerClient.StopMoveAsync(new Empty());
        }

        /// <summary>
        /// 圆弧移动
        /// </summary>
        /// <param name="moveCRequest"></param>
        public virtual async ValueTask MoveC(MoveCRequest moveCRequest)
        {
            await RobotControllerClient.MoveCAsync(moveCRequest);
        }

        /// <summary>
        /// 关节空间线性移动
        /// </summary>
        /// <param name="moveJRequest"></param>
        public virtual async ValueTask MoveJ(MoveJRequest moveJRequest)
        {
            await RobotControllerClient.MoveJAsync(moveJRequest);
        }

        /// <summary>
        /// 笛卡尔空间线性移动
        /// </summary>
        /// <param name="moveLRequest"></param>
        public virtual async ValueTask MoveL(MoveLRequest moveLRequest)
        {
            await RobotControllerClient.MoveLAsync(moveLRequest);
        }

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        /// <param name="moveLRequest"></param>
        [Obsolete]
        public virtual async ValueTask MoveLJ(MoveLRequest moveLRequest)
        {
            await RobotControllerClient.MoveLJAsync(moveLRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="movePRequest"></param>
        public virtual async ValueTask MoveP(MovePRequest movePRequest)
        {
            await RobotControllerClient.MovePAsync(movePRequest);
        }

        /// <summary>
        /// pt move
        /// </summary>
        /// <param name="pVATRequest"></param>
        public virtual async ValueTask MovePT(PVATRequest pVATRequest)
        {
            await RobotControllerClient.MovePTAsync(pVATRequest);
        }

        /*public void MovePTStream(PVATRequest request)
        {
            
        }*/

        /// <summary>
        /// pvt move
        /// </summary>
        /// <param name="pVATRequest"></param>
        public virtual async ValueTask MovePVT(PVATRequest pVATRequest)
        {
            await RobotControllerClient.MovePVTAsync(pVATRequest);
        }

        /*public void MovePVTStream(PVATRequest request)
        {
            RobotControllerClient.MovePVTStreamAsync(PVATRequest);
        }*/

        /// <summary>
        /// pvat move
        /// </summary>
        /// <param name="pVATRequest"></param>
        public virtual async ValueTask MovePVAT(PVATRequest pVATRequest)
        {
            await RobotControllerClient.MovePVATAsync(pVATRequest);
        }

        /*public void MovePVATStream(stream stream PVATRequest)
        {
            RobotControllerClient.MovePVATStreamAsync(PVATRequest);
        }*/

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="servoCRequest"></param>
        public virtual async ValueTask ServoC(ServoCRequest servoCRequest)
        {
            await RobotControllerClient.ServoCAsync(servoCRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <param name="servoJRequest"></param>
        public virtual async ValueTask ServoJ(ServoJRequest servoJRequest)
        {
            await RobotControllerClient.ServoJAsync(servoJRequest);
        }

        /// <summary>
        /// 获取机器人所有状态数据
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<RobotData> GetRobotData()
        {
            return RobotControllerClient.GetRobotDataAsync(new Empty());
        }

#if NET5_0||NET6_0
        /// <summary>
        /// 获取机器人概要数据流
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<RobotBriefData> GetRobotBriefDataStream()
        {
            return RobotControllerClient.GetRobotBriefData().ResponseStream.ReadAllAsync();
        }

        /// <summary>
        /// 获取机器人的IO数据流
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncEnumerable<IO> GetRobotIODataStream()
        {
            return RobotControllerClient.GetRobotIOData().ResponseStream.ReadAllAsync();
        }

        /// <summary>
        /// 获取机器人的IO数据流
        /// </summary>
        /// <returns></returns>
        public virtual AsyncDuplexStreamingCall<RobotDataCmd, IO> GetRobotIOData()
        {
            return RobotControllerClient.GetRobotIOData();
        }
#endif

        /// <summary>
        /// 获取机器人状态
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<RobotMode> GetRobotMode()
        {
            return RobotControllerClient.GetRobotModeAsync(new Empty());
        }

        /// <summary>
        /// 获得实际关节位置
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> GetActualJointPositions()
        {
            return RobotControllerClient.GetActualJointPositionsAsync(new Empty());
        }

        /// <summary>
        /// 获得目标关节位置
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> GetTargetJointPositions()
        {
            return RobotControllerClient.GetTargetJointPositionsAsync(new Empty());
        }

        /// <summary>
        /// 获得实际关节速度
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> GetActualJointSpeeds()
        {
            return RobotControllerClient.GetActualJointSpeedsAsync(new Empty());
        }

        /// <summary>
        /// 获得目标关节速度
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> GetTargetJointSpeeds()
        {
            return RobotControllerClient.GetTargetJointSpeedsAsync(new Empty());
        }

        /// <summary>
        /// 获得末端在笛卡尔坐标系下的位姿
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> GetActualTcpPose()
        {
            return RobotControllerClient.GetActualTcpPoseAsync(new Empty());
        }

        /// <summary>
        /// 获得末端在笛卡尔坐标系下的目标位姿
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> GetTargetTcpPose()
        {
            return RobotControllerClient.GetTargetTcpPoseAsync(new Empty());
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> GetActualTcpSpeed()
        {
            return RobotControllerClient.GetActualTcpSpeedAsync(new Empty());
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> GetTargetTcpSpeed()
        {
            return RobotControllerClient.GetTargetTcpSpeedAsync(new Empty());
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> GetActualFlangePose()
        {
            return RobotControllerClient.GetActualFlangePoseAsync(new Empty());
        }

        /// <summary>
        /// 获取关节扭矩
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> GetJointTorques()
        {
            return RobotControllerClient.GetJointTorquesAsync(new Empty());
        }

        /// <summary>
        /// 获取控制器温度
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Temperature> GetControllerTemp()
        {
            return RobotControllerClient.GetControllerTempAsync(new Empty());
        }

        /// <summary>
        /// 获取关节内部温度
        /// </summary>
        /// <param name="intRequest"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Temperature> GetJointTemp(IntRequest intRequest)
        {
            return RobotControllerClient.GetJointTempAsync(intRequest);
        }

        /// <summary>
        /// implement later
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Current> GetToolCurrent()
        {
            return RobotControllerClient.GetToolCurrentAsync(new Empty());
        }

        /// <summary>
        /// 设置数字输出端口的值
        /// </summary>
        /// <param name="dIO"></param>
        public virtual async ValueTask SetDIO(DIO dIO)
        {
            await RobotControllerClient.SetDIOAsync(dIO);
        }

        /// <summary>
        /// 设置扩展数字输出端口的值
        /// </summary>
        /// <param name="dIO"></param>
        public virtual async ValueTask SetExtraDIO(DIO dIO)
        {
            await RobotControllerClient.SetExtraDIOAsync(dIO);
        }

        /// <summary>
        /// 获得数字输入端口的值
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DIO> GetDIO(IOPin iOPin)
        {
            return RobotControllerClient.GetDIOAsync(iOPin);
        }

        /// <summary>
        /// 获得扩展数字数如端口的值
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DIO> GetExtraDIO(IOPin iOPin)
        {
            return RobotControllerClient.GetExtraDIOAsync(iOPin);
        }

        /// <summary>
        /// 设置TCP数字输出端口的值
        /// </summary>
        /// <param name="dIO"></param>
        public virtual async ValueTask SetTcpDIO(DIO dIO)
        {
            await RobotControllerClient.SetTcpDIOAsync(dIO);
        }

        /// <summary>
        /// 获得TCP数字输入端口的值
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DIO> GetTcpDIO(IOPin iOPin)
        {
            return RobotControllerClient.GetTcpDIOAsync(iOPin);
        }

        /// <summary>
        /// 设置模拟输出端口的值
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetAIO(AIO aIO)
        {
            await RobotControllerClient.SetAIOAsync(aIO);
        }

        /// <summary>
        /// 获得模拟输入端口的值
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetExtraAIO(AIO aIO)
        {
            await RobotControllerClient.SetExtraAIOAsync(aIO);
        }

        /// <summary>
        /// 获得模拟输入端口的值
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetAIO(IOPin iOPin)
        {
            return RobotControllerClient.GetAIOAsync(iOPin);
        }

        /// <summary>
        /// 获得扩展模拟输入端口的值
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetExtraAIO(IOPin iOPin)
        {
            return RobotControllerClient.GetExtraAIOAsync(iOPin);
        }

        /// <summary>
        /// 设置模拟输入端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetAInMode(AIO aIO)
        {
            await RobotControllerClient.SetAInModeAsync(aIO);
        }

        /// <summary>
        /// 设置扩展模拟输入端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetExtraAInMode(AIO aIO)
        {
            await RobotControllerClient.SetExtraAInModeAsync(aIO);
        }

        /// <summary>
        /// 获得模拟输入端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetAInMode(IOPin iOPin)
        {
            return RobotControllerClient.GetAInModeAsync(iOPin);
        }

        /// <summary>
        /// 获得扩展模拟输入端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetExtraAInMode(IOPin iOPin)
        {
            return RobotControllerClient.GetExtraAInModeAsync(iOPin);
        }

        /// <summary>
        /// 设置模拟输出端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetAOutMode(AIO aIO)
        {
            await RobotControllerClient.SetAOutModeAsync(aIO);
        }

        /// <summary>
        /// 设置扩展模拟输出端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="aIO"></param>
        public virtual async ValueTask SetExtraAOutMode(AIO aIO)
        {
            await RobotControllerClient.SetExtraAOutModeAsync(aIO);
        }

        /// <summary>
        /// 获得模拟输出端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetAOutMode(IOPin iOPin)
        {
            return RobotControllerClient.GetAOutModeAsync(iOPin);
        }

        /// <summary>
        /// 获得扩展模拟输出端口工作模式：0:电压，1:电流
        /// </summary>
        /// <param name="iOPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<AIO> GetExtraAOutMode(IOPin iOPin)
        {
            return RobotControllerClient.GetExtraAOutModeAsync(iOPin);
        }

        /// <summary>
        /// 开启/启动系统
        /// </summary>
        public virtual async ValueTask StartSys()
        {
            await RobotControllerClient.StartSysAsync(new Empty());
        }

        /// <summary>
        /// 关闭/停止系统
        /// </summary>
        public virtual async ValueTask StopSys()
        {
            await RobotControllerClient.StopSysAsync(new Empty());
        }

        /// <summary>
        /// 程序停止
        /// </summary>
        public virtual async ValueTask Stop()
        {
            await RobotControllerClient.StopAsync(new Empty());
        }

        /// <summary>
        /// 急停
        /// </summary>
        public virtual async ValueTask EStop()
        {
            await RobotControllerClient.EStopAsync(new Empty());
        }

        /// <summary>
        /// 获取kdl参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<KDParam> GetKDL()
        {
            return RobotControllerClient.GetKDLAsync(new Empty());
        }

        /// <summary>
        /// 查询系统里面的日志信息
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Logs> GetLogs()
        {
            return RobotControllerClient.GetLogsAsync(new Empty());
        }

        /// <summary>
        /// 获得当前正在执行的命令id，如果没有在执行的命令，则返回-1
        /// </summary>
        public virtual async ValueTask GetCurrentCmd()
        {
            await RobotControllerClient.GetCurrentCmdAsync(new Empty());
        }

        /*
        // 获得指定命令id的执行结果：-1: 未执行；0: 已执行
        public CmdStatus GetCmdExecStatus(void cmdId)
        {
            return nt.GetCmdExecStatus(cmdId);
        }*/

        /// <summary>
        /// 开始微调: 如果当前有其他微调再传入新的微调命令会终止当前的微调进行新的微调
        /// </summary>
        /// <param name="fineTuning"></param>
        public virtual async ValueTask StartFineTuning(FineTuning fineTuning)
        {
            await RobotControllerClient.StartFineTuningAsync(fineTuning);
        }

        /// <summary>
        /// 停止微调
        /// </summary>
        public virtual async ValueTask StopFineTuning()
        {
            await RobotControllerClient.StopFineTuningAsync(new Empty());
        }

        /// <summary>
        /// 暂停机器人
        /// </summary>
        public virtual async ValueTask Pause()
        {
            await RobotControllerClient.PauseAsync(new Empty());
        }

        /// <summary>
        /// 恢复机器人
        /// </summary>
        public virtual async ValueTask Resume()
        {
            await RobotControllerClient.ResumeAsync(new Empty());
        }

        /// <summary>
        /// 机器人正解
        /// </summary>
        /// <param name="joint"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> KinematicsForward(Joint joint)
        {
            return RobotControllerClient.KinematicsForwardAsync(joint);
        }

        /// <summary>
        /// 机器人反解
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Joint> KinematicsInverse(Vector vector)
        {
            return RobotControllerClient.KinematicsInverseAsync(vector);
        }

        /// <summary>
        /// TCP示教添加
        /// </summary>
        /// <param name="calcTcpParam"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Vector> CalcTcpTranslation(CalcTcpParam calcTcpParam)
        {
            return RobotControllerClient.CalcTcpTranslationAsync(calcTcpParam);
        }

        /// <summary>
        /// 测试命令，以给定的RPY数据执行线性移动
        /// </summary>
        /// <param name="moveLRPYRequest"></param>
        public virtual async ValueTask MoveLRPY(MoveLRPYRequest moveLRPYRequest)
        {
            await RobotControllerClient.MoveLRPYAsync(moveLRPYRequest);
        }

        /// <summary>
        /// 设置LED灯状态
        /// </summary>
        /// <param name="lEDStatus"></param>
        public virtual async ValueTask SetLED(LEDStatus lEDStatus)
        {
            await RobotControllerClient.SetLEDAsync(lEDStatus);
        }

        /// <summary>
        /// 设置声音
        /// </summary>
        /// <param name="voiceStatus"></param>
        public virtual async ValueTask SetVoice(VoiceStatus voiceStatus)
        {
            await RobotControllerClient.SetVoiceAsync(voiceStatus);
        }

        /// <summary>
        /// 设置风扇
        /// </summary>
        /// <param name="fanStatus"></param>
        public virtual async ValueTask SetFan(FanStatus fanStatus)
        {
            await RobotControllerClient.SetFanAsync(fanStatus);
        }

        /// <summary>
        /// 获取灯板状态
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<LampStatus> GetLampStatus()
        {
            return RobotControllerClient.GetLampStatusAsync(new Empty());
        }

        /// <summary>
        /// Lua 状态查询
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<LuaStatus> GetLuaState()
        {
            return RobotControllerClient.GetLuaStateAsync(new Empty());
        }

        /// <summary>
        /// 设置外置数字输出
        /// </summary>
        /// <param name="externalDigital"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetExternalDO(ExternalDigital externalDigital)
        {
            return RobotControllerClient.SetExternalDOAsync(externalDigital);
        }

        /// <summary>
        /// 获取外置数字输出
        /// </summary>
        /// <param name="externalPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalDigital> GetExternalDO(ExternalPin externalPin)
        {
            return RobotControllerClient.GetExternalDOAsync(externalPin);
        }

        /// <summary>
        /// 获取外置数字输入
        /// </summary>
        /// <param name="externalPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalDigital> GetExternalDI(ExternalPin externalPin)
        {
            return RobotControllerClient.GetExternalDIAsync(externalPin);
        }

        /// <summary>
        /// 设置外置模拟输出
        /// </summary>
        /// <param name="externalAnalog"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetExternalAO(ExternalAnalog externalAnalog)
        {
            return RobotControllerClient.SetExternalAOAsync(externalAnalog);
        }

        /// <summary>
        /// 获取外置模拟输出
        /// </summary>
        /// <param name="externalPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalAnalog> GetExternalAO(ExternalPin externalPin)
        {
            return RobotControllerClient.GetExternalAOAsync(externalPin);
        }

        /// <summary>
        /// 获取外置模拟输入
        /// </summary>
        /// <param name="externalPin"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalAnalog> GetExternalAI(ExternalPin externalPin)
        {
            return RobotControllerClient.GetExternalAIAsync(externalPin);
        }

        /// <summary>
        /// 获取某个外置io的全部io信息
        /// </summary>
        /// <param name="externalDevice"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalIOs> GetExternalIOs(ExternalDevice externalDevice)
        {
            return RobotControllerClient.GetExternalIOsAsync(externalDevice);
        }

        /// <summary>
        /// 设置某个外置io的全部io信息
        /// </summary>
        /// <param name="externalDigitals"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetExternalDOs(ExternalDigitals externalDigitals)
        {
            return RobotControllerClient.SetExternalDOsAsync(externalDigitals);
        }

        /// <summary>
        /// 获取外置数字输出
        /// </summary>
        /// <param name="externalPins"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalDigitals> GetExternalDOs(ExternalPins externalPins)
        {
            return RobotControllerClient.GetExternalDOsAsync(externalPins);
        }

        /// <summary>
        /// 获取外置数字输入
        /// </summary>
        /// <param name="externalPins"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalDigitals> GetExternalDIs(ExternalPins externalPins)
        {
            return RobotControllerClient.GetExternalDIsAsync(externalPins);
        }

        /// <summary>
        /// 设置外置模拟输出
        /// </summary>
        /// <param name="externalAnalogs"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetExternalAOs(ExternalAnalogs externalAnalogs)
        {
            return RobotControllerClient.SetExternalAOsAsync(externalAnalogs);
        }

        /// <summary>
        /// 获取外置模拟输出
        /// </summary>
        /// <param name="externalPins"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalAnalogs> GetExternalAOs(ExternalPins externalPins)
        {
            return RobotControllerClient.GetExternalAOsAsync(externalPins);
        }

        /// <summary>
        /// 获取外置模拟输入
        /// </summary>
        /// <param name="externalPins"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExternalAnalogs> GetExternalAIs(ExternalPins externalPins)
        {
            return RobotControllerClient.GetExternalAIsAsync(externalPins);
        }

        /// <summary>
        /// 设置信号量
        /// </summary>
        /// <param name="signalValue"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<SignalResult> SetSignal(SignalValue signalValue)
        {
            return RobotControllerClient.SetSignalAsync(signalValue);
        }

        /// <summary>
        /// 获取信号量
        /// </summary>
        /// <param name="signalValue"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<SignalResult> GetSignal(SignalValue signalValue)
        {
            return RobotControllerClient.GetSignalAsync(signalValue);
        }

        /// <summary>
        /// 添加信号量
        /// </summary>
        /// <param name="signalValue"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<SignalResult> AddSignal(SignalValue signalValue)
        {
            return RobotControllerClient.AddSignalAsync(signalValue);
        }
        /*

        public stream RegisterSignals(stream stream SignalList)
        {
            return RobotControllerClient.RegisterSignals().RequestStream.WriteAsync();
        }*/


        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="configuration"></param>
        public virtual async void Init(Configuration configuration)
        {
            await RobotPrivateControllerClient.InitAsync(configuration);
        }

        /// <summary>
        /// 获取机器人基础信息
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<RobotInfo> GetRobotInfo()
        {
            return RobotPrivateControllerClient.GetRobotInfoAsync(new Empty());
        }

        /// <summary>
        /// 设置机器人安装方向
        /// </summary>
        /// <param name="installDirection"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetInstallDirection(InstallDirection installDirection)
        {
            return RobotPrivateControllerClient.SetInstallDirectionAsync(installDirection);
        }

        /// <summary>
        /// 设置碰撞检测
        /// </summary>
        /// <param name="collisionDetector"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetCollisionDetector(CollisionDetector collisionDetector)
        {
            return RobotPrivateControllerClient.SetCollisionDetectorAsync(collisionDetector);
        }

        /// <summary>
        /// 设置关节配置
        /// </summary>
        /// <param name="jointConfigs"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetJointConfig(JointConfigs jointConfigs)
        {
            return RobotPrivateControllerClient.SetJointConfigAsync(jointConfigs);
        }

        /// <summary>
        /// 设置笛卡尔空间的配置
        /// </summary>
        /// <param name="cartesianConfig"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetCartesianConfig(CartesianConfig cartesianConfig)
        {
            return RobotPrivateControllerClient.SetCartesianConfigAsync(cartesianConfig);
        }

        /// <summary>
        /// 开启DDS
        /// </summary>
        /// <param name="trueOrFalse"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> EnableDDS(TrueOrFalse trueOrFalse)
        {
            return RobotPrivateControllerClient.EnableDDSAsync(trueOrFalse);
        }

        /// <summary>
        /// 设置碰撞检测力矩差阈值
        /// </summary>
        /// <param name="collisionTorqueDiff"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetCollisionTorqueDiff(CollisionTorqueDiff collisionTorqueDiff)
        {
            return RobotPrivateControllerClient.SetCollisionTorqueDiffAsync(collisionTorqueDiff);
        }

        // 注册通知事件
        /*public virtual async stream Notification RegisterNotification(google.protobuf.Empty){
        await RobotPrivateControllerClient.RegisterNotification(new Empty());
        }*/
        public virtual AsyncUnaryCall<DriverInfo> RobotDriverInfo()
        {
            return RobotPrivateControllerClient.RobotDriverInfoAsync(new Empty());
        }

        /*// 机器人OTA单个设备更新接口
        public virtual async stream OTAResult RobotOTA(OTAData){
        await RobotPrivateControllerClient.RobotOTAAsync();
        }*/

        /// <summary>
        /// 通知灯板、法兰、关节切换分区
        /// </summary>
        /// <param name="otaCmd"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SwitchOtaPartition(OTACmd otaCmd)
        {
            return RobotPrivateControllerClient.SwitchOtaPartitionAsync(otaCmd);
        }

        /*// 机器人OTA批量更新接口
        public virtual async stream OTAResults RobotOTABatch(OTADatas){
        await RobotPrivateControllerClient.RobotOTABatchAsync();
        }*/

        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> Reset()
        {
            return RobotPrivateControllerClient.ResetAsync(new Empty());
        }

        /// <summary>
        /// 以给定角度置零
        /// </summary>
        /// <param name="zero"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> InitZero(Zero zero)
        {
            return RobotPrivateControllerClient.InitZeroAsync(zero);
        }

        /// <summary>
        /// 以零位置零
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetZero()
        {
            return RobotPrivateControllerClient.SetZeroAsync(new Empty());
        }

        /// <summary>
        /// 获取机器人电压V
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<CurrentVoltage> GetVoltage()
        {
            return RobotPrivateControllerClient.GetVoltageAsync(new Empty());
        }

        /// <summary>
        /// 设置单关节伺服参数
        /// </summary>
        /// <param name="jointServoParam"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointServoParams> SetServoParam(JointServoParam jointServoParam)
        {
            return RobotPrivateControllerClient.SetServoParamAsync(jointServoParam);
        }

        /// <summary>
        /// 获取当前所有关节伺服参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointServoParams> GetServoParams()
        {
            return RobotPrivateControllerClient.GetServoParamsAsync(new Empty());
        }

        /// <summary>
        /// 调试设置
        /// </summary>
        /// <param name="debugParams"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DebugParams> SetDebugParams(DebugParams debugParams)
        {
            return RobotPrivateControllerClient.SetDebugParamsAsync(debugParams);
        }

        /// <summary>
        /// 更改DH参数（三轴平行6参数）
        /// </summary>
        /// <param name="fixDhRequest"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<FixDHResult> FixDHParams(FixDHRequest fixDhRequest)
        {
            return RobotPrivateControllerClient.FixDHParamsAsync(fixDhRequest);
        }

        /// <summary>
        /// 设置LED样式
        /// </summary>
        /// <param name="ledStyle"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<LEDStyles> SetLEDStyle(LEDStyle ledStyle)
        {
            return RobotPrivateControllerClient.SetLEDStyleAsync(ledStyle);
        }

        /// <summary>
        /// 获取LED样式
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<LEDStyles> GetLEDStyles()
        {
            return RobotPrivateControllerClient.GetLEDStylesAsync(new Empty());
        }

        /*// 注册命令状态事件
        public virtual async stream LuaEvent RegisterLuaEvent(){
        await RobotPrivateControllerClient.RegisterLuaEventAsync(new Empty);
        }*/

        /// <summary>
        /// 当推送 ALERT/CONFIRM/INPUT/SELECT，用户在前端确定后调用该接口
        /// </summary>
        /// <param name="confirmInput"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> ConfirmCallback(ConfirmInput confirmInput)
        {
            return RobotPrivateControllerClient.ConfirmCallbackAsync(confirmInput);
        }

        /// <summary>
        /// 获取 Lua 上次执行到的机器人位置
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<PoseRes> GetLastPose()
        {
            return RobotPrivateControllerClient.GetLastPoseAsync(new Empty());
        }

        /// <summary>
        /// 配置Modbus外部IO设备
        /// </summary>
        /// <param name="modbusExternalIOs"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetModbusExternalIO(ModbusExternalIOs modbusExternalIOs)
        {
            return RobotPrivateControllerClient.SetModbusExternalIOAsync(modbusExternalIOs);
        }

        /// <summary>
        /// 修改按钮配置
        /// </summary>
        /// <param name="buttonConfig"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetButtonConfig(ButtonConfig buttonConfig)
        {
            return RobotPrivateControllerClient.SetButtonConfigAsync(buttonConfig);
        }

        /// <summary>
        /// 设置绑定设备开关, true: 不限制设备绑定； false：限制设备绑定逻辑
        /// </summary>
        /// <param name="trueOrFalse"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SetBreakACup(TrueOrFalse trueOrFalse)
        {
            return RobotPrivateControllerClient.SetBreakACupAsync(trueOrFalse);
        }

        /*// PVAT数据记录接口，用户记录pvat数据
        public virtual async stream RecordPVATResponse RecordPVAT(RecordPVATRequest){
        await RobotPrivateControllerClient.RecordPVATAsync();
        }*/


        /// <summary>
        /// 停止记录pvat数据
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> StopRecordPVAT()
        {
            return RobotPrivateControllerClient.StopRecordPVATAsync(new Empty());
        }

        /*// 语音升级
        public virtual async stream VoiceResult);//yvoi UpgradeVoiceFile(VoiceFile){
        await RobotPrivateControllerClient.UpgradeVoiceFileAsync();
        }*/

        /// <summary>
        /// 获取当前 DH 参数
        /// </summary>
        /// <param name="dhRequest"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DHParams> GetDHParams(DHRequest dhRequest)
        {
            return RobotPrivateControllerClient.GetDHParamsAsync(dhRequest);
        }

        /// <summary>
        /// 设置 DH 参数并返回设置后的结果
        /// </summary>
        /// <param name="dhParams"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<DHParams> SetDHParams(DHParams dhParams)
        {
            return RobotPrivateControllerClient.SetDHParamsAsync(dhParams);
        }

        /// <summary>
        /// 写伺服控制参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExtraServoParam> WriteExtraServoParam(ExtraServoParam param)
        {
            return RobotPrivateControllerClient.WriteExtraServoParamAsync(param);
        }

        /// <summary>
        /// 读取伺服控制参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExtraServoParam> ReadExtraServoParam(ExtraServoParam param)
        {
            return RobotPrivateControllerClient.ReadExtraServoParamAsync(param);
        }

        /// <summary>
        /// 写多个伺服控制参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExtraServoParams> WriteExtraServoParams(ExtraServoParam param)
        {
            return RobotPrivateControllerClient.WriteExtraServoParamsAsync(param);
        }

        /// <summary>
        /// 读取多个伺服控制参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExtraServoParams> ReadExtraServoParams()
        {
            return RobotPrivateControllerClient.ReadExtraServoParamsAsync(new Empty());
        }

        /// <summary>
        /// 重置伺服控制参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<ExtraServoParams> ResetExtraServoParams()
        {
            return RobotPrivateControllerClient.ResetExtraServoParamsAsync(new Empty());
        }

        /// <summary>
        /// 写“主动消回差”参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklash> WriteJointBacklash(JointBacklash param)
        {
            return RobotPrivateControllerClient.WriteJointBacklashAsync(param);
        }

        /// <summary>
        /// 读取“主动消回差”参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklash> ReadJointBacklash(JointBacklash param)
        {
            return RobotPrivateControllerClient.ReadJointBacklashAsync(param);
        }

        /// <summary>
        /// 写“主动消回差”参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashes> WriteJointBacklashes(JointBacklash param)
        {
            return RobotPrivateControllerClient.WriteJointBacklashesAsync(param);
        }

        /// <summary>
        /// 读取多个“主动消回差”参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashes> ReadJointBacklashes()
        {
            return RobotPrivateControllerClient.ReadJointBacklashesAsync(new Empty());
        }

        /// <summary>
        /// 重置“主动消回差”参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashes> ResetJointBacklashes()
        {
            return RobotPrivateControllerClient.ResetJointBacklashesAsync(new Empty());
        }

        /// <summary>
        /// 启用主动消回差
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<EnableJointBacklashes> WriteEnableJointBacklashes(EnableJointBacklash param)
        {
            return RobotPrivateControllerClient.WriteEnableJointBacklashesAsync(param);
        }

        /// <summary>
        /// 是否启用主动消回差
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<EnableJointBacklashes> ReadEnableJointBacklashes()
        {
            return RobotPrivateControllerClient.ReadEnableJointBacklashesAsync(new Empty());
        }

        /// <summary>
        /// 重置主动消回差
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<EnableJointBacklashes> ResetEnableJointBacklashes()
        {
            return RobotPrivateControllerClient.ResetEnableJointBacklashesAsync(new Empty());
        }

        /// <summary>
        /// 写关节回差参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashParam> WriteJointBacklashParam(JointBacklashParam param)
        {
            return RobotPrivateControllerClient.WriteJointBacklashParamAsync(param);
        }

        /// <summary>
        /// 读取关节回差参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashParam> ReadJointBacklashParam(JointBacklashParam param)
        {
            return RobotPrivateControllerClient.ReadJointBacklashParamAsync(param);
        }

        /// <summary>
        /// 写多个关节回差参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashParams> WriteJointBacklashParams(JointBacklashParam param)
        {
            return RobotPrivateControllerClient.WriteJointBacklashParamsAsync(param);
        }

        /// <summary>
        /// 读多个关节回差参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashParams> ReadJointBacklashParams()
        {
            return RobotPrivateControllerClient.ReadJointBacklashParamsAsync(new Empty());
        }

        /// <summary>
        /// 重置关节回差参数
        /// </summary>
        /// <returns></returns>
        public virtual AsyncUnaryCall<JointBacklashParams> ResetJointBacklashParams()
        {
            return RobotPrivateControllerClient.ResetJointBacklashParamsAsync(new Empty());
        }

        /// <summary>
        /// 启用关节限位检测
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> EnableJointLimit(TrueOrFalse param)
        {
            return RobotPrivateControllerClient.EnableJointLimitAsync(param);
        }

        /// <summary>
        /// 切换模拟环境
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual AsyncUnaryCall<Response> SwitchSimulate(TrueOrFalse param)
        {
            return RobotPrivateControllerClient.SwitchSimulateAsync(param);
        }
        /*// 连接/断开 MODBUS 设备
        public virtual AsyncUnaryCall<Response> ConnectExternalIO(ExternalIOState param){
        return RobotPrivateControllerClient.ConnectExternalIOAsync(param);
        }*/
    }
}