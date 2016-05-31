using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ETouch;
using Lockstep;
using message;
using UnityEngine.SceneManagement;

// 战斗模式
public enum BattleType
{
    PVE, // pve 客户端离线战斗
    PVP, // pvp 实时在线pvp
    REPLAY, // 录像重放
}

// 战斗控制 负责战斗流程
public partial class BattleController : Singleton<BattleController>
{
    // 当前状态
    private enum State
    {
        NONE, // 没有在比赛
        PREPARE, // 开始准备比赛
        GAME, // 比赛中
        FINISH, // 当前比赛已经结束
    }

    public int BattleSpeed = 1; // 录像时使用，战斗播放速度
    public int ServerTurnIndex = 0; // 服务器同步的回合
    public int ServerTimestamp = 0; // 服务器的时间戳
    public int ClientTurnIndex = 0; // 客户端当前执行到的回合
    public int Mana = 0;    // 当前圣水
    public long UserID = 0; // 用户ID，暂时由战斗服务器生成
    public int UserLevel = 1;   // 玩家等级
    public long EnemyUserID = 0;    // 敌方玩家id
    public int EnemyUserLevel = 1;  // 敌方玩家等级
    public bool IsHomeCourt; // 是否是主场，执行单位逻辑的时候先运算主场单位
    public string Token = ""; // 游戏服务器从战斗服务器获取到的登录token，也用于身份识别
    public string ServerIP; // 战斗服务器ip
    public CardInfo PreviewCard; // 下一张卡牌
    public CardInfo SelectedCard; // 当前选中的卡牌
    public List<CardInfo> CardList = new List<CardInfo>(); // 我的卡组
    public List<CardInfo> HandCardList = new List<CardInfo>();  // 手牌卡组，手牌（包括预览牌）不能有相同的卡牌，即玩家出牌不可能出两张相同的牌（但是如果单位未死，是可能出现相同单位的）
    public List<CardInfo> DeckCardList = new List<CardInfo>();  // 牌库卡组

    private int _arenaID; // 当前加载的竞技场(地图)配置
    private int _generatedActorID = 0; // actor的实例id，整场比赛唯一
    private int _accumilatedTime = 0; // 计时，用于控制逻辑帧间隔
    private bool _isGamePaused = false; // 游戏是否暂停，pve和录像可以暂停，pvp不可以
    private string _currentSceneName; // 当前场景名字
    private State _battleState; // 战斗的状态
    private Camera _camera; // 场景摄像机
    private MapGrid _mapGrid; // 地面格子
    private BattleType _battleType;
    private Queue<message.SynAction> _actionQueue = new Queue<SynAction>(); // 操作序列
    private List<Actor> _previewModelList = new List<Actor>(); // 预览模型列表
    private List<Actor> _actorList = new List<Actor>(); // 场景中的所有角色
    private List<Skill> _skillList = new List<Skill>(); // 释放的技能列表 
    private List<Projectile> _projectileList = new List<Projectile>();    // 抛射物列表
    private List<Actor> _dummyActorList = new List<Actor>();    // 马甲单位列表 
    private GameObject _goPreview;
    private Vector2 _myKingTowerCell;   // 我方国王塔的位置，用于释放法术
    private Vector2 _enemyKingTowerCell;    // 敌方国王塔的位置
    private List<Actor> _enemyTower = new List<Actor>(); // 敌方的塔，0和1为公主塔，2为国王塔

    private void Awake()
    {
        RegisterMsg();

        // 初始化lua脚本系统
        LuaManager.Instance.Init();
    }

    private void OnEnable()
    {
        EasyTouch.On_SimpleTap += OnTap;
    }

    private void OnDisable()
    {
        EasyTouch.On_SimpleTap -= OnTap;
    }

    private void Update()
    {
        if (_battleState != State.GAME)
        {
            return;
        }

        // 控制帧率
        _accumilatedTime += (int)(Time.deltaTime * 1000);

        // 更新单位
        float dt = Time.deltaTime;

        // 渲染帧更新
        OnUpdate(dt);

        // 如果fps太低，则一次更新可能会更新多次逻辑帧
        while (_accumilatedTime >= GameConfig.FRAME_INTERVAL)
        {
            OnTick();
            _accumilatedTime = _accumilatedTime - GameConfig.FRAME_INTERVAL;
        }
    }

    // 暂停或者继续游戏
    public void SetGamePaused(bool pause)
    {
        _isGamePaused = pause;
    }

    // 返回游戏是否被暂停了
    public bool IsGamePaused()
    {
        return _isGamePaused;
    }

