using UnityEngine;
using System.Collections;
using comrt.comnet;

// 测试用的命令
public class GMManager
{
    public static readonly GMManager Instance = new GMManager();

    public void RegisterMsg()
    {
        Net.Register(eCommand.CREATE_HERO, OnMsgCreateHero);
        Net.Register(eCommand.ADJUST_PLAYER_LEVEL, OnMsgAdjustPlayerLevel);
        Net.Register(eCommand.SYS_ADD_GOOD, OnMsgSysAddGood);
    }

    public void RequestCreateHero(int heroConfigID)
    {
        PCMInt data = new PCMInt();
        data.arg = heroConfigID;
        Net.Send(eCommand.CREATE_HERO, data);
    }

    public void OnMsgCreateHero(byte[] buffer)
    {
        PHeroAttr ret = Net.Deserialize<PHeroAttr>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.CREATE_HERO)) return;

        HeroInfo info = new HeroInfo();
        info.Deserialize(ret, false);
        UserManager.Instance.HeroList.Add(info);

        Log.Info("添加英雄：" + ret.heroId);
    }

    public void RequestPlayerUpLevel(int exp)
    {
        PCMInt data = new PCMInt();
        data.arg = exp;
        Net.Send(eCommand.ADJUST_PLAYER_LEVEL, data);
    }

    private void OnMsgAdjustPlayerLevel(byte[] buffer)
    {
        PRoleAttr data = Net.Deserialize<PRoleAttr>(buffer);
        UserManager.Instance.Deserialize(data);
    }

    public void RequestAddGood(int itemCfgID, int count)
    {
        PCMInt2 data = new PCMInt2();
        data.arg = itemCfgID;
        data.arg2 = count;
        Net.Send(eCommand.SYS_ADD_GOOD, data);

    }
    private void OnMsgSysAddGood(byte[] buffer)
    {
        PComItemList ret = Net.Deserialize<PComItemList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.SYS_ADD_GOOD)) return;
        
        UserManager.Instance.AddItem(ret, false);

        // 刷新ui
        UIManager.Instance.RefreshWindow<UINewBagView>();
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }
}
