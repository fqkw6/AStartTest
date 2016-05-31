using UnityEngine;
using System.Collections.Generic;

public class TestBattle : MonoBehaviour
{
    public string _serverIp;
    public bool _randomDeck;
    public int _cardLevel;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        UIManager.Instance.OpenWindow<UISystemMsgView>();

        // 先调用BattleController以初始化lua引擎
        BattleController.Instance.ServerIP = _serverIp;
        BattleController.Instance.UserLevel = _cardLevel;

        if (_randomDeck) {
            // 没有配置卡组，随机生成测试卡组
            GenerateRandomDeck();
        } else {
            // 从lua配置中读取测试用的卡组
            LuaManager.Instance.DoFile("deck");
            LuaManager.Instance.CallFunction(null, "GenerateDeck");
        }
    }

    // 生成随机卡组
    public void GenerateRandomDeck()
    {
        BattleController.Instance.CardList.Clear();

        List<int> list = new List<int>();
        foreach (var item in CardsAttributeConfigLoader.Data) {
            if(item.Key < 1001)
            list.Add(item.Key);
        }

        list = Utils.RandomSortList(list);
        for (int i = 0; i < GameConfig.MAX_CARD_COUNT; ++i) {
            var cardInfo = new CardInfo();
            cardInfo.ConfigID = list[i];
            cardInfo.Level = _cardLevel;
            BattleController.Instance.CardList.Add(cardInfo);
        }
    }

    // Use this for initialization
    void Start () {
    }

    // pvp战斗
    public void OnClickPVP()
    {
        BattleController.Instance.StartBattle(1, BattleType.PVP);
    }

    // 单机pve，只跟游戏服务器通信，不跟战斗服务器通信
    public void OnClickPVE()
    {
        BattleController.Instance.StartBattle(1, BattleType.PVE);
    }

    // 添加机器人
    public void OnClickAddBot()
    {
        BattleController.Instance.ReqAddBot();
    }

    // 随机卡组
    public void OnClickRandomDeck()
    {
        GenerateRandomDeck();
    }
}