    // 开始pve单机战斗
    public void StartBattle(int mapID, BattleType battleType)
    {
        if (_battleState != State.NONE)
        {
            // 已经在比赛中
            return;
        }

        // 显示ui
        UIManager.Instance.OpenWindow<UIBattleView>();

        // 竞技场配置数据
        var cfg = ArenaConfigLoader.GetConfig(mapID);
        if (cfg == null)
        {
            return;
        }

        _arenaID = mapID;
        _battleType = battleType;

        // 加载场景
        StartCoroutine(LoadScene(cfg.Map));

        //if (_battleType == BattleType.PVP) {
        //    // pvp战斗，先连接战斗服务器
        //    ConnectToBattleServer();
        //} 
    }

    // 加载场景
    public IEnumerator LoadScene(string mapName)
    {
        _battleState = State.PREPARE;
        _currentSceneName = mapName;

        // 加载场景
        yield return SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);

        // 设置当前场景（新创建的单位都创建在这个场景中）
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentSceneName));
        _camera = Camera.main;

        _mapGrid = GameObject.FindObjectOfType<MapGrid>();

        // 场景加载完毕，等待服务器确认游戏开始
        if (_battleType == BattleType.PVE)
        {
            // pve开始战斗
            UserID = 1;
            S2C_StartBattle data = new S2C_StartBattle();
            data.UserID1 = UserID;
            data.UserID2 = UserID + 1;
            data.RandomSeed = 0;
            data.UserLevel1 = UserLevel;
            data.UserLevel2 = UserLevel;
            OnStartBattle(data);
        }
        else
        //(_battleType == BattleType.PVP)
        {
            // pvp战斗，先连接战斗服务器
            ConnectToBattleServer();
        }
    }

    // 卸载场景
    public void UnLoadScene()
    {
        _mapGrid = null;
        _camera = null;
        SceneManager.UnloadScene(_currentSceneName);
    }

    // 世界坐标转为格子坐标
    public Vector3 WorldToCell(Vector3 pos)
    {
        if (_mapGrid != null)
        {
            Vector2 cellPos = _mapGrid.WorldToCell(pos);
            return cellPos;
        }
        return Vector3.zero;
    }

    // 格子坐标转为世界坐标
    public Vector3 CellToWorld(Vector2 cell)
    {
        if (_mapGrid != null)
        {
            return _mapGrid.CellToWorld(cell);
        }
        return Vector3.zero;
    }

    // 屏幕坐标(触摸坐标)转换为世界坐标
    public Vector3 ScreenToWorld(Vector2 point)
    {
        if (_camera == null) return Vector3.zero;
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, GameLayer.GroundMask))
        {
            Vector3 pos = hit.point;
            return pos;
        }
        return Vector3.zero;
    }

    // 屏幕坐标转为格子坐标
    public Vector2 ScreenToCell(Vector2 position)
    {
        Vector3 pos = ScreenToWorld(position);
        return WorldToCell(pos);
    }

    // 添加卡牌
    public void AddCard(int cfgID, int level)
    {
        var cardInfo = new CardInfo();
        cardInfo.ConfigID = cfgID;
        cardInfo.Level = level;
        CardList.Add(cardInfo);
    }

    // 根据卡牌创建场上的单位
    private void CreateCardActor(CardInfo info, Vector2 cellPos, long userID, bool isPreview)
    {
        // 解析阵型数据
        List<Vector3> list = null;
        if (info.Cfg.Formation > 0)
        {
            FormationConfig formation = FormationConfigLoader.GetConfig(info.Cfg.Formation);
            if (formation != null)
            {
                list = new List<Vector3>();
                string[] split = formation.Positions.Split(' ');
                foreach (var item in split)
                {
                    string txt = item.Trim('(', ')');
                    string[] txtPos = txt.Split(',');
                    Vector3 pos = new Vector3(Convert.ToInt32(txtPos[0]) * 6.5f, 0, Convert.ToInt32(txtPos[1]) * 6.5f);
                    list.Add(pos);
                }
            }
        }

        if (list == null)
        {
            list = new List<Vector3>();
            list.Add(Vector3.zero);
        }

        Vector3 worldPos = CellToWorld(cellPos);

        // 创建相应数量的单位
        for (int i = 0; i < info.Cfg.ArmyNumber; ++i)
        {
            // 模型
            GameObject go = new GameObject();
            Actor actor = go.AddComponent<Actor>();

            // 数据
            actor.ConfigID = info.ConfigID;
            actor.EntityID = ++_generatedActorID;
            actor.Level = info.Level;
            actor.UserID = userID;
            go.name = actor.Cfg.Name + actor.EntityID;
            actor.OnActorDeath = OnActorDeath;

            // 初始化
            actor.OnInit();

            // 位置
            if (i < list.Count)
            {
                actor.PreviewOffset = list[i];
            }
            else
            {
                actor.PreviewOffset = Vector3.zero;
            }
            actor.SetPosition(worldPos + actor.PreviewOffset);
            bool isEnemy = actor.IsEnemy();

            // 右边的模型统一旋转180度
            if (IsHomeCourt)
            {
                // 主场玩家，敌人旋转
                if (isEnemy)
                {
                    actor.transform.Rotate(Vector3.up, -180);
                }
            }
            else
            {
                // 客场玩家，我方旋转
                if (!isEnemy)
                {
                    actor.transform.Rotate(Vector3.up, -180);
                }
            }

            // 设置物理和寻路组件
            actor.SetupPhysic();

            if (isPreview)
            {
                // 预览状态 此状态的单位可能会被随时取消掉，不会影响到战斗
                actor.Preview();
                _previewModelList.Add(actor);
            }
            else
            {
                // 部署状态
                _actorList.Add(actor);
                actor.Deploy();
            }
        }
    }

    // 创建单位
    public Actor CreateActor(int cfgID, int level, long userID, Vector3 pos)
    {
        GameObject go = new GameObject();
        Actor actor = go.AddComponent<Actor>();

        // 数据
        actor.ConfigID = cfgID;
        actor.EntityID = ++_generatedActorID;
        actor.Level = level;
        actor.UserID = userID;
        go.name = actor.Cfg.Name + actor.EntityID;
        actor.OnActorDeath = OnActorDeath;

        // 初始化
        actor.OnInit();
        actor.OnSpawn();

        actor.SetPosition(pos);

        // 设置物理和寻路组件
        actor.SetupPhysic();


        _actorList.Add(actor);
        actor.Idle();


        return actor;
    }

    // 创建虚拟actor，用于光环buff的实现
    public Actor CreateDummyActor(long userID, Vector3 pos)
    {
        GameObject go = new GameObject();
        Actor actor = go.AddComponent<Actor>();

        // 数据
        actor.EntityID = ++_generatedActorID;
        actor.UserID = userID;
        actor.OnActorDeath = OnActorDeath;
        actor.IsDummy = true;

        // 初始化
        actor.OnInit();
        actor.SetPosition(pos);

        _dummyActorList.Add(actor);
        return actor;
    }

    // 创建抛射物
    public Projectile CreateProjectile(string projName)
    {
        GameObject go = new GameObject(projName);
        Projectile proj = go.AddComponent<Projectile>();
        proj.OnProjectileHit = OnProjectileHit;
        _projectileList.Add(proj);
        return proj;
    }

    private void OnProjectileHit(Projectile proj)
    {
    }

    // 根据配置，创建场上的塔（它们不是由玩家卡牌创建的，其等级是由玩家等级决定的）
    public Actor CreateTower(int cfgID, int level, string cfgPos, long userID)
    {
        string[] split = cfgPos.Split(',');
        Vector2 pos = new Vector2(System.Convert.ToInt32(split[0]), System.Convert.ToInt32(split[1]));

        GameObject go = new GameObject();
        Actor actor = go.AddComponent<Actor>();

        // 数据
        actor.EntityID = ++_generatedActorID;
        actor.ConfigID = cfgID;
        actor.Level = level;
        actor.UserID = userID;
        go.name = actor.Cfg.Name + actor.EntityID;
        actor.OnActorDeath = OnActorDeath;

        // 初始化
        actor.OnInit();

        actor.SetPosition(CellToWorld(pos));

        bool isEnemy = actor.IsEnemy();

        // 右边的塔旋转
        if (IsHomeCourt)
        {
            // 主场玩家，敌人旋转
            if (isEnemy)
            {
                actor.transform.Rotate(Vector3.up, -180);
            }
        }
        else
        {
            // 客场玩家，自己旋转
            if (!isEnemy)
            {
                actor.transform.Rotate(Vector3.up, -180);
            }
        }

        // 如果是国王塔，记录其坐标，以便将来释放法术
        if (cfgID == GameConfig.KING_TOWER_ID)
        {
            if (isEnemy)
            {
                _enemyKingTowerCell = pos;
            }
            else
            {
                _myKingTowerCell = pos;
            }
        }

        _actorList.Add(actor);

        // 塔直接切换为idle状态，不需要部署
        actor.Idle();
        return actor;
    }

    // 单位死亡时调用
    private void OnActorDeath(Actor actor)
    {
        if (actor.ConfigID == GameConfig.KING_TOWER_ID)
        {
            // 如果是国王塔被摧毁，判定胜负
            if (actor.IsEnemy())
            {
                // 赢了
                UIUtil.ShowMsgFormat("MSG_WIN");
            }
            else
            {
                // 输了
                UIUtil.ShowMsgFormat("MSG_LOSE");
            }

            // 结束战斗
            OnFinishBattle();
            _enemyTower.Remove(actor);
        }
        else if (actor.ConfigID == GameConfig.TOWER_ID)
        {
            // 如果是公主塔被摧毁，解锁投兵区域
            _enemyTower.Remove(actor);

        }
    }

    // 返回所有角色列表
    public List<Actor> GetActorList()
    {
        return _actorList;
    }

    // 寻找范围内的单位
    public Actor FindTarget(long ownerID, Vector3 pos, float radius, TeamFlag teamFlag, TargetFlag targetFlag, FindType findType)
    {
        List<Actor> allActors = GetActorList();
        List<Actor> list = new List<Actor>();
        foreach (var item in allActors)
        {
            // TODO 这里应该有错误
            if (item == this)
            {
                continue;
            }

            if (Vector3.Distance(pos, item.Position) > radius)
            {
                // 超出范围
                continue;
            }

            if (teamFlag == TeamFlag.ENEMY && item.UserID == ownerID)
            {
                // 要求目标是敌人 但是跟目标是同一队伍
                continue;
            }
            else if (teamFlag == TeamFlag.FRIENDLY && item.UserID != ownerID)
            {
                // 要求目标是友方 但是跟目标不是同一队伍
                continue;
            }

            // 炸弹等不能被作为攻击目标
            if (item.CanBeTarget)
                list.Add(item);
        }

        if (list.Count <= 0)
        {
            return null;
        }

        if (findType == FindType.FIND_CLOSEST)
        {
            // 最近的
            list.Sort((a, b) =>
            {
                var lenA = Vector3.Distance(pos, a.Position);
                var lenB = Vector3.Distance(pos, b.Position);
                return lenA.CompareTo(lenB);
            });
        }
        else if (findType == FindType.FIND_FARTHEST)
        {
            // 最远的
            list.Sort((a, b) =>
            {
                var lenA = Vector3.Distance(pos, a.Position);
                var lenB = Vector3.Distance(pos, b.Position);
                return lenB.CompareTo(lenA);
            });
        }
        return list[0];
    }

    public Actor[] FindTargets(long ownerID, Vector3 pos, float radius, TeamFlag teamFlag, TargetFlag targetFlag)
    {
        List<Actor> allActors = GetActorList();
        List<Actor> list = new List<Actor>();
        foreach (var item in allActors)
        {
            if (item == this)
            {
                continue;
            }

            if (Vector3.Distance(pos, item.Position) > radius)
            {
                // 超出范围
                continue;
            }

            if (teamFlag == TeamFlag.ENEMY && item.UserID == ownerID)
            {
                // 要求目标是敌人
                continue;
            }
            else if (teamFlag == TeamFlag.FRIENDLY && item.UserID != ownerID)
            {
                // 要求目标是友方
                continue;
            }

            list.Add(item);
        }

        return list.ToArray();
    }

    // 获取应该攻击的塔，这个是全局ai目标
    public Actor FindTower(long userID, Vector3 pos)
    {
        List<Actor> list = new List<Actor>();
        foreach (var item in _enemyTower)
        {
            if (item != null && !item.IsDead)
            {
                if (item.UserID != userID)
                    list.Add(item);
            }
        }

        int count = list.Count;
        if (count <= 0)
        {
            return null;
        }
        else if (count == 1)
        {
            return list[0];
        }
        else
        {
            list.Sort((a, b) =>
            {
                var lenA = Vector3.Distance(pos, a.Position);
                var lenB = Vector3.Distance(pos, b.Position);
                return lenA.CompareTo(lenB);
            });
            return list[0];
        }
    }

    // 处理同步Action （当前仅同步在什么位置出了什么卡牌）
    private void ProcessAction(message.SynAction action)
    {
        switch (action.Action)
        {
            case BattleAction.PUT_CARD:
                if (action.UserID == UserID)
                {
                    // 我方出卡，服务器确认
                    OnUseCardOK(action.CardID, action.Level, new Vector2(action.PosX, action.PosY));
                }
                else
                {
                    // 敌方出卡
                    OnEnemyUseCard(action.CardID, action.Level, new Vector2(action.PosX, action.PosY));
                }
                break;
        }
    }

    // 显示手指上的模型
    public void ShowCursorModel(Vector2 screenPos)
    {
        if (SelectedCard.IsSpell())
        {
            // 如果是法术卡的话，显示作用区域
            if (_goPreview == null)
            {
                var prefab = Resources.Load<GameObject>("Misc/RangePreview");
                _goPreview = Instantiate(prefab);
            }

            // 法术卡的出卡区域没有限制，任何地方都可以
            Vector2 cell = ScreenToCell(screenPos);
            Vector3 world = CellToWorld(cell);

            float scale = SelectedCard.Cfg.DamageRadius / 100f * 2;

            _goPreview.SetActive(true);
            _goPreview.transform.position = world + Vector3.up * 0.1f;

            Vector3 previewScale = Vector3.one * scale;
            previewScale.z = 1;
            _goPreview.transform.localScale = previewScale;
        }
        else
        {
            // 如果是建筑或者是单位的话，显示预览模型
            if (_previewModelList.Count <= 0)
            {
                // 之前没有创建预览模型，创建新的模型
                Vector2 cellPos = FixCell(screenPos);
                CreateCardActor(SelectedCard, cellPos, UserID, true);
            }

            foreach (var item in _previewModelList)
            {
                item.gameObject.SetActive(true);
            }
        }
    }

    // 隐藏手指上的模型(TODO 注意一些bug，快速出卡的时候，后面对卡的操作可能影响之前的预览模型)
    public void HideCursorModel()
    {
        if (_goPreview != null)
        {
            _goPreview.SetActive(false);
        }

        foreach (var item in _previewModelList)
        {
            item.gameObject.SetActive(false);
        }
    }

    // 清理预览模型资源
    public void ClearCursorModel()
    {
        // 预览模型复用性较高，不需要清理
        if (_goPreview != null)
        {
            _goPreview.SetActive(false);
        }

        foreach (var item in _previewModelList)
        {
            Destroy(item.gameObject);
        }

        _previewModelList.Clear();
    }

    // 设置预览模型的坐标
    public void SetCursorModelPosition(Vector2 screenPos)
    {
        Vector2 cell;
        // 法术牌
        if (SelectedCard.IsSpell())
        {
            cell = ScreenToCell(screenPos);
        }
        else
        {
            cell = FixCell(screenPos);
        }

        Vector3 world = CellToWorld(cell);

        if (SelectedCard.IsSpell())
        {
            if (_goPreview != null && _goPreview.activeInHierarchy)
            {
                _goPreview.transform.position = world + Vector3.up * 0.1f; ;
            }
        }
        else
        {
            foreach (var item in _previewModelList)
            {
                item.SetPosition(world + item.PreviewOffset);
            }
        }
    }

    // 本地客户端出卡
    public void UseCard(Vector2 screenPos)
    {
        CardInfo card = SelectedCard;
        if (card == null)
        {
            UIUtil.ShowMsgFormat("MSG_NOT_SELECT_CARD");
            return;
        }

        // 圣水不足，不能出牌
        if (Mana < card.Cfg.Cost * GameConfig.MANA_MUL)
        {
            EventDispatcher.TriggerEvent(EventID.UI_BATTLE_CANCEL_USE_CARD, SelectedCard.Index);
            ClearCursorModel();
            UIUtil.ShowErrMsgFormat("MSG_NOT_ENOUGH_MANA");
            return;
        }

        // 从手牌中移除
        HandCardList.Remove(card);
        // 加入到可以出牌的序列中
        DeckCardList.Add(card);

        if (card.Cfg.Type == (int)CardType.SPELL)
        {
            // 法术卡的出卡区域没有限制
            Vector2 cellPos = ScreenToCell(screenPos);

            // 法术牌不创建单位，而是走技能流程
            if (_goPreview == null || !_goPreview.gameObject.activeInHierarchy)
            {
                // 点击屏幕出卡，生成预览区域
                ShowCursorModel(screenPos);
            }

            ReqSynAction(BattleAction.PUT_CARD, card.ConfigID, card.Level, cellPos);
        }
        else
        {
            Vector2 cellPos = FixCell(screenPos);
            if (_battleType == BattleType.PVE)
            {
                // 我方出卡 不需要服务器，客户端自己处理
                OnUseCardOK(card.ConfigID, card.Level, cellPos);
            }
            else if (_battleType == BattleType.PVP)
            {
                // 我方出卡  实时pvp出卡需要经过服务器确认
                if (_previewModelList.Count <= 0)
                {
                    // 点击出卡，生成预览模型，等服务器确认后，转为部署状态
                    CreateCardActor(SelectedCard, cellPos, UserID, true);
                }
                ReqSynAction(BattleAction.PUT_CARD, card.ConfigID, card.Level, cellPos);
            }
            else if (_battleType == BattleType.REPLAY)
            {
                // 录像模式 完全按照录像文件走
            }
        }
    }

    // 服务器回应卡牌成功使用 预览actor转为部署状态 
    public void OnUseCardOK(int cardID, int level, Vector2 cellPos)
    {
        var card = GetCard(cardID);
        if (card.Cfg.Type == (int)CardType.SPELL)
        {
            // 法术牌不创建单位，而是走技能流程
            // 法术牌没有预览和预部署状态
            CastSkill(card, UserID, _myKingTowerCell, cellPos);

            // 将预览卡放到手牌上
            PushNewCard(card.Index, PreviewCard, 0);

            // 生成一张新的卡牌放在预览卡上面
            GenerateCard();

            // 销毁圣水
            Mana = Mathf.Max(0, Mana - SelectedCard.Cfg.Cost * GameConfig.MANA_MUL);

            // 清除当前选择
            SelectedCard = null;
        }
        else
        {
            // 我方出卡
            foreach (var item in _previewModelList)
            {
                item.Deploy();
            }

            // 添加actor
            _actorList.AddRange(_previewModelList);

            // 将预览卡放到手牌上
            PushNewCard(card.Index, PreviewCard, 0);

            // 生成一张新的卡牌放在预览卡上面
            GenerateCard();

            // 销毁圣水
            Mana = Mathf.Max(0, Mana - SelectedCard.Cfg.Cost * GameConfig.MANA_MUL);

            // 清除当前选择
            SelectedCard = null;
            _previewModelList.Clear();
        }
    }

    // 敌方使用卡牌
    public void OnEnemyUseCard(int cardID, int level, Vector2 cellPos)
    {
        CardInfo card = new CardInfo();
        card.ConfigID = cardID;
        card.EntityID = 0;
        card.Level = level;

        if (card.Cfg.Type == (int)CardType.SPELL)
        {
            // 法术牌不创建单位，而是走技能流程
            // 法术牌没有预览和预部署状态
            CastSkill(card, EnemyUserID, _enemyKingTowerCell, cellPos);
        }
        else
        {
            // 敌方出卡  创建士兵或者建筑
            CreateCardActor(card, cellPos, EnemyUserID, false);
        }
    }

    // 释放技能
    private void CastSkill(CardInfo card, long userID, Vector2 srcPos, Vector2 cellPos)
    {
        Vector3 srcWorldPos = CellToWorld(srcPos);
        Vector3 worldPos = CellToWorld(cellPos);

        // 隐藏技能区域预览
        if (_goPreview != null)
        {
            _goPreview.gameObject.SetActive(false);
        }

        var skill = new Skill();
        skill.OnSkillFinish = OnSkillFinish;
        skill.UserID = userID;
        skill.SrcPosition = srcWorldPos;    // 设置抛射物的起始坐标（只有火球等抛射物技能会用上）
        skill.SelectPosition = worldPos;    // 设定技能目标
        _skillList.Add(skill);

        // 初始化技能
        skill.Init(card.ConfigID, card.Level);

        // 开始播放技能
        skill.Play();
    }

    // 技能释放完毕
    private void OnSkillFinish(Skill skill)
    {
    }

    // 点击场景
    private void OnTap(Gesture gesture)
    {
        if (_battleType == BattleType.REPLAY)
        {
            // 录像模式不能进行操作
            return;
        }

        // 我方出卡
        UseCard(gesture.position);
    }

    // 服务器确认，开始战斗
    public void OnStartBattle(S2C_StartBattle data)
    {
        // 设置随机数种子
        UnityEngine.Random.seed = data.RandomSeed;
        _battleState = State.GAME;
        ServerTurnIndex = 0;
        ClientTurnIndex = 0;
        _accumilatedTime = 0;

        Mana = GameConfig.START_MANA * GameConfig.MANA_MUL;

        // 清理出牌卡组
        DeckCardList.Clear();
        DeckCardList.AddRange(CardList);

        // 创建塔
        ArenaConfig cfg = ArenaConfigLoader.GetConfig(_arenaID);
        if (cfg == null) return;

        _enemyTower.Clear();

        // 判定主客场 只有判定了主客场，才能开始整场战斗
        if (data.UserID1 == UserID)
        {
            // 主场 我在左边
            IsHomeCourt = true;

            // 敌人的id和等级
            EnemyUserID = data.UserID2;
            EnemyUserLevel = data.UserLevel2;

            // 创建我方的塔 (我在左边)
            // 主场玩家左侧为我方塔，右侧为敌方塔，非主场玩家则相反。 将来统一坐标系后再考虑玩家统一在左方，现在双方玩家跟服务器坐标一致，通过塔的颜色来区分
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, UserLevel, cfg.TowerLeft1, UserID));
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, UserLevel, cfg.TowerLeft2, UserID));
            _enemyTower.Add(CreateTower(GameConfig.KING_TOWER_ID, UserLevel, cfg.CastleLeft, UserID));

            // 创建敌方的塔
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, EnemyUserLevel, cfg.TowerRight1, EnemyUserID));
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, EnemyUserLevel, cfg.TowerRight2, EnemyUserID));
            _enemyTower.Add(CreateTower(GameConfig.KING_TOWER_ID, EnemyUserLevel, cfg.CastleRight, EnemyUserID));
        }
        else
        {
            // 客场，我在右边
            IsHomeCourt = false;

            // 敌人的id和等级
            EnemyUserID = data.UserID1;
            EnemyUserLevel = data.UserLevel1;

            // 创建我方的塔 (我在右边)

            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, UserLevel, cfg.TowerRight1, UserID));
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, UserLevel, cfg.TowerRight2, UserID));
            _enemyTower.Add(CreateTower(GameConfig.KING_TOWER_ID, UserLevel, cfg.CastleRight, UserID));

            // 创建敌方的塔
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, EnemyUserLevel, cfg.TowerLeft1, EnemyUserID));
            _enemyTower.Add(CreateTower(GameConfig.TOWER_ID, EnemyUserLevel, cfg.TowerLeft2, EnemyUserID));
            _enemyTower.Add(CreateTower(GameConfig.KING_TOWER_ID, EnemyUserLevel, cfg.CastleLeft, EnemyUserID));
        }

        // 产生4张卡牌
        const float TIME = 0.3f;
        PushNewCard(0, null, 0);
        PushNewCard(1, null, TIME * 1);
        PushNewCard(2, null, TIME * 2);
        PushNewCard(3, null, TIME * 3);

        // 生成下一张卡牌
        GenerateCard();
    }

    // 战斗结束
    public void OnFinishBattle()
    {
        _battleState = State.FINISH;
    }

    // 渲染帧更新
    private void OnUpdate(float dt)
    {
        for (int j = 0; j < _actorList.Count; ++j)
        {
            var actor = _actorList[j];
            actor.OnUpdate(dt);
        }
        // 更新技能
        for (int j = 0; j < _skillList.Count; ++j)
        {
            var skill = _skillList[j];
            skill.OnUpdate(dt);
        }

        // 更新抛射物
        for (int j = 0; j < _projectileList.Count; ++j)
        {
            var proj = _projectileList[j];
            proj.OnUpdate(dt);
        }

        // 更新马甲单位
        for (int j = 0; j < _dummyActorList.Count; ++j)
        {
            var dummy = _dummyActorList[j];
            dummy.OnUpdate(dt);
        }
    }

    // 逻辑帧
    private void OnTick()
    {
        int speed = 1;

        if (_battleType == BattleType.PVP)
        {
            // 实时pvp会考虑网络延迟进行控制，客户端不能比服务器跑的更快
            int offsetIndex = ServerTurnIndex - ClientTurnIndex;
            if (offsetIndex <= 0)
            {
                // 网络卡住了，没有接收到服务器的时间戳，停止游戏逻辑
                return;
            }

            if (offsetIndex <= 3)
            {
                // 差3帧还算正常情况
                speed = 1;
            }
            else if (offsetIndex <= 10)
            {
                // 差的比较多的时候，加快游戏进度，追赶服务器时间
                speed = 2;
            }
            else
            {
                // 积压了非常多的时间戳数据，快进
                speed = 4;
            }
        }
        else if (_battleType == BattleType.REPLAY)
        {
            // 录像模式可以控制播放速度 (TODO 将来做0.5倍慢速播放，现在只能加快播放)
            speed = BattleSpeed;
        }

        // 处理 action （speed 的值决定此次逻辑帧更新次数）
        for (int i = 0; i < speed; ++i)
        {
            // 圣水恢复
            Mana = Mathf.Min(GameConfig.MANA_MAX, Mana + GameConfig.MANA_REGEN);

            // 更新单位 无论实际经过多长时间，对每个逻辑帧而言其间隔时间是固定的
            // 举例来说，当网络卡了，实际时间已经经过1秒钟，但是处理下一个逻辑帧依然按100ms处理，不会出现瞬移的情况
            bool needToRemoveActor = false;
            
            // 遍历更新所有 Actor
            for (int j = 0; j < _actorList.Count; ++j)
            {
                var actor = _actorList[j];
                if (actor.NeedToRemove)
                {
                    needToRemoveActor = true;
                }
                actor.OnTick();
            }

            if (needToRemoveActor)
            {
                _actorList.RemoveAll(x => x.NeedToRemove);
            }

            // 更新技能
            bool needToRemoveSkill = false;
            for (int j = 0; j < _skillList.Count; ++j)
            {
                var skill = _skillList[j];
                if (skill.NeedToRemove)
                {
                    needToRemoveSkill = true;
                    continue;
                }

                skill.OnTick();
            }

            if (needToRemoveSkill)
            {
                _skillList.RemoveAll(x => x.NeedToRemove);
            }

            // 更新抛射物
            bool needToRemoveProj = false;
            for (int j = 0; j < _projectileList.Count; ++j)
            {
                var proj = _projectileList[j];
                if (proj.NeedToRemove)
                {
                    needToRemoveProj = true;
                    continue;
                }
                proj.OnTick();
            }

            if (needToRemoveProj)
            {
                _projectileList.RemoveAll(x => x.NeedToRemove);
            }

            // 更新马甲单位
            bool needToRemoveDummy = false;
            for (int j = 0; j < _dummyActorList.Count; ++j)
            {
                var dummy = _dummyActorList[j];
                if (dummy.NeedToRemove)
                {
                    needToRemoveDummy = true;
                    continue;
                }

                dummy.OnTick();
            }

            if (needToRemoveDummy)
            {
                _dummyActorList.RemoveAll(x => x.NeedToRemove);
            }

            // 处理事件同步事件
            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Peek();
                if (action.TurnIndex <= ClientTurnIndex)
                {
                    // 处理当前逻辑帧的事件
                    ProcessAction(action);
                    _actionQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }

            // 刷新ui时间
            EventDispatcher.TriggerEvent(EventID.UI_BATTLE_REFRESH_TIME);

            // 更新客户端回合
            ++ClientTurnIndex;
        }
    }

    // 获取卡牌
    public CardInfo GetCard(int cardID)
    {
        return CardList.Find(x => x.ConfigID == cardID);
    }

    // 随机取一张卡牌
    public CardInfo RandomCard()
    {
        // 从卡组中随机取一个牌
        int index = UnityEngine.Random.Range(0, DeckCardList.Count);
        var cardInfo = DeckCardList[index];

        // 加入到手牌中（包括预览牌）
        HandCardList.Add(cardInfo);
        DeckCardList.Remove(cardInfo);
        return cardInfo;
    }

    // 添加一张牌到手牌
    public void PushNewCard(int index, CardInfo card = null, float delay = 0)
    {
        if (card == null)
        {
            card = RandomCard();
        }

        card.Index = index;
        EventDispatcher.TriggerEvent(EventID.UI_BATTLE_PUSH_NEW_CARD, card, delay);
    }

    // 产生一个新卡牌到预览牌
    public void GenerateCard()
    {
        PreviewCard = RandomCard();
        PreviewCard.Index = -1;

        // 触发预览卡刷新
        EventDispatcher.TriggerEvent(EventID.UI_BATTLE_PREVIEW_CARD, PreviewCard);
    }

    // 设定可出牌的地点 受地图边界和击毁对方塔的情况决定
    public Vector2 FixCell(Vector2 position)
    {
        Vector2 cell = ScreenToCell(position);
        if (IsHomeCourt)
        {
            // 主场玩家在左边，跟服务器视角一致
            if (cell.y > GameConfig.GRID_CELL_Y / 2 - 1)
            {
                cell.y = GameConfig.GRID_CELL_Y / 2 - 1;
            }
        }
        else
        {
            // 客场玩家在右边
            if (cell.y < GameConfig.GRID_CELL_Y / 2 + 1)
            {
                cell.y = GameConfig.GRID_CELL_Y / 2 + 1;
            }
        }

        return cell;
    }

    // 播放光效
    public void CreateParticle(string effectName, AttachType attachType, Vector3 pos, float delay = 0, float duration = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(effectName);
        GameObject go = Instantiate(prefab);
        go.transform.position = pos;
        if (duration > 0)
        {
            // TODO:这里的销毁时间是Unity时间，需要改为服务器时间控制
            Destroy(go, duration + 5);
        }
    }
}