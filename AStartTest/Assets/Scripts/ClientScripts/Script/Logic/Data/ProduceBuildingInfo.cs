using UnityEngine;
using System.Collections;
using comrt.comnet;

// 可以进行生产的建筑，如民房、伐木场、采石场
public class ProduceBuildingInfo : BuildingInfo
{
    public int ProduceValue;                // 当前产出的数量
    public ElapseTime ProduceRewardElapseTime;    // 距离上次收获的经过的时间

    public override void Deserialize(PBuildInfo data)
    {
        base.Deserialize(data);

        ProduceRewardElapseTime.SetTimeMilliseconds(data.elapseTime);
    }

    // 获取当前产值
    public int GetCurrentProduceValue()
    {
        // 10分钟的产量
        float speedValue = CfgLevel.OutputNumber / (3600f / GameConfig.PRODUCE_REWARD_INTERVAL);

        // 有多少个10分钟
        int countValue = Mathf.FloorToInt(ProduceRewardElapseTime.GetTime() / GameConfig.PRODUCE_REWARD_INTERVAL);
        int value = Mathf.FloorToInt(countValue * speedValue);

        if (value > CfgLevel.MaxStorage) {
            value = CfgLevel.MaxStorage;
        }

        return value;
    }

    // 清理建筑的产值
    public void ClearProduceValue()
    {
        ProduceValue = 0;
        ProduceRewardElapseTime.SetTimeMilliseconds(0);
    }

    // 获取一小时的产量
    public int GetUnitProduceValue()
    {
        return CfgLevel.OutputNumber;
    }

    // 获取产品名称
    public string GetProduceTypeName()
    {
        switch (BuildingType) {
            case CityBuildingType.HOUSE:
                return Str.Get("UI_CITY_BUILDING_MONEY");
            case CityBuildingType.STONE:
                return Str.Get("UI_CITY_BUILDING_STONE");
            case CityBuildingType.WOOD:
                return Str.Get("UI_CITY_BUILDING_WOOD");
        }

        return null;
    }

    // 获取升级增加的一小时的产量
    public int GetNextProduceValue()
    {
        if (IsMaxLevel()) {
            return 0;
        } else {
            return CfgNextLevel.OutputNumber - CfgLevel.OutputNumber;
        }
    }

    // 资源是否满了
    public bool IsProduceFull()
    {
        switch (BuildingType) {
            case CityBuildingType.HOUSE:
                return UserManager.Instance.Money >= UserManager.Instance.MaxMoneyStorage;
            case CityBuildingType.STONE:
                return UserManager.Instance.Stone >= UserManager.Instance.MaxStoneStorage;
            case CityBuildingType.WOOD:
                return UserManager.Instance.Wood >= UserManager.Instance.MaxWoodStorage;
        }

        return false;
    }
}