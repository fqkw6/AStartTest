using UnityEngine;
using System.Collections.Generic;

// Actor中的组件和状态部分
public partial class Actor
{
    protected FSMStateMachine _fsm; // 状态机
    protected HUDComponent _hud;    // hud显示组件
    protected AvatarComponent _avatar;  // 换装组件
    protected AnimationComponent _animation;    // 动画组件

    protected Dictionary<string, ActorComponent> _components = new Dictionary<string, ActorComponent>();

    // 添加组件
    protected void AddComponent(ActorComponent component)
    {
        if (component == null) {
            return;
        }
        
        _components[component.GetType().FullName] = component;
        component.OnInit(this);
    }

    protected void RemoveComponent(string componentName)
    {
        if (_components.ContainsKey(componentName)) {
            _components[componentName].OnDestory();
            _components.Remove(componentName);
        }
    }

    // 设置hud显示组件
    protected void SetHUDComponent(HUDComponent hud)
    {
        _hud = hud;
        if (_hud == null) {
            RemoveComponent(typeof(HUDComponent).FullName);
        } else {
            AddComponent(_hud);
        }
    }

    // 设置avatar显示组件
    protected void SetAvatarComponent(AvatarComponent avatar)
    {
        _avatar = avatar;
        if (_avatar == null) {
            RemoveComponent(typeof(AvatarComponent).FullName);
        } else {
            AddComponent(_avatar);
        }
    }

    // 设置动画组件
    protected void SetAnimationComponent(AnimationComponent anim)
    {
        _animation = anim;
        if (_animation == null) {

            RemoveComponent(typeof(AnimationComponent).FullName);
        } else {
            AddComponent(_animation);
        }
    }

    // 初始化状态
    protected virtual void InitState()
    {
        _fsm.AddState(StateID.STATE_PREVIEW, new ActorStatePreview());
        _fsm.AddState(StateID.STATE_DEPLOY, new ActorStateDeploy());
        _fsm.AddState(StateID.STATE_IDLE, new ActorStateIdle());
        _fsm.AddState(StateID.STATE_ATTACK, new ActorStateAttack());
        _fsm.AddState(StateID.STATE_PROJECTILE_ATTACK, new ActorStateProjectileAttack());
        _fsm.AddState(StateID.STATE_DIE, new ActorStateDie());
        _fsm.AddState(StateID.STATE_KNOCKBACK, new ActorStateKnockBack());
        _fsm.AddState(StateID.STATE_MOVE_TO_POSITION, new ActorStateMoveToPosition());
        _fsm.AddState(StateID.STATE_MOVE_TO_TARGET, new ActorStateMoveToTarget());
        _fsm.AddState(StateID.STATE_REVIVE, new ActorStateRevive());
        _fsm.AddState(StateID.STATE_STUN, new ActorStateStun());
    }
}
