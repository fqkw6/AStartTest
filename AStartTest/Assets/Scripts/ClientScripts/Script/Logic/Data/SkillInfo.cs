using UnityEngine;
using System.Collections;
using comrt.comnet;

// 技能数据
public class SkillInfo
{
    public long HeroID;
    public long EntityID;
    public int ConfigID;
    public int Level;

    public void Deserialize(PSkill data)
    {
        HeroID = data.heroId;
        EntityID = data.skillId;
        ConfigID = data.cfgId;
        Level = data.level;
    }

    private SkillSettingsConfig _cfg;
    public SkillSettingsConfig Cfg
    {
        get 
        { 
            if (_cfg == null) {
                _cfg = SkillSettingsConfigLoader.GetConfig(ConfigID);
            }
            return _cfg;
        }
    }

    public SkillInfo Clone()
    {
        SkillInfo info = new SkillInfo();
        info.HeroID = HeroID;
        info.EntityID = EntityID;
        info.ConfigID = ConfigID;
        info.Level = Level;
        return info;
    }

    public static int GetUpgradeCost(int index, int level)
    {
        UpgradeSkillConfig upgradeCfg = UpgradeSkillConfigLoader.GetConfig(index);
        if (upgradeCfg != null) {
            return upgradeCfg.Cost + Mathf.Max(0, level - 1) * upgradeCfg.CostPerLevel;
        } else {
            return 0;
        }
    }
}
