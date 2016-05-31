using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

// 技能
public class Skill
{
    private enum State
    {
        NOT_START, // 尚未开始
        PHASE_START, // 开始
        SPELL_START, // 效果已触发
    }

    public System.Action<Skill> OnSkillFinish; // 技能结束的回调

    public long UserID; // 哪个玩家释放的技能
    public int ConfigID; // 配置ID 卡牌的id
    public int Level; // 技能当前等级   卡牌的等级
    public string Name; // 技能名字
    public string Script; // 技能的脚本
    public bool NeedToRemove;

    public int CastPoint; // 施法前摇 即施法动作时间(暂时没用，法术卡都是立即释放的，没有部署时间，也没有角色动画)
    public int Duration; // 持续时间
    public int Damage; // 技能伤害
    public int Radius; // AOE范围的大小
    public RangeType Range; // 技能范围现在都是圆形
    public TeamFlag TeamFlag; // 技能目标队伍类型
    public TargetFlag TargetFlag; // 目标类别（士兵、建筑等）

    // 释放技能时选中的目标（地点或者是单位，如果技能是不需要选中目标的，则这两个字段无效）
    public Vector3 SrcPosition; // 发射抛射物的起点 一般来说就是国王塔
    public Vector3 SelectPosition;  // 选择的技能施法点
    public Actor SelectTarget;  // 选择的技能施法目标（暂时没用，法术卡都是针对坐标点来进行释放的范围攻击）

    private int _startTime = 0; // 开始播放技能的时间
    private State _state = State.NOT_START; // 当前的技能播放状态
    private int _intervalTime = 0; // 思考间隔
    private int _lastThinkTime = 0; // 上次思考时间

    private CardsAttributeConfig _cfg; // 卡牌数据

    public CardsAttributeConfig Cfg
    {
        get
        {
            if (_cfg == null)
            {
                _cfg = CardsAttributeConfigLoader.GetConfig(ConfigID);
            }
            return _cfg;
        }
    }

    // 根据数据加载技能
    public void Init(int cfgID, int level)
    {
        ConfigID = cfgID;
        Level = level;
        Script = Cfg.Script;
        Name = Script;
        // 加载技能脚本(这里仅仅是加载脚本，并没有执行任何函数)
        LuaManager.Instance.DoFile("skill/" + Script);

        // 通知脚本，初始化技能数据
        CallFunction("OnInit");

        // 现在的技能都是圆形范围
        Range = RangeType.CIRCLE;
        Radius = Mathf.FloorToInt(Cfg.DamageRadius / 1000f * MapGrid.CellSize);
        Damage = Mathf.FloorToInt(Cfg.Damage * Mathf.Pow(GameConfig.ATK_LVL_MUL, Level));
        Duration = Cfg.LifeDuration + Level * Cfg.LifeDurationIncreasePerLevel;
    }

    // 技能销毁
    public void OnDestory()
    {
        NeedToRemove = true;

        // 通知脚本
        CallFunction("OnDestroy");
    }

    // 调用脚本函数
    public object[] CallFunction(string funcName, params object[] args)
    {
        if (args.Length > 0)
        {
            // 有参数
            object[] param = new object[args.Length + 1];
            param[0] = this;
            for (int i = 0; i < args.Length; ++i)
            {
                param[i + 1] = args[i];
            }
            return LuaManager.Instance.CallFunction(Script, funcName, param);
        }
        else
        {
            // 无参数，只需要传递self
            //Debug.Log("---------------" + Script + " : " + funcName);
            return LuaManager.Instance.CallFunction(Script, funcName, this);
        }
    }

    // 开始播放技能
    public void Play()
    {
        _startTime = BattleTime.GetTime();
        _state = State.PHASE_START;

        // 通知脚本
        CallFunction("OnSpellStart");
    }

    // 每逻辑帧更新
    public void OnTick()
    {
        if (BattleTime.GetTime() - _startTime > Duration)
        {
            // 技能结束
            _state = State.NOT_START;

            OnDestory();

            // 通知上层逻辑技能结束
            if (OnSkillFinish != null)
            {
                OnSkillFinish(this);
            }

            return;
        }

        ProcessThink();
    }

    public void OnUpdate(float dt)
    {
    }

    // 添加一个持续触发的陷阱或光环型buff，使用Aura防止它跟actor的AddBuff概念冲突
    public void AddAura(Vector3 pos, string buffName, int duration)
    {
        // 创建持有光环的马甲单位
        var actor = BattleController.Instance.CreateDummyActor(UserID, pos);
        actor.AddBuff(buffName, duration, this);
    }

    public Projectile LaunchProjectile(LuaTable param)
    {

        // 创建抛射物
        var proj = BattleController.Instance.CreateProjectile(Name + "_proj");

        // 设置参数
        // TODO:这里部分参数一定要在 SetModel 之前设置，因为 SetModel 要用到 Owner 有待优化
        // 设置回调者
        proj.Skill = this;
        proj.MultipleProjectiles = (int)(double)param["MultipleProjectiles"];  // 抛射物数量
        proj.ProjectilesFormation = (ProjectileFormation)param["ProjectilesFormation"];  // 抛射物队形
        proj.Radius = Radius;
        proj.SetModel((string)param["Model"]);
        proj.Speed = (int)(double)param["Speed"];
        proj.Gravity = (int)(double)param["Gravity"];
        proj.EnableTrack = (bool)param["EnableTrack"];

        if (proj.Gravity > 0)
        {
            // 抛物线
            proj.ProjectileType = ProjectileType.PARABOLA;
        }
        else
        {
            // 直线
            proj.ProjectileType = ProjectileType.LINEAR;
        }

        var teamFlag = param["TeamFlag"];
        if (teamFlag != null)
        {
            proj.TeamFlag = (TeamFlag)teamFlag;
        }
        else
        {
            proj.TeamFlag = TeamFlag;
        }

        var targetFlag = param["TargetFlag"];
        if (targetFlag != null)
        {
            proj.TargetFlag = (TargetFlag)targetFlag;
        }
        else
        {
            proj.TargetFlag = TargetFlag;
        }

        // 抛射物起始位置
        var launchSrc = param["LaunchSrc"];
        if (launchSrc == null)
        {
            // 没配置，默认从国王塔飞出
            proj.transform.position = SrcPosition;
        }
        else if (launchSrc is bool)
        {
            // 抛射物从国王塔飞出
            proj.transform.position = SrcPosition;

        }
        else if (launchSrc is Vector3)
        {
            // 指定抛射物起点
            proj.transform.position = (Vector3)launchSrc;
        }

        
        proj.Damage = Damage;
        proj.OwnerID = UserID;
        // 设置目标
        proj.SetTarget(SelectPosition);

        

        // 发射
        CreateDummyProjectiles(proj);
        proj.Launch();

        return proj;
    }

    // 投射物撞击到某目标或者到达指定位置 (目标不可用)
    public bool OnProjectileHitUnit(Actor target, Vector3 position)
    {
        return false;
    }

    // 投射物正在运动
    public void OnProjectileThink(Vector3 vLocation)
    {
        CallFunction("OnProjectileThink");
    }

    //开始计时功能 (OnThink) ，根据给定的间隔 (ms)。 值0代表每帧运行
    public void StartThink(int interval)
    {
        _intervalTime = interval;
        _lastThinkTime = 0;
    }

    // 停止计时器
    public void StopThink()
    {
        _intervalTime = 0;
        _lastThinkTime = 0;
    }

    // 处理没帧的回调
    private void ProcessThink()
    {
        if (_intervalTime <= 0)
        {
            return;
        }

        // 间隔触发的事件
        if (_intervalTime > 0 && BattleTime.GetTime() - _lastThinkTime >= _intervalTime)
        {
            // 通知脚本
            CallFunction("OnThink");
            _lastThinkTime = BattleTime.GetTime();
        }
    }

    // 多抛射物抛射，创建马甲抛射物
    public void CreateDummyProjectiles(Projectile realPro)
    {
        int projectileNum = realPro.MultipleProjectiles;
        if (projectileNum < 2) return;

        List<Vector3> tmpTargetPos = new List<Vector3>();
        switch (realPro.ProjectilesFormation)
        {
            case ProjectileFormation.NONE:
                for (int i = 1; i < projectileNum; i++)
                {
                    int ranX = UnityEngine.Random.Range(-Radius, Radius + 1);
                    int ranZ = UnityEngine.Random.Range(-Radius, Radius + 1);
                    tmpTargetPos.Add(SelectPosition + new Vector3(ranX, 0, ranZ));
                }
                break;
            case ProjectileFormation.ARROW:
                break;
            case ProjectileFormation.FAN:
                break;
            default:
                break;
        }

        foreach (var targetPosition in tmpTargetPos)
        {
            Projectile proj = BattleController.Instance.CreateProjectile(Name + "_proj");

            // 以真正的抛射物创建马甲抛射物
            proj.transform.position = realPro.transform.position;
            proj._model = UnityEngine.GameObject.Instantiate<GameObject>(realPro._model);
            proj._model.transform.SetParent(proj.transform);
            proj._model.transform.localPosition = Vector3.zero;
            proj.IsDummyProjectile = true;
            proj.Speed = realPro.Speed;
            proj.Gravity = realPro.Gravity;
            proj.EnableTrack = realPro.EnableTrack;
            proj.OwnerID = UserID;
            // 设置目标
            proj.SetTarget(targetPosition);
            // 发射
            proj.Launch();
        }
    }
}