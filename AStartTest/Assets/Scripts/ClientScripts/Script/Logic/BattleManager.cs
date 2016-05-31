using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;
using DG.Tweening.Plugins;
using message;
using UnityEngine.SceneManagement;

public enum LogicBattleType
{
    PVE,    // 副本战斗
    PVP,    // 竞技场战斗
    WORLD,  // 世界地图（攻城战）

}

// 战斗管理，负责与服务器和其他业务逻辑交互(旧业务逻辑使用，新的战斗模块是BattleController)
public class BattleManager
{
    public static readonly BattleManager Instance = new BattleManager();
    
    public long BattleID = 0;
    public LogicBattleType BattleType;   // 当前战斗的类型

    public string OpenedWindow = "";    // 结束战斗回到主界面要打开的界面
    
    // 开始战斗，具体流程交给战斗系统负责
    public void StartBattle(long battleID, LogicBattleType battleType)
    {
        UIUtil.ShowMsgFormat("MSG_BATTLE_START");
        Log.Info("BattleStart: " + battleID);
        BattleID = battleID;
        BattleType = battleType;

        // 异步操作，防止切场景时造成崩溃
        Timer.AsyncCall(() =>
        {
            // 关闭所有界面
            UIManager.Instance.OnChangeScene();

            // 切换到战斗状态(TODO 游戏状态这样写维护有些乱，将来重构整理)
            Game.Instance.EnterBattle();

            // 切换场景
            AsyncOperation ap = SceneManager.LoadSceneAsync("Scene02", LoadSceneMode.Additive);
            UIManager.Instance.OpenWindow<UIBattleView>();
            UIManager.Instance.OpenWindow<UILoadingSceneView>(ap);
        });
    }

    // 战斗结束，玩家确认奖励，跳转到主场景
    public void ChangeToMainScene(string openedWindow = null)
    {
        OpenedWindow = openedWindow;

        // 异步操作，防止切场景时造成崩溃
        Timer.AsyncCall(() =>
        {
            // 关闭所有界面
            UIManager.Instance.OnChangeScene();

            Game.Instance.BackFromBattle = true;

            // 切换场景， TODO 单独一个loading界面
            SceneManager.UnloadScene("Scene02");
            Game.Instance.Init();

            if (!string.IsNullOrEmpty(OpenedWindow)) {
                UIManager.Instance.OpenWindow(OpenedWindow);
            }
            // AsyncOperation ap = SceneManager.LoadSceneAsync("GameMain", LoadSceneMode.Additive);
            // UIManager.Instance.OpenWindow("Common/UILoadingSceneView", ap);
        });
    }

    // 处理战报
    public void ProcessBattleReport(PBattleReport report)
    {
        switch (BattleType) {
            case LogicBattleType.PVE:
                PVEManager.Instance.OnBattleResult(report);
                break;
            case LogicBattleType.PVP:
                PVPManager.Instance.OnBattleResult(report);
                break;
            case LogicBattleType.WORLD:
                WorldManager.Instance.OnBattleResult(report);
                break;
        }
    }

    // 战斗系统返回战斗结果
    public void OnBattleFinish(bool isWin)
    {
        if (BattleID == 0) {
            //Log.Error("错误的战斗ID: " + BattleID);
            return;
        }

        RequestGetBattleResult(BattleID, isWin);
    }

    public void RequestGetBattleResult(long battleID, bool isWin)
    {
        PGetBattleResult data = new PGetBattleResult();
        data.battleId = battleID;
        data.btResult = isWin ? eBattleResult.BTR_WIN : eBattleResult.BTR_FAIL;
        Net.Send(eCommand.GET_BATTLE_RESULT, data, (buffer) =>
        {
            PBattleReport ret = Net.Deserialize<PBattleReport>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_BATTLE_RESULT)) return;

            ProcessBattleReport(ret);

            BattleID = 0;
        });
    }
}
