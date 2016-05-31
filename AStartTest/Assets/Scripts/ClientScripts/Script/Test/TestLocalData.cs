using UnityEngine;
using System.Collections;

public class TestLocalData : Singleton<TestLocalData>
{
    public int[] testSkill;
    void Awake()
    {
    }

    // 添加全部英雄
    public void AddAllHero()
    {
        int index = 0;
        UserManager.Instance.HeroList.Clear();
        foreach (var item in HeroConfigLoader.Data) {
            ++index;
            HeroInfo info = new HeroInfo();
            info.EntityID = index;
            info.ConfigID = item.Key;
            info.StarLevel = 1;
            info.Level = 10;
            info.FightingScore = 1;

            UserManager.Instance.HeroList.Add(info);
        }
        UserManager.Instance.SortHeroList();
        
    }

    public void AddAllItem()
    {
        int index = 0;
        UserManager.Instance.ItemList.Clear();
        foreach (var item in ItemsConfigLoader.Data) {
            ++index;
            ItemInfo info = new ItemInfo();
            info.EntityID = index;
            info.ConfigID = item.Key;
            info.Number = 1;

            UserManager.Instance.ItemList.Add(info);
        }
    }

    public void AddAllBuilding()
    {
        int index = 0;
        CityManager.Instance.BuildingList.Clear();

        foreach (var item in BuildingConstConfigLoader.Data) {
            ++index;
            BuildingInfo info = CityManager.Instance.CreateBuilding(item.Key);
            info.EntityID = index;
            info.ConfigID = item.Key;
            info.Level = 1;
            info.LevelUpRemainTime.Reset();

            CityManager.Instance.BuildingList.Add(info);
        }
    }

    public void AddAllSoldier()
    {
        int index = 0;

        CityManager.Instance.SoldierLevelList.Clear();
        foreach (var item in SoldierConfigLoader.Data) {
            CityManager.Instance.SoldierLevelList[item.Key] = 1;
        }
    }

    public void AddAllCity()
    {
        if (Game.Instance.IsOffline) {
            int index = 0;
            for (int i = 0; i < 10; ++i) {
                ++index;
                WorldCityInfo city = new WorldCityInfo();
                city.MapPosition = index;
                city.UserName = "测试" + index;
                city.UserLevel = 11;
                city.UserFightScore = 1234;
                city.RewardMoney = 10000;
                city.RewardStone = 100;
                city.RewardWood = 10;

                int heroIndex = 0;
                foreach (var item in HeroConfigLoader.Data) {
                    if (heroIndex >= 3) {
                        break;
                    }
                }
                WorldManager.Instance.CityList.Add(city);
            }

            for (int i = 0; i < 5; ++i) {
                ++index;
                WorldResTownInfo city = new WorldResTownInfo();
                city.MapPosition = index;
                city.UserName = "测试" + index;
                city.UserLevel = 11;
                city.UserFightScore = 1234;
                city.RewardMoney = 10000;
                city.RewardStone = 100;
                city.RewardWood = 10;

                int heroIndex = 0;
                foreach (var item in HeroConfigLoader.Data) {
                    if (heroIndex >= 3) {
                        break;
                    }
                }

                WorldManager.Instance.CityList.Add(city);
            }
        }
    }
}