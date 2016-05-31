using UnityEngine;
using System.Collections.Generic;

// 动画控制组件
public class AnimationComponent : ActorComponent
{
    // 攻击动作组别
    public enum AttackGroup
    {
        EMPTY = 0,  // 空手
        LSLASH = 1,
        RSLASH = 2,
        LWAND = 3,
        RWAND = 4,
        POLEARM = 5,
        RIFLE = 6,
        BOW = 7,
        CROSSBOW = 8,
        LPISTOL = 9,
        RPISTOL = 10,
    }

    private const string ACTION_NAME = "action";
    private const string SUB_ACTION_NAME = "subaction";
    private const string BASE_LAYER = "Base Layer"; // 动画层名字
    private const string ATTACK_STATE = "attack"; // 攻击子状态机名字
    private const string IDLE_NAME = BASE_LAYER + "." + "idle";
    private const string RUN_NAME = BASE_LAYER + "." + "run";

    protected Animator _animator;
    private bool _isPauseAnimation = false;

    private int _lastAttackActionIndex = -1; // 上次攻击的动画序号
    private AttackGroup _lastAttackGroup = AttackGroup.EMPTY; // 上次攻击的组别，如果可以的话，左右手轮流攻击
    private bool _isAttacking = false;
    private Dictionary<int, List<string>> _attackDictionary = new Dictionary<int, List<string>>(); // 攻击动画

    private AttackGroup _rightHandAttackGroup = AttackGroup.EMPTY; // 右手为主武器手 外部逻辑进行设置，决定了选择哪组动画进行播放
    private AttackGroup _leftHandAttackGroup = AttackGroup.EMPTY; // 左手为空的时候不会进行攻击
    private List<int> _rightHandAttackList = new List<int>(); // 左手的攻击动画
    private List<int> _leftHandAttackList = new List<int>(); // 右手的攻击动画

    private string _curAnimationName;
    private Actor _actor;

    public override void OnInit(Actor actor)
    {
        _actor = actor;
    }

    public override void OnDestory()
    {
    }

    public override void OnModelChanged()
    {
        // 更换模型后，重新获取animator属性
        if (_actor != null && _actor.Model != null)
        {
            _animator = _actor.Model.GetComponent<Animator>();
        }
    }

    public override void OnUpdate(float dt)
    {
    }

    // 设置右手攻击动画组别
    public void SetRightHandAttackGroup(AttackGroup attackGroup)
    {
        _rightHandAttackGroup = attackGroup;
        SetAttackList(_rightHandAttackGroup, _rightHandAttackList);
    }

    // 设置左手的攻击动画组别
    public void SetLeftHandAttackGroup(AttackGroup attackGroup)
    {
        _leftHandAttackGroup = attackGroup;
        SetAttackList(_leftHandAttackGroup, _leftHandAttackList);
    }

    // 设置攻击序号，这些序号跟controller中的配置相关
    private void SetAttackList(AttackGroup group, List<int> list)
    {
        list.Clear();
        switch (group)
        {
            case AttackGroup.EMPTY:
                list.AddRange(new[] { 1, 2, 3, 4, 5 });
                break;
            case AttackGroup.LSLASH:
                list.AddRange(new[] { 11, 12, 13, 14 });
                break;
            case AttackGroup.RSLASH:
                list.AddRange(new[] { 21, 22, 23, 24 });
                break;
            case AttackGroup.LWAND:
                list.Add(31);
                break;
            case AttackGroup.RWAND:
                list.Add(41);
                break;
            case AttackGroup.POLEARM:
                list.AddRange(new[] { 51, 52, 53 });
                break;
            case AttackGroup.RIFLE:
                list.Add(61);
                break;
            case AttackGroup.BOW:
                list.AddRange(new[] { 71, 72 });
                break;
            case AttackGroup.CROSSBOW:
                list.Add(81);
                break;
            case AttackGroup.LPISTOL:
                list.Add(91);
                break;
            case AttackGroup.RPISTOL:
                list.Add(101);
                break;
        }
    }

    // 设置action，action是持续的状态，由上层逻辑负责重置action。subAction是大的状态中的小分支，跟具体动作相关
    public void SetAction(AnimationAction action, int subAction = 0)
    {
        SetAction((int)action, subAction);
    }

    public void SetAction(int action, int subAction = 0)
    {
        if (_animator == null) return;
        _animator.SetInteger(ACTION_NAME, action);
        _animator.SetInteger(SUB_ACTION_NAME, subAction);
    }

    // 播放动画，对应controller中的trigger的名字
    public void Play(string animName)
    {
        if (_animator == null) return;
        _animator.SetTrigger(animName);
    }

    public void Play(string animName, bool value)
    {
        if (_animator == null) return;
        _animator.SetBool(animName, value);
    }

    // 是否正在播放动画
    public bool IsPlayingAnimation(string animName)
    {
        if (_animator == null) return false;

        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName(animName);
    }

    // 获取当前播放的动画的时间
    public float GetCurrentAnimationLength()
    {
        if (_animator == null)
        {
            return 0;
        }

        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        return state.length;
    }

    // 当前动画是否播放完毕
    public bool IsCurrentAnimationFinish()
    {
        if (_animator == null)
        {
            return true;
        }

        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        //return state.normalizedTime >= 0.9f;
        return state.normalizedTime >= 1f;
    }

    public void PlayAnimation(string animName)
    {
        if (_animator == null) return;

        if (_curAnimationName == animName)
        {
            return;
        }

        if (IsPlayingAnimation(animName))
        {
            return;
        }

        _curAnimationName = animName;
        _animator.CrossFade(animName, 0.1f);
        //_animator.Play(animName);
    }

    // 动画速度
    public float Speed
    {
        get { return _animator.speed; }
        set
        {
            _animator.speed = value;
        }
    }

    // 动画是否有效
    public bool IsEnabled
    {
        get { return _animator.enabled; }
        set { _animator.enabled = value; }
    }

    // 动画暂停和恢复
    public bool IsPauseAnimation
    {
        get { return _isPauseAnimation; }
        set
        {
            if (_animator == null) return;
            if (_isPauseAnimation)
            {
                _animator.speed = 0;
            }
            else
            {
                _animator.speed = 1;
            }
        }
    }

    // 站立
    public void StandBy()
    {
        // 清理攻击数据
        _lastAttackActionIndex = -1;
        _lastAttackGroup = AttackGroup.EMPTY;

        SetAction(AnimationAction.IDLE);
    }

    // 跑步
    public void Run()
    {
        SetAction(AnimationAction.RUN);
    }

    // 站立不动的时候，随机会向周围看
    public void Fidget()
    {
        SetAction(AnimationAction.FIDGET, Random.Range(1, 4));
        _actor.AddFrameEvent(() =>
        {
            SetAction(AnimationAction.IDLE);
        });
    }

    // 死亡动画
    public void Die(int index = 0)
    {
        if (index == 0)
        {
            List<int> actionList = new List<int> { 1, 2, 3, 4 };
            SetAction(AnimationAction.DIE, GetRandomAction(actionList));
        }
        else
        {
            SetAction(AnimationAction.DIE, index);
        }
    }

    // 攻击
    public void Attack()
    {
        // 只有站立和跑步的时候可以切换到攻击
        AnimatorStateInfo animatorState = _animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName(IDLE_NAME) || animatorState.IsName(RUN_NAME))
        {
            int index = 0;
            if (_leftHandAttackGroup != AttackGroup.EMPTY)
            {
                // 如果有左手动画，则左右手轮流播放
                if (_lastAttackGroup == _rightHandAttackGroup)
                {
                    // 如果上次播放的是右手动画，则此次播放左手动画
                    index = GetRandomAction(_leftHandAttackList, _lastAttackActionIndex);
                    _lastAttackGroup = _leftHandAttackGroup;
                }
                else
                {
                    // 其余情况播放右手动画
                    index = GetRandomAction(_rightHandAttackList, _lastAttackActionIndex);
                    _lastAttackGroup = _rightHandAttackGroup;
                }
            }
            else
            {
                index = GetRandomAction(_rightHandAttackList, _lastAttackActionIndex);
            }

            SetAction(AnimationAction.ATTACK, index);

            // 延迟几帧清理设置的动画参数
            _actor.AddFrameEvent(() =>
            {
                // 仅需要处理参数
                SetAction(AnimationAction.IDLE);
            });
        }
    }

    // 获取一个随机动画序号
    private int GetRandomAction(List<int> list, int exclude = 0)
    {
        // 错误情况
        if (list.Count == 0)
        {
            return 1;
        }

        // 只有一个动画
        if (list.Count == 1)
        {
            return list[0];
        }

        int value = 0;
        for (int i = 0; i < 5; ++i)
        {
            value = list[Random.Range(0, list.Count)];
            if (value != exclude)
            {
                return value;
            }
        }
        return value;
    }

    public void SetMoveAnimSpeed()
    {

    }

    public void SetAttackAnimSpeed()
    {

    }
}
