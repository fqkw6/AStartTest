using UnityEngine;
using System.Collections;
using message;

public partial class BattleController
{
    // 连接战斗服务器
    public void ConnectToBattleServer()
    {
        BattleNetworkManager.Instance.Connect(ServerIP, 9527, (state) => {
            if (state == ConnectState.CONNECTED) {
                Log.Info("登录战斗服务器成功");

                // 连接成功，开始登陆，以便获取玩家ID
                ReqConnect();
            } else {
                Log.Error("登录战斗服务器失败: " + state);
            }
        });
    }

    public void RegisterMsg()
    {
        // 战斗服务器
        Net.Register(MSG.CONNECT, OnMsgConnect);
        Net.Register(MSG.REQ_BATTLE, OnMsgReqBattle);
        Net.Register(MSG.LOGIN, OnMsgLogin);

        Net.Register(MSG.GM_COMMAND, OnMsgGmCommand);
        Net.Register(MSG.START_BATTLE, OnMsgStartBattle);
        Net.Register(MSG.RESUME_BATTLE, OnMsgResumeBattle);
        Net.Register(MSG.FINISH_BATTLE, OnMsgFinishBattle);
        Net.Register(MSG.SYN_TIMESTAMP, OnSynTimestamp);
        Net.Register(MSG.SYN_ACTION, OnMsgSynAction);
        Net.Register(MSG.SYN_FULL, OnMsgSynFull);
        Net.Register(MSG.SYN_POS, OnMsgSynPos);
        Net.Register(MSG.SYN_MOVE_TARGET, OnMsgSynMoveTarget);
        Net.Register(MSG.SYN_AI_TARGET, OnMsgSynAiTarget);
        Net.Register(MSG.SYN_ATTACK, OnMsgSynAttack);
        Net.Register(MSG.SYN_SKILL, OnMsgSynSkill);
        Net.Register(MSG.SYN_DEAD, OnMsgSynDead);
    }

    // 战斗服务器的消息
    public void ReqConnect()
    {
        G2B_Connect data = new G2B_Connect();
        data.Psw = "";

        Net.Send(MSG.CONNECT, data);
    }

    private void OnMsgConnect(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

        B2G_Connect ret = Net.Deserialize<B2G_Connect>(rpc.MsgData);
        UserID = ret.UserID;

        // 登陆成功，直接开启战斗 TODO 这个流程是测试流程，将来这个要游戏服务器请求
        ReqBattle();
    }

    // 请求开始战斗(将来这个是游戏服务器做的)
    public void ReqBattle()
    {
        G2B_ReqBattle data = new G2B_ReqBattle();
        data.UserID1 = UserID;
        data.UserLevel1 = UserLevel;
        data.UserID2 = 0;   // 现阶段做一个兼容，战斗服务器会自动匹配可战斗的房间进行战斗
        data.UserLevel2 = 0;

        Net.Send(MSG.REQ_BATTLE, data);
    }

    // 战斗服务器返回一个战斗房间
    private void OnMsgReqBattle(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

        B2G_ReqBattle ret = Net.Deserialize<B2G_ReqBattle>(rpc.MsgData);
        Token = ret.Token;

        Log.Info("战斗Token: " + Token);

        // TODO 暂时直接登录
        ReqLogin();
    }

    // 请求登录战斗服务器
    public void ReqLogin()
    {
        C2S_Login data = new C2S_Login();
        data.Token = Token;
        data.UserID = UserID;

        Net.Send(MSG.LOGIN, data);
    }

    // 成功登陆战斗服务器中的相应房间
    private void OnMsgLogin(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

        // 已经加入对应房间，随时可以开始战斗
    }

    // GM命令，添加机器人
    public void ReqAddBot()
    {
        DataString data = new DataString();
        data.Param = string.Format("addbot");
        Net.Send(MSG.GM_COMMAND, data);
    }

    // GM命令
    private void OnMsgGmCommand(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 战斗开始(两个人都加入对应房间后，开启战斗，战斗服务器直接通知)
    private void OnMsgStartBattle(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

        S2C_StartBattle ret = Net.Deserialize<S2C_StartBattle>(rpc.MsgData);
        OnStartBattle(ret);
    }

    // 战斗恢复（离线再重登陆，战斗服务器直接通知）
    private void OnMsgResumeBattle(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 结束战斗(客户端通知战斗服务器战斗结束--结果在客户端判定  TODO 做服务器校验和服务器判定)
    private void OnMsgFinishBattle(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;
    }

    // 同步时间戳(客户端只允许执行到服务器通知到的时间帧，不允许超前执行)
    private void OnSynTimestamp(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;
        SynTimestamp ret = Net.Deserialize<SynTimestamp>(rpc.MsgData);

        // 记录服务器时间，客户端可以执行到服务器确认过的turn
        ServerTurnIndex = ret.TurnIndex;
        ServerTimestamp = ret.Timestamp;
    }

    // 请求操作（主要是出卡，ponit传屏幕坐标，跟服务器通信转换为cell坐标）
    public void ReqSynAction(BattleAction action, int cardID, int level, Vector2 cellPos)
    {
        int x = (int)cellPos.x;
        int y = (int)cellPos.y;
        SynAction data = new SynAction();
        data.Action = action;
        data.UserID = UserID;
        data.CardID = cardID;
        data.Level = level;
        data.PosX = x;
        data.PosY = y;
        Net.Send(MSG.SYN_ACTION, data);
    }

    // 同步操作（对于发起操作的客户端而言，服务器是确认出卡有效，预览模型转换为部署状态。对对方客户端而言，是通知出兵，直接部署）
    private void OnMsgSynAction(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;
        SynAction ret = Net.Deserialize<SynAction>(rpc.MsgData);

        // 将操作加入集合中，注意不要立即处理，而是等到对应的turn再处理
        _actionQueue.Enqueue(ret);
    }

    // 全状态同步
    private void OnMsgSynFull(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 同步坐标
    private void OnMsgSynPos(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 同步移动目的地
    private void OnMsgSynMoveTarget(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 同步移动目标
    private void OnMsgSynAiTarget(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 同步攻击
    private void OnMsgSynAttack(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    //同步技能
    private void OnMsgSynSkill(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }

    // 同步单位死亡
    private void OnMsgSynDead(RPCResponse rpc)
    {
        if (rpc.ErrCode != ErrorCode.OK) return;

    }
}