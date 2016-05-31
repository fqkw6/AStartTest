using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PropertySet = System.Collections.Generic.Dictionary<PropertyType, int>;

// 基础人物
public partial class Actor : MonoBehaviour
{
    public long UserID; // 所属玩家id
    public long EntityID; // 实例id，角色唯一，只有战斗时有用
    public int ConfigID; // 配置id
    public int Level; // 当前等级
    public bool NeedToRemove;   // 现在用于马甲单位的销毁，一个标志
    public float CollisonRadius = 4f;  // 角色碰撞范围 

    // 基础属性
    public GameObject Model; // 模型
    public System.Action<Actor> OnActorDeath;
    public bool DeployFinished = false;  // TODO:是否部署完毕，目前用于处理建筑单位自动扣血

    // 普通攻击
    public string Projectile; // 抛射物prefab路径(如果普通攻击是远程攻击的话)
    public int ProjectileSpeed; // 抛射物速度
    public int ProjectileGravity;   // 重力，影响抛物线轨迹，如果重力为0则是直线抛射物
    public bool ProjectileTrack; // 抛物线是否会追踪目标 如果追踪目标，则抛射物是必中的，否则会在落点的时候进行范围判定 
    public Transform ProjectileSpawnPoint;  // 抛射位置（相对于发射者）
    public int MultipleProjectiles = 1;  // 抛射物数量
    public ProjectileFormation ProjectilesFormation = ProjectileFormation.NONE;  // 抛射物队形

    // 从lua中设置的变量
    public float HealthBarOffsetY;  // 血条的偏移
    public float FlyingHeight = 0; // 飞行的高度
    public string AtkAnimation = "attack_01"; // 攻击动画
    public string MoveAnimation = "walk";   // 跑步动画
    public bool CanBeTarget = true;

    // 移动战斗逻辑
    public Vector3 Position;    // 坐标(逻辑坐标)
    public int AttackerCount; // 攻击该单位人的数量，ai使用
    public Actor CastTarget; // 施法目标
    public Vector3 CastPosition; // 施法目标
    public Vector3 TargetPosition; // 移动的目的地
    public Vector3 RebornPosition; // 重生地点(一般是大本营，重生技能可以原地复活)
    public Actor TargetAggro; // 仇恨目标(强制攻击目标)
    public Actor TargetPursuing; // 正在追击的目标
    public Actor TargetAttacking; // 正在攻击的目标
    public Vector3 PreviewOffset; // 预览时的偏移

    public bool IsSummoned; // 一个标志，是否是召唤生物
    public bool IsIllusion; // 一个标志，是否是镜像
    public bool IsDummy;    // 是否是马甲单位，马甲单位在buff消失的时候要自动销毁

    // 私有变量
    private int _lifeReducePerTurn;  // 建筑物每一帧掉多少血
    private BitArray _status; // 状态位（眩晕、沉默等）
    private List<Buff> _buffList = new List<Buff>(); // 所有的buff
    private List<Actor> _summonList = new List<Actor>(); // 召唤物列表
    private List<Actor> _illusionList = new List<Actor>(); // 幻象列表
    protected Dictionary<string, GameObject> _effectDictionary = new Dictionary<string, GameObject>(); // 当前角色已绑定的光效

    private int _thinkInterval = 0;   // 思考间隔，由ai脚本设置 毫秒
    private int _lastThinkTime = 0;   // 上次思考的时间 毫秒

    // 角色属性
    private bool _isPause; // 暂停和继续单位逻辑
    private bool _isDead; // 是否已死亡
    private bool _isPauseFSM;   // 停止状态机更新（但是不影响其他逻辑）

    // 战斗属性
    private int _currentHP; // 当前血量  这些值不受Buff 装备影响，战斗过程中会对其进行操作
    private int _currentShield; // 当前护盾

    // 会被装备、buff影响的属性
    public PropertySet Data = new PropertySet(); // 总和，计算后的最终值
    public PropertySet BaseData = new PropertySet(); // 基础属性(含装备和符文加成)
    public PropertySet BuffData = new PropertySet(); // 状态属性(含被动技能和天赋)

    // 初始化
    public virtual void OnInit()
    {
        _fsm = new FSMStateMachine(this);

        // 初始化状态
        InitState();
        InitProperty();

        // 添加组件
        SetAvatarComponent(gameObject.AddComponent<AvatarComponent>());
        SetHUDComponent(gameObject.AddComponent<HUDComponent>());
        SetAnimationComponent(gameObject.AddComponent<AnimationComponent>());

        if (ConfigID > 0)
        {
            if (!string.IsNullOrEmpty(Cfg.Script))
            {
                LuaManager.Instance.DoFile("ai/" + Cfg.Script);

                CallFunction("OnInit");
            }
        }
    }

    // 每帧更新
    public virtual void OnUpdate(float delta)
    {
        if (IsPause)
        {
            // 暂停，停止更新
            return;
        }

        // 更新状态机(渲染帧)
        if (_fsm != null && !_isPauseFSM)
        {
            _fsm.UpdateFSM(delta);
        }

        // 更新所有组件
        foreach (var item in _components)
        {
            item.Value.OnUpdate(delta);
        }
        // TODO:角色推挤
        ApplyCollisionMove(delta);
    }

    // 逻辑帧更新
    public virtual void OnTick()
    {
        if (IsPause)
        {
            // 暂停，停止更新
            return;
        }

        // 更新状态机(逻辑帧)
        if (_fsm != null && !_isPauseFSM)
        {
            _fsm.UpdateFSM();
        }

        // 更新各个组件
        foreach (var item in _components)
        {
            item.Value.OnTick();
        }

        // 建筑物自动掉血
        ProcessBuilding();

        // 更新计时器
        UpdateTimer();

        // 更新技能和buff
        UpdateBuff();

        // ai思考
        OnThink();
    }

    // 销毁，释放资源
    public virtual void OnDestroy()
    {
        foreach (var item in _components)
        {
            item.Value.OnDestory();
        }

        // 删除模型
        if (Model != null)
        {
            Destroy(Model);
        }
    }

    protected void InitProperty()
    {
        if (ConfigID == 0)
        {
            return;
        }

        Data.Clear();
        BaseData.Clear();
        BuffData.Clear();

        // 基础数值 TODO 将来考虑符文影响
        BaseData[PropertyType.Hp] = Mathf.FloorToInt(Cfg.Health * Mathf.Pow(GameConfig.HP_LVL_MUL, Level)); // 血量
        BaseData[PropertyType.Dmg] = Mathf.FloorToInt(Cfg.Damage * Mathf.Pow(GameConfig.ATK_LVL_MUL, Level));    // 伤害
        BaseData[PropertyType.HpPct] = 0;   // 血量增加百分比，符文获得
        //BaseData[PropertyType.Shield] = 0;  // 护盾（现在只有骑士有护盾，扣血的时候先扣护盾）
        BaseData[PropertyType.Shield] = Mathf.FloorToInt(Cfg.ShieldHitpoints);
        BaseData[PropertyType.DmgPct] = 0;  // 伤害增加百分比，符文获得
        BaseData[PropertyType.MoveSpeedPct] = 0; // 移动速度增加百分比
        BaseData[PropertyType.AtkSpeedPct] = 0; // 攻击速度增加百分比

        // 更新数据
        UpdateProperty();

        // 当前血量和护盾
        CurrentHP = GetMaxHealth();
        CurrentShield = GetProperty(PropertyType.Shield);
        _lifeReducePerTurn = GetBuildingLifeReducePerTurn(CurrentHP);
    }

    // 获取建筑每秒流失生命值
    public int GetBuildingLifeReducePerTurn(int startHP)
    {
        if (GetBuildingLifeDuration() <= 0) return 0;
        int lifeReducePerTurn = startHP / GetBuildingLifeDuration();
        return lifeReducePerTurn > 0 ? lifeReducePerTurn : 1;
    }

    // 获取最终属性值
    public int GetProperty(PropertyType property)
    {
        int value = 0;
        if (Data.TryGetValue(property, out value))
        {
            return value;
        }

        return 0;
    }

    // 部分属性要考虑百分比加成影响
    public int GetProperty(PropertyType property, PropertyType propertyPct)
    {
        int value = 0;
        if (Data.TryGetValue(property, out value))
        {
            int pct = 0;
            if (Data.TryGetValue(propertyPct, out pct))
            {
                return Mathf.FloorToInt(value * (1 + pct / 100f));
            }
            else
            {
                return value;
            }
        }
        return 0;
    }

    // 获取属性值
    private int GetProperty(PropertySet data, PropertyType property)
    {
        int value = 0;
        if (data.TryGetValue(property, out value))
        {
            return value;
        }

        return 0;
    }

    // 重新计算属性（一般是 buff 更新后计算）
    public void UpdateProperty()
    {
        BuffData.Clear();

        // 遍历所有的buff，处理属性变化
        foreach (var item in _buffList)
        {
            item.ProcessProperty(BuffData);
        }

        foreach (PropertyType property in Enum.GetValues(typeof(PropertyType)))
        {
            int baseValue = GetProperty(BaseData, property);
            int buffValue = GetProperty(BuffData, property);
            Data[property] = baseValue + buffValue;
        }
        // 通知状态机属性改变
        EventDispatcher.TriggerEvent(EventID.ACTOR_PROPERTY_CHANGE);
    }

    // 配置属性
    private CardsAttributeConfig _cfg;

    public CardsAttributeConfig Cfg
    {
        get
        {
            if (ConfigID == 0)
            {
                return null;
            }

            if (_cfg == null)
            {
                _cfg = CardsAttributeConfigLoader.GetConfig(ConfigID);
            }
            return _cfg;
        }
    }

    // 最大血量
    public int GetMaxHealth()
    {
        return GetProperty(PropertyType.Hp, PropertyType.HpPct);
    }

    // 当前血量
    public int CurrentHP
    {
        get { return _currentHP; }
        set { _currentHP = Mathf.Clamp(value, 0, GetMaxHealth()); }
    }

    // 护盾 有护盾的先扣护盾，当护盾没了再扣血
    public int CurrentShield
    {
        get { return _currentShield; }
        set { _currentShield = Mathf.Clamp(value, 0, GetProperty(PropertyType.Shield)); }
    }

    // 是否暂停
    public bool IsPause
    {
        get { return _isPause; }
        set { _isPause = value; }
    }

    // 是否已死亡
    public bool IsDead
    {
        get { return _isDead; }
        set { _isDead = value; }
    }

    // 是否是范围攻击
    public bool IsRangeDmg()
    {
        return Cfg.DamageRadius > 0;
    }

    // 是否可以飞行
    public bool IsFlyable()
    {
        return FlyingHeight > 0;
    }

    // 是否是敌方玩家的单位
    public bool IsEnemy()
    {
        return UserID != BattleController.Instance.UserID;
    }

    // 是否是敌对目标
    public bool IsEnemy(Actor target)
    {
        return UserID != target.UserID;
    }

    // 是否是塔
    public bool IsBuilding()
    {
        return ConfigID != 0 && Cfg.Type == (int)CardType.BUILDING;
    }

    // 是否被冰冻了
    public bool IsFrozen()
    {
        return CheckStatus(StatusFlag.FROZEN);
    }

    // 是否是无敌的
    public bool IsInvulinerable()
    {
        return CheckStatus(StatusFlag.INVULNERABLE);
    }

    // 该单位是否被沉默
    public bool IsSilenced()
    {
        return CheckStatus(StatusFlag.SILENCED);
    }

    //该单位是否被晕眩了
    public bool IsStunned()
    {
        return CheckStatus(StatusFlag.STUNNED);
    }

    // 伤害
    public int GetDamage()
    {
        return GetProperty(PropertyType.Dmg, PropertyType.DmgPct);
    }

    // 移动速度
    public float GetMoveSpeed()
    {
        // 换算一个倍率关系，60代表每秒移动一格，120就代表每秒移动两格
        int pct = GetProperty(PropertyType.MoveSpeedPct);
        return Cfg.MoveSpeed * (1 + pct / 100f) / 60f * MapGrid.CellSize * 1.5f;
        //return Mathf.FloorToInt(Cfg.MoveSpeed * (1 + pct / 100f) / 60f * MapGrid.CellSize);
    }

    // 攻击间隔
    public int GetAtkInterval()
    {
        int pct = GetProperty(PropertyType.AtkSpeedPct);
        return Mathf.FloorToInt(Cfg.AtkInterval * (1 + pct / 100f));
    }

    // 攻击距离
    public int GetAtkRange()
    {
        float range = Cfg.AtkRange / 1000f * MapGrid.CellSize;
        return Mathf.FloorToInt(Cfg.AtkRange / 1000f * MapGrid.CellSize);
    }

    // 获取视野距离
    public int GetSightRange()
    {
        return Mathf.FloorToInt(Cfg.SightRange / 1000f * MapGrid.CellSize);
    }

    // 碰撞范围
    public float GetCollisionRadius()
    {
        return Cfg.CollisionRadius / 1000f * MapGrid.CellSize;
    }

    // 攻击前摇
    public int GetAtkHitpoint()
    {
        int pct = GetProperty(PropertyType.AtkSpeedPct);
        return Mathf.FloorToInt(Cfg.AtkLoad * (1 + pct / 100f));
    }

    //治疗该单位(amount为增量)
    public void Heal(int value, Actor source)
    {
        CurrentHP += value;
    }

    // 创建模型(如果之前已有模型的话，则释放原来的模型)
    public void SetModel(string prefabPath)
    {
        _avatar.CreateModel("Model/Battle/" + prefabPath, (goModel) =>
        {
            Model = goModel;

            // TODO:此处添加阴影
            GameObject prefab = Resources.Load<GameObject>("Model/Battle/" + "Eff_yinying");
            GameObject shadow = Instantiate(prefab);
            shadow.transform.SetParent(transform);
            //shadow.transform.localScale = new Vector3(Model.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.x, 1, Model.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.z);
            shadow.transform.localPosition = new Vector3(0, 2, 0);

            OnModelChanged();

            // 因效率问题，暂时取消阴影
            //SetShadow(true);
        });
    }

    // 设置模型
    public void SetModel(GameObject model)
    {
        Model = model;
        Model.transform.SetParent(transform);
        Model.transform.localPosition = Vector3.zero;
        //SetShadow(true);

        OnModelChanged();
    }

    // 设置坐标(逻辑和显示坐标)
    public void SetPosition(Vector3 pos)
    {
        Position = pos;
        transform.position = pos;
    }

    // 创建血条
    public void CreateHealthBar(string prefabName)
    {
        if (_hud != null)
        {
            _hud.CreateHealthBar(prefabName);
        }
    }

    // 显示血条
    public void ShowHealthBar()
    {
        if (_hud != null)
        {
            _hud.ShowHealthBar();
        }
    }

    // 设置物理参数 寻路组件
    public void SetupPhysic()
    {
        // 使用物理模拟推人和阻挡   不做RVO，即角色不带避人   不用CharactorController因为角色控制器不带物理效果
        // TODO 将来替换为确定性2D物理
        /*
        Rigidbody rbody = gameObject.AddComponent<Rigidbody>();
        if (rbody != null) {
            // 地图没有高度概念，单位都不受重力影响，运动锁定在xz平面
            rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            rbody.useGravity = false;
            rbody.drag = 1;
            rbody.velocity = Vector3.zero;

            // 建筑物是不带动力学的，即建筑物不可移动或者被推开
            if (IsBuilding()) {
                rbody.isKinematic = true;
            } else {
                rbody.isKinematic = false;
            }
        }

        // 碰撞体
        CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
        if (col != null) {
            // 注意height要大于center*2，否则会因为跟地面碰撞导致单位飞走
            col.height = 11;
            col.center = new Vector3(0, 5f, 0);
            col.radius = 3;

            // 默认先关闭碰撞，直到部署完毕再开启碰撞
            col.enabled = false;
        }
        */

        // 建筑物不会移动，除建筑之外的其他单位添加寻路组件
        if (!IsBuilding())
        {
            gameObject.AddComponent<Seeker>();
        }
    }

    // 阴影
    public void SetShadow(bool show)
    {
        if (show)
        {
            _avatar.AddShadow();
        }
        else
        {
            _avatar.RemoveShadow();
        }
    }

