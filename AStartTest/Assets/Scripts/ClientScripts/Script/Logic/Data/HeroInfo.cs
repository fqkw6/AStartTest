using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using comrt.comnet;

public struct HeroProperty
{
    // 一级属性
    public int Strength;    // 力量
    public int Intelligence;    // 智力
    public int Leadership;  // 统帅

    // 二级属性
    public int Attack;  // 攻击
    public int MagicAttack; // 法术伤害
    public int Hp;  // 生命
    public float Def; // 伤害减免
    public float Critical;    // 暴击
    public float HpSorb;  // 吸血
    public float Stum;    // 重击
    public int AttackSpeed; // 攻击速度
    public float Cooldown;   // 冷却缩减
}

// 英雄数据
public class HeroInfo
{
    public long EntityID;
    public int ConfigID;
    public int Level;
    public int Exp;
    public int StarLevel;           // 星级
    public int FightingScore;       // 战斗力
    public bool IsInPVEFormation;

    public HeroProperty Property;

    public List<ItemInfo> EquipedItem = new List<ItemInfo>();
    public List<SkillInfo> LearnedSkill = new List<SkillInfo>();

    private HeroConfig _config;
    public HeroConfig Cfg
    {
        get
        {
            if (_config == null) {
                _config = HeroConfigLoader.GetConfig(ConfigID);
            }
            return _config;
        }
    }
    
    public HeroInfo()
    {
    }

    public HeroInfo Clone()
    {
        HeroInfo info = new HeroInfo();
        info.EntityID = EntityID;
        info.ConfigID = ConfigID;
        info.Level = Level;
        info.Exp = Exp;
        info.StarLevel = StarLevel;
        info.FightingScore = FightingScore;

        info.Property = Property;
        
        foreach (var item in EquipedItem) {
            info.EquipedItem.Add(item.Clone());
        }

        foreach (var item in LearnedSkill) {
            info.LearnedSkill.Add(item.Clone());
        }

        return info;
    }

    public HeroInfo(int entityId, int configId, int level)
    {
        EntityID = entityId;
        ConfigID = configId;
        Level = level;
    }

    //public void Init(BattleTeam team, UnitConfig cfg)
    //{
    //    Team = team;

    //    // 根据不同的阵营创建模型
    //    if (team == BattleTeam.BLUE_TEAM) {
    //        ModelName = cfg.PrefabBlue;
    //    } else if (team == BattleTeam.RED_TEAM) {
    //        ModelName = cfg.PrefabRed;
    //    }
    //}

    public ItemInfo GetItemByType(ItemType itemType)
    {
        foreach (var item in EquipedItem) {
            ItemsConfig cfg = ItemsConfigLoader.GetConfig(item.ConfigID);
            if (cfg != null && (ItemType)cfg.Type == itemType) {
                return item;
            }
        }
        return null;
    }

    public ItemInfo GetItem(long itemID)
    {
        return EquipedItem.Find((x) => x.EntityID == itemID);
    }

    public ItemInfo GetItemByConfigID(int itemCfgID)
    {
        return EquipedItem.Find((x) => x.ConfigID == itemCfgID);
    }

    public SkillInfo GetSkillByID(int skillID)
    {
        SkillInfo info = LearnedSkill.Find((x)=>x.ConfigID == skillID);
        return info;
    }

    public int GetSkillConfigIDByIndex(int index)
    {
        if (index == 1) {
            return Cfg.Skill1;
        } else if (index == 2) {
            return Cfg.Skill2; 
        } else if (index == 3) {
            return Cfg.Skill3;
        } else if (index == 4) {
            return Cfg.Skill4;
        }
        return 0;
    }
    // 当前的装备
    private List<int> _equipedItem = new List<int>();

    public void Deserialize(PHeroAttr data, bool notifyChange)
    {
        EntityID = data.heroId;
        ConfigID = data.heroCfgId;
        StarLevel = data.starLv;
        FightingScore = data.fighting;
        Exp = data.xiuwei;
        Level = data.level;

        EquipedItem.Clear();
        foreach (var item in data.fitups) {
            ItemInfo info = new ItemInfo();
            info.Deserialize(item);
            EquipedItem.Add(info);
        }

        LearnedSkill.Clear();
        foreach (var item in data.skillList) {
            SkillInfo info = new SkillInfo();
            info.Deserialize(item);
            LearnedSkill.Add(info);
        }

        HeroProperty oldProperty = Property;

        if (data.basicAttr != null) {
            Property.Strength = data.basicAttr.strength;
            Property.Intelligence = data.basicAttr.Intelligence;
            Property.Leadership = data.basicAttr.LeaderShip;
        }

        if (data.battleAttr != null) {
            Property.Attack = data.battleAttr.physAtk;
            Property.MagicAttack = data.battleAttr.magicAtk;
            Property.Hp = data.battleAttr.hp;
            Property.Def = data.battleAttr.damReduce;
            Property.Critical = data.battleAttr.criticStrike;
            Property.Stum = data.battleAttr.damDeepen;
            Property.AttackSpeed = data.battleAttr.atkSpeed;
            Property.Cooldown = data.battleAttr.coolDown;
        }

        if (notifyChange) {
            const float delayTimeInterval = 0.2f;
            float delayTime = 0;
            if (Property.Strength != oldProperty.Strength) {
                AddFloatingMsg(Str.Get("ATTR_STR"), Property.Strength - oldProperty.Strength, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.Intelligence != oldProperty.Intelligence) {
                AddFloatingMsg(Str.Get("ATTR_INT"), Property.Intelligence - oldProperty.Intelligence, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.Leadership != oldProperty.Leadership) {
                AddFloatingMsg(Str.Get("ATTR_LEADER"), Property.Leadership - oldProperty.Leadership, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.Attack != oldProperty.Attack) {
                AddFloatingMsg(Str.Get("ATTR_ATTACK"), Property.Attack - oldProperty.Attack, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.MagicAttack != oldProperty.MagicAttack) {
                AddFloatingMsg(Str.Get("ATTR_MAGIC_ATTACK"), Property.MagicAttack - oldProperty.MagicAttack, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.Hp != oldProperty.Hp) {
                AddFloatingMsg(Str.Get("ATTR_HP"), Property.Hp - oldProperty.Hp, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Math.Abs(Property.Def - oldProperty.Def) > 0.01f) {
                AddFloatingMsg(Str.Get("ATTR_DEF"), Property.Def - oldProperty.Def, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Math.Abs(Property.Critical - oldProperty.Critical) > 0.01f) {
                AddFloatingMsg(Str.Get("ATTR_CRIT"), Property.Critical - oldProperty.Critical, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Math.Abs(Property.HpSorb - oldProperty.HpSorb) > 0.01f) {
                AddFloatingMsg(Str.Get("ATTR_HP_SORB"), Property.HpSorb - oldProperty.HpSorb, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Math.Abs(Property.Stum - oldProperty.Stum) > 0.01f) {
                AddFloatingMsg(Str.Get("ATTR_STUM"), Property.Stum - oldProperty.Stum, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Property.AttackSpeed != oldProperty.AttackSpeed) {
                AddFloatingMsg(Str.Get("ATTR_ATTACK_SPEED"), Property.AttackSpeed - oldProperty.AttackSpeed, delayTime);
                delayTime += delayTimeInterval;
            }

            if (Math.Abs(Property.Cooldown - oldProperty.Cooldown) > 0.01f) {
                AddFloatingMsg(Str.Get("ATTR_CD"), Property.Cooldown - oldProperty.Cooldown, delayTime);
                delayTime += delayTimeInterval;
            }
        }
    }

    private void AddFloatingMsg(string text, float value, float delayTime)
    {
        if (value > 0) {
            UIUtil.AddFloatingMsg(text + " +" + value, Color.green, delayTime);
        } else {
            UIUtil.AddFloatingMsg(text + " -" + Mathf.Abs(value), Color.red, delayTime);
        }
    }
    
    public string GetName()
    {
        return Cfg.HeroName;
    }

    // 此英雄是否在pve出站队列
    public bool IsOnPVE()
    {
        return UserManager.Instance.PVEHeroList.Find(x=>x.ConfigID == ConfigID) != null;
    }
    
    public void OnAddExp(int value)
    {
        Exp += value;

        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(Level);
        if (expCfg != null) {
            if (Exp + value >= expCfg.ExpRequire) {
                // 升级的情况由服务端处理

            } else {
                // 未升级的情况直接加经验
                Exp += value;
            }
        }
    }

    // 获取技能名称等级
    public string GetFixedSkillName(int skillID, int level)
    {
        SkillSettingsConfig skillCfg = SkillSettingsConfigLoader.GetConfig(skillID);
        return skillCfg.Name + "  LV." + level;
    }

    // 获取技能描述
    public string GetSkillDesc(int skillID, int level)
    {
        SkillSettingsConfig skillCfg = SkillSettingsConfigLoader.GetConfig(skillID);

        int skillLevel = level;

        float value1 = 0;
        float value2 = 0;

        switch (skillLevel) {
            case 1:
                value1 = skillCfg.Level1Param1;
                value2 = skillCfg.Level1Param2;
                break;
            case 2:
                value1 = skillCfg.Level2Param1;
                value2 = skillCfg.Level2Param2;
                break;
            case 3:
                value1 = skillCfg.Level3Param1;
                value2 = skillCfg.Level3Param2;
                break;
            case 4:
                value1 = skillCfg.Level4Param1;
                value2 = skillCfg.Level4Param2;
                break;
        }
        return string.Format(skillCfg.Description, value1, value2);
    }

    public string GetTalentSkillInfo()
    {
        return GetSkillDesc(Cfg.PassiveSkill, StarLevel - 1);
    }

}
