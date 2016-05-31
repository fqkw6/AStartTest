using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using comrt.comnet;

public class TestGM : MonoBehaviour
{
    private bool _showGM = false;
    private bool _hideGM = false;

    void Start ()
	{
	}
	
    void OnGUI()
    {

        const int BUTTON_WIDTH = 100;
        const int BUTTON_HEIGHT = 60;
        const int OFFSET = 10;

        if (!Game.Instance.IsInGame && !Game.Instance.IsOffline) {
            return;
        }

        if (_hideGM) {
            if (GUI.Button(new Rect(100, 0, BUTTON_WIDTH, BUTTON_HEIGHT), "显示")) {
                _hideGM = !_hideGM;
            }
            return;
        }

        float x = 0;
        float y = 0;
        
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "显示GM命令")) {
            _showGM = !_showGM;
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "隐藏")) {
            _hideGM = !_hideGM;
        }

        if (!_showGM) return;

        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "创建英雄")) {
            foreach (var item in HeroConfigLoader.Data) {
                GMManager.Instance.RequestCreateHero(item.Key);
                break;
            }
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "玩家升级+1")) {
            PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(UserManager.Instance.Level);
            GMManager.Instance.RequestPlayerUpLevel(cfg.Exp);
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "玩家升级+10")) {
            int exp = 0;
            for (int i = UserManager.Instance.Level; i < UserManager.Instance.Level + 10; ++i) {
                PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(i, false);
                if (cfg == null) {
                    break;
                }
                exp += cfg.Exp;
            }

            GMManager.Instance.RequestPlayerUpLevel(exp);
        }
        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "大富翁")) {
            if (Game.Instance.IsOffline) {
                UserManager.Instance.Money = 99999999;
                UserManager.Instance.Gold = 99999999;
                UserManager.Instance.Wood = 99999999;
                UserManager.Instance.Stone = 99999999;
            } else {
                GMManager.Instance.RequestAddGood(GameConfig.ITEM_CONFIG_ID_MONEY, 100000);
                GMManager.Instance.RequestAddGood(GameConfig.ITEM_CONFIG_ID_WOOD, 100000);
                GMManager.Instance.RequestAddGood(GameConfig.ITEM_CONFIG_ID_STONE, 100000);
                GMManager.Instance.RequestAddGood(GameConfig.ITEM_CONFIG_ID_GOLD, 100000);
            }
        }
        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "穷B")) {
            if (Game.Instance.IsOffline) {
                UserManager.Instance.Money = 0;
                UserManager.Instance.Gold = 0;
                UserManager.Instance.Wood = 0;
                UserManager.Instance.Stone = 0;
            } else {
                UseItem(GameConfig.ITEM_CONFIG_ID_MONEY, 100000);
                UseItem(GameConfig.ITEM_CONFIG_ID_WOOD, 100000);
                UseItem(GameConfig.ITEM_CONFIG_ID_STONE, 100000);
                UseItem(GameConfig.ITEM_CONFIG_ID_GOLD, 100000);
            }
        }

        x = 0;
        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "添加全部英雄")) {
            if (Game.Instance.IsOffline) {
                TestLocalData.Instance.AddAllHero();
            } else {
                StartCoroutine(GetAllHero());
            }
        }

        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "添加全部物品")) {
            if (Game.Instance.IsOffline) {
                TestLocalData.Instance.AddAllItem();
            } else {
                StartCoroutine(GetAllItem());
            }
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "请求商品列表")) {
            //ShopManager.Instance.RequestShopInfo();
        }

        x = 0;
        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "添加全部建筑")) {
            if (Game.Instance.IsOffline) {
                TestLocalData.Instance.AddAllBuilding();
                UIManager.Instance.RefreshWindow<UICityView>();
            }
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "添加全部兵种")) {
            if (Game.Instance.IsOffline) {
                TestLocalData.Instance.AddAllSoldier();
            }
        }

        x = 0;
        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "掉线")) {
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "战斗开始")) {
            BattleManager.Instance.StartBattle(1, LogicBattleType.PVP);
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "战斗结束")) {
            BattleManager.Instance.OnBattleFinish(true);
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "返回主场景")) {
            BattleManager.Instance.ChangeToMainScene();
        }

        x = 0;
        y += BUTTON_HEIGHT + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "添加城池")) {
            if (Game.Instance.IsOffline) {
                TestLocalData.Instance.AddAllCity();
                UIManager.Instance.RefreshWindow<UIWorldMapView>();
            }
        }

        x += BUTTON_WIDTH + OFFSET;
        if (GUI.Button(new Rect(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), "收集资源")) {
            WorldManager.Instance.RequestCollectResource();
        }
    }

    private void UseItem(int itemCfgID, int count)
    {
        PCMInt2 data = new PCMInt2();
        data.arg = itemCfgID;
        data.arg2 = count;

        NetworkManager.Instance.Send(eCommand.SYS_REDUCE_GOOD, data, (byte[] buffer) =>
        {
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (ret.err_code != 0) {
                Log.Error("GM使用物品失败 {0}", ret.err_code);
                return;
            }

            Log.Info("GM使用物品成功");
            if (itemCfgID == GameConfig.ITEM_CONFIG_ID_MONEY) {
                UserManager.Instance.Money = Mathf.Max(UserManager.Instance.Money - count, 0);
            } else if (itemCfgID == GameConfig.ITEM_CONFIG_ID_WOOD) {
                UserManager.Instance.Money = Mathf.Max(UserManager.Instance.Wood - count, 0);
            } else if (itemCfgID == GameConfig.ITEM_CONFIG_ID_STONE) {
                UserManager.Instance.Money = Mathf.Max(UserManager.Instance.Stone - count, 0);
            } else if (itemCfgID == GameConfig.ITEM_CONFIG_ID_GOLD) {
                UserManager.Instance.Money = Mathf.Max(UserManager.Instance.Gold - count, 0);
            } else {
                ItemInfo info = UserManager.Instance.GetItemByConfigID(itemCfgID);
                if (info != null) {
                    info.Number = Mathf.Max(info.Number - count, 0);
                }
            }

            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }

    IEnumerator GetAllHero()
    {
        UserManager.Instance.HeroList.Clear();
        foreach (var item in HeroConfigLoader.Data) {
            GMManager.Instance.RequestCreateHero(item.Key);
            yield return new WaitForSeconds(0.1f);
        }

        Log.Info("GetAllHero 请求完毕");
    }

    IEnumerator GetAllItem()
    {
        foreach (var item in ItemsConfigLoader.Data) {
            // 货币
            if (item.Value.Type == 10) continue;

            if (item.Value.Type >= 9) {
                GMManager.Instance.RequestAddGood(item.Key, 100);
            }

            yield return new WaitForSeconds(0.1f);
        }

        Log.Info("GetAllItem 请求完毕");
    }
}
