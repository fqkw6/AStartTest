using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public partial class UserManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.GET_ASSERT_INFO, OnMsgGetAssetInfo);
        Net.Register(eCommand.GET_KNAPSACK_INFO, OnMsgGetKnapsackInfo);
        Net.Register(eCommand.GET_ROLE_INFO, OnGetRoleInfo);
        Net.Register(eCommand.GET_SKILL_POINT, OnGetSkillPoint);
        Net.Register(eCommand.GET_SOME_HEROS_INFO, OnGetSomeHerosInfo);
        Net.Register(eCommand.PUSH_ROLELEVEL_UPGRADE, OnRoleLevelUpgrade);
    }

    // 请求同步玩家资源数据
    public void RequestSyncRes()
    {
        Net.Send(eCommand.GET_ASSERT_INFO);
    }

    private void OnMsgGetAssetInfo(byte[] buffer)
    {
        PAssertAttr ret = Net.Deserialize<PAssertAttr>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_ASSERT_INFO)) return;

        Deserialize(ret);
    }

    // 请求背包数据
    public void RequestBagInfo()
    {
        Net.Send(eCommand.GET_KNAPSACK_INFO);
    }

    private void OnMsgGetKnapsackInfo(byte[] buffer)
    {
        PComItemList ret = Net.Deserialize<PComItemList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_KNAPSACK_INFO)) return;

        // 添加物品
        ItemList.Clear();
        AddItem(ret, false);

        UIManager.Instance.RefreshWindow<UINewBagView>();
    }

    // 获取玩家数据
    public void RequestUserInfo()
    {
        if (Game.Instance.IsInGame) {
            Net.Send(eCommand.GET_ROLE_INFO);
        } else {
            // 在ServerManager中处理接收消息
            PUserId builder = new PUserId();
            builder.userId = ServerManager.Instance.AccountID;
            builder.serverId = ServerManager.Instance.CurrentGameServerID;
            Net.Send(eCommand.GET_PLAYER_INFO, builder);
        }
    }

    private void OnGetRoleInfo(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_ROLE_INFO)) return;

        Deserialize(ret);
    }

    public void RequestSkillPointInfo()
    {
        Net.Send(eCommand.GET_SKILL_POINT);
    }

    private void OnGetSkillPoint(byte[] buffer)
    {
        PSkillPoint ret = Net.Deserialize<PSkillPoint>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_SKILL_POINT)) return;

        Log.Info("获取玩家技能点信息成功: {0}   {1}", ret.skillPoint, ret.nextTime / 1000.0f);

        UserManager.Instance.SetSkillPoint(ret.skillPoint, ret.nextTime);

        EventDispatcher.TriggerEvent(EventID.EVENT_UI_REFRESH_SKILL_POINT);
    }

    // 获取英雄信息
    public void RequestHeroInfo(List<long> heroesID)
    {
        PCmLongList builder = new PCmLongList();
        foreach (var item in heroesID) {
            builder.arg.Add(item);
        }
        Net.Send(eCommand.GET_SOME_HEROS_INFO, builder);
    }

    private void OnGetSomeHerosInfo(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_SOME_HEROS_INFO)) return;

        UserManager.Instance.Deserialize(ret);
    }

    // 卸下装备
    public bool RequestTakeOffEquip(long heroID, long itemID)
    {
        HeroInfo heroInfo = GetHeroInfo(heroID);
        if (heroInfo == null) return false;
        
        PUseCommonItem builder = new PUseCommonItem();
        builder.heroId = heroID;

        ItemInfo itemInfo = heroInfo.GetItem(itemID);
        if (itemInfo == null) return false;

        PComItem ibuilder = new PComItem();
        ibuilder.id = itemID;
        builder.comItem.Add(ibuilder);

        NetworkManager.Instance.Send(eCommand.FIT_DOWN_EQUIP, builder, (buffer) => {
            PHeroAttr ret = Net.Deserialize<PHeroAttr>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.FIT_DOWN_EQUIP)) return;

            // 把装备放到背包里
            AddItem(itemInfo, false);

            heroInfo.Deserialize(ret, true);

            UIManager.Instance.RefreshWindow<UINewHeroView>();
            UIManager.Instance.RefreshWindow<UINewHeroListView>();
        });
        return true;
    }

    private void OnRoleLevelUpgrade(byte[] buffer)
    {
        PRoleList ret = Net.Deserialize<PRoleList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.PUSH_ROLELEVEL_UPGRADE)) return;

        if (ret.roleAttr != null) {
            Deserialize(ret.roleAttr);
        }

        if (ret.heroAttr != null) {
            HeroInfo heroInfo = GetHeroInfo(ret.heroAttr.heroId);
            if (heroInfo != null) {
                heroInfo.Deserialize(ret.heroAttr, false);
            }
        }

        UIManager.Instance.RefreshWindow<UINewHeroView>();
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }

    // 请求更换名字
    public void RequestChangeName(string newName)
    {
        // 发送消息  减少元宝  替换名字
    }

    // 请求更换头像
    public void RequestChangeIcon(string icon)
    {
        
    }

    // 请求登出游戏
    public void RequestLogout()
    {
        NetworkManager.Instance.Close();
    }
}