    // 设置透明度
    public void SetAlpha(float alpha)
    {
        if (alpha < 1)
        {
            // 透明，使用半透明的shader
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var item in renderers)
            {
                if (item.GetComponent<ParticleSystem>() == null)
                {
                    item.material.shader = Shader.Find("Shader/Transparent");
                    item.material.SetFloat("_Alpha", alpha);
                }
            }
        }
        else
        {
            // 使用正常的shader，效率高
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var item in renderers)
            {
                if (item.GetComponent<ParticleSystem>() == null)
                    item.material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
            }
        }
    }

    // 添加残影效果
    public void AddAfterImage()
    {
    }

    // 播放光效
    public void CreateParticle(string effectName, AttachType attachType, Vector3 pos, float delay = 0, float duration = 0)
    {
        try
        {
            GameObject prefab = Resources.Load<GameObject>(effectName);
            GameObject go = Instantiate(prefab);

            if (attachType != AttachType.WORLDPOINT)
            {
                go.transform.parent = gameObject.transform;
                go.transform.localPosition = pos;
                // TODO:这里仅是王子冲锋特效偏移的处理，需要进行改动
                go.transform.Rotate(transform.rotation.eulerAngles);
            }
            else
            {
                go.transform.position = pos;
            }

            if (duration > 0)
            {
                Destroy(go, duration);
            }
            else
            {
                _effectDictionary[effectName] = go;
            }
        }
        catch
        {

        }
    }

    // 移除光效
    public void RemoveParticle(string effect)
    {
        GameObject go;
        if (_effectDictionary.TryGetValue(effect, out go))
        {
            Destroy(go);
        }
    }

    public void CreateModel(string modelName, Vector3 pos, float delay = 0, float duration = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(modelName);
        GameObject go = Instantiate(prefab);
        go.name = modelName;
        go.transform.position = pos;

        if (duration > 0)
        {
            Destroy(go, duration);
        }
    }

    // 预载声音文件
    public void PrecacheSound(string soundName)
    {
    }

    // 播放声音
    public void PlaySound(string soundName, bool loop)
    {
    }

    // 停止播放这个实体的特定的声音
    public void StopSound(string soundName)
    {
    }

    // 设置为隐身
    public void SetInvisible(bool value)
    {
        // 隐身 半透明处理 shader修改   也可以参考风暴英雄，添加流光效果
        SetAlpha(value ? 0.1f : 1f);
    }

    // 变身
    public void MorphTo()
    {
        // 变身处理 （属性完全变化成新单位   属性增加只是模型改变     攻击行为可能改变也可能不变）
    }

    // 复活
    public void Reborn()
    {
    }

    // 嘲讽
    public void Taunt()
    {
    }

    // 开始普通攻击
    public void PlayAttack(Actor target)
    {
        TargetAttacking = target;

        if (!string.IsNullOrEmpty(Projectile))
        {
            // 远程攻击
            _fsm.ChangeState(StateID.STATE_PROJECTILE_ATTACK);
        }
        else
        {
            // 近战攻击
            _fsm.ChangeState(StateID.STATE_ATTACK);
        }
    }

    // 显示刀光拖尾
    public void ShowTrail()
    {
    }

    // 隐藏刀光拖尾
    public void HideTrail()
    {
    }

    // 是否正在播放动画
    public bool IsPlayingAnimation(string animName)
    {
        if (_animation == null) return false;
        return _animation.IsPlayingAnimation(animName);
    }

    // 获取当前播放的动画的时间
    public float GetCurrentAnimationLength()
    {
        if (_animation == null) return 0;
        return _animation.GetCurrentAnimationLength();
    }

    // 当前动画是否播放完毕
    public bool IsCurrentAnimationFinish()
    {
        if (_animation == null) return false;
        return _animation.IsCurrentAnimationFinish();
    }

    // 播放动画
    public void PlayAnimation(string animName)
    {
        if (_animation == null) return;
        _animation.PlayAnimation(animName);
    }

    // 动画速度
    public float AnimationSpeed
    {
        get { return _animation == null ? 0 : _animation.Speed; }
        set
        {
            if (_animation != null)
            {
                _animation.Speed = value;
            }
        }
    }

    // 动画暂停和恢复
    public bool IsPauseAnimation
    {
        get { return _animation == null ? false : _animation.IsPauseAnimation; }
        set
        {
            if (_animation != null)
            {
                _animation.IsPauseAnimation = value;
            }
        }
    }

    // 预览状态
    public void Preview()
    {
        if (_fsm != null) _fsm.ChangeState(StateID.STATE_PREVIEW);
    }

    // 部署
    public void Deploy()
    {
        if (_fsm != null) _fsm.ChangeState(StateID.STATE_DEPLOY);
    }

    // 待机状态(停止移动和攻击)
    public void Idle()
    {
        if (_fsm != null) _fsm.ChangeState(StateID.STATE_IDLE);
    }

    // 移动到目标点
    public void MoveToPosition(Vector3 pos)
    {
        TargetPosition = pos;

        if (Vector3.Distance(Position, pos) <= 0.5f)
        {
            return;
        }

        // 切到移动状态
        if (_fsm != null) _fsm.ChangeState(StateID.STATE_MOVE_TO_POSITION);
    }

    // 移动到目标
    public void MoveToTarget(Actor target)
    {
        TargetPursuing = target;

        if (Vector3.Distance(Position, target.Position) <= 0.5f)
        {
            return;
        }

        // 切到移动状态
        if (_fsm != null) _fsm.ChangeState(StateID.STATE_MOVE_TO_TARGET);
    }

    // 转向目标
    public void TurnToTarget(Vector3 pos)
    {
        transform.LookAt(pos);
    }

    // 击退
    public void Knockback(Vector3 centerPos, int power)
    {
        // 添加力需要 rigibody 支持
        Vector3 knockBackDir = centerPos - transform.position;
        transform.Translate(knockBackDir.normalized * power);
    }

    // 更新buff
    private void UpdateBuff()
    {
        // 检测buff时间，移除到时间的buff
        bool needToRemove = false;
        bool needToUpdate = false;

        // 检测属性变化（脚本通知c#更新数据）
        foreach (var item in _buffList)
        {
            if (item.PropertyDirty)
            {
                needToUpdate = true;
                break;
            }
        }

        // 检测buff移除
        foreach (var item in _buffList)
        {
            if (item.NeedToRemove)
            {
                needToRemove = true;
                continue;
            }

            item.OnTick();

            if (item.Duration > 0 && item.RemainTime > 0)
            {
                item.RemainTime -= GameConfig.FRAME_INTERVAL;
                if (item.RemainTime <= 0)
                {
                    //if (item.Name == "aura_poison")
                    //{
                    //    Debug.Log("=--------------------");
                    //}
                    item.OnDestroy();
                    needToRemove = true;
                }
            }
        }

        // 移除buff
        if (needToRemove)
        {
            _buffList.RemoveAll(x => x.Duration > 0 && x.RemainTime <= 0 || x.NeedToRemove);
        }

        // 重新计算角色属性
        if (needToRemove || needToUpdate)
        {
            UpdateProperty();
        }
    }

    // 添加 buff 或者光环
    public void AddBuff(string buffName, int duration, Skill skill, bool isStack = false)
    {
        if (string.IsNullOrEmpty(buffName)) return;

        // buff 不能叠加则返回
        if (!isStack)
        {
            foreach (var item in _buffList)
            {
                if (item.Name == buffName) return;
            }
        }
        Buff buff = new Buff();
        buff.Name = buffName;
        buff.Script = buffName;
        buff.Owner = this;
        buff.Skill = skill;
        buff.CreateTime = BattleTime.GetTime();
        buff.Duration = duration;
        buff.RemainTime = duration;
        _buffList.Add(buff);
        buff.OnCreate();
        // 更新玩家属性
        UpdateProperty();
    }

    // 根据名字获取buff
    public Buff GetBuff(string buffName)
    {
        return _buffList.Find(x => x.Name == buffName);
    }

    // 根据名字移除buff
    public void RemoveBuff(string buffName)
    {
        //Log.Info("RemoveBuff: " + buffName);
        _buffList.RemoveAll(x => x.Name == buffName);

        // 更新玩家属性
        UpdateProperty();
    }

    // 清理所有的buff 
    public void ClearBuff()
    {
        foreach (var item in _buffList)
        {
            item.OnDestroy();
        }

        _buffList.Clear();
        UpdateProperty();
    }

    // 是否持有某个buff
    public bool HasBuff(string modifierName)
    {
        Buff buff = GetBuff(modifierName);
        return buff != null;
    }

    // 设置角色状态，如眩晕、无敌
    public void SetStatus(StatusFlag flag, bool value)
    {
        // TODO  设置状态时判定当前状态时间和要设置的状态时间，取长的
        _status[(int)flag] = value;
    }

    // 检测角色状态
    public bool CheckStatus(StatusFlag flag)
    {
        return _status[(int)flag];
    }

    // 目标查找
    public Actor FindTarget(float radius, TeamFlag teamFlag, TargetFlag targetFlag)
    {
        return BattleController.Instance.FindTarget(UserID, Position, radius, teamFlag, targetFlag, FindType.FIND_CLOSEST);
    }

    public Actor[] FindTargets(float radius, TeamFlag teamFlag, TargetFlag targetFlag)
    {
        return BattleController.Instance.FindTargets(UserID, Position, radius, teamFlag, targetFlag);
    }

    // 设置是否允许碰撞
    public void SetCollisionEnable(bool value)
    {
        if (value)
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
        }
        else
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    // 调用脚本函数
    public object[] CallFunction(string funcName, params object[] args)
    {
        if (ConfigID == 0)
        {
            return null;
        }

        if (string.IsNullOrEmpty(Cfg.Script))
        {
            // 没有对应的脚本
            return null;
        }

        if (args.Length > 0)
        {
            // 有参数
            object[] param = new object[args.Length + 1];
            param[0] = this;
            for (int i = 0; i < args.Length; ++i)
            {
                param[i + 1] = args[i];
            }
            return LuaManager.Instance.CallFunction(Cfg.Script, funcName, param);
        }
        else
        {
            // 无参数，只需要传递self
            return LuaManager.Instance.CallFunction(Cfg.Script, funcName, this);
        }
    }

    // 开始思考（调用lua的Think函数）
    public void StartThink(int interval = 250)
    {
        _thinkInterval = interval;
        _lastThinkTime = 0;
    }

    // 每帧思考
    private void OnThink()
    {
        if (_thinkInterval <= 0)
        {
            return;
        }

        if (BattleTime.GetTime() - _lastThinkTime >= _thinkInterval)
        {
            _lastThinkTime = BattleTime.GetTime();
            CallFunction("OnThink");
        }
    }

    // 设置跑步时脚下的特效
    public void SetMoveEffect(string prefab)
    {

    }

    // 召唤id，主要用于导出给lua
    public int GetSummonID()
    {
        return Cfg.SummonID;
    }

    // 相对召唤等级
    public int GetSummonLevel()
    {
        return Cfg.SummonLevel;
    }

    // 召唤数量
    public int GetSummonNumber()
    {
        return Cfg.SummonNumber;
    }

    // 每次召唤间隔时间
    public int GetSummonPauseTime()
    {
        return Cfg.SummonPauseTime;
    }

    // 建筑物持续时间
    public int GetBuildingLifeDuration()
    {
        return Cfg.LifeDuration;
    }

    // 停止状态机更新（用于角色召唤单位或者释放技能的时候）
    public void PauseFSM()
    {
        _isPauseFSM = true;
    }

    // 恢复状态机更新
    public void ResumeFSM()
    {
        _isPauseFSM = false;
    }

    // 搜寻目标
    public Actor ScanTarget()
    {
        if (IsBuilding())
        {
            // 如果是建筑物的话
            Actor target = FindTarget(GetAtkRange(), TeamFlag.ENEMY, TargetFlag.ALL);
            if (target != null)
            {
                return target;
            }
        }
        else
        {
            // 如果是单位的话
            // 先获取视野内的敌人，如果有的话，优先攻击
            Actor target = FindTarget(GetSightRange(), TeamFlag.ENEMY, TargetFlag.ALL);
            if (target != null)
            {
                return target;
            }

            // 视野内没有目标，则寻找应该攻击的敌方塔
            target = BattleController.Instance.FindTower(UserID, Position);
            if (target != null)
            {
                return target;
            }
        }

        return null;
    }

    // 建筑物自动掉血
    public void ProcessBuilding()
    {
        if (!IsBuilding() || GetBuildingLifeDuration() <= 0 || !DeployFinished) return;
        OnHit(_lifeReducePerTurn);
    }

    // TODO:设置开火点，（一开始就设置好，这样会跟着物体旋转）
    public void SetProjectileSpawnPoint(Vector3 spawnPosition)
    {
        GameObject go = new GameObject(name + "_FirePoint");
        go.transform.SetParent(Model.transform);
        go.transform.localPosition = spawnPosition;
        ProjectileSpawnPoint = go.transform;
    }

    // 角色推挤位移
    public void ApplyCollisionMove(float delta)
    {
        if (IsBuilding() || !DeployFinished) return;

        Vector3 dir = Vector3.zero;

        List<Actor> allActors = BattleController.Instance.GetActorList();
        foreach (var item in allActors)
        {
            // 空中单位与地面单位无需避让
            if (item == null || item == this || item.FlyingHeight - FlyingHeight != 0 || item.IsBuilding())
            {
                continue;
            }

            if (Vector3.Distance(Position, item.Position) <= Cfg.CollisionRadius/1000f * 6.5f)
            {
                // 推挤的位移受单位配置的质量影响
                Vector3 toNeibour = (Position - item.Position) / 2 * Mathf.Max(1, item.Cfg.Mass/6f);
                dir += toNeibour;
            }
        }

        if (dir.magnitude > 0)
        {
            Position += dir;
        }
    }
}