using UnityEngine;
using System.Collections;

// 各种事件回调
public partial class Actor
{
    // 模型设置后调用
    public virtual void OnModelChanged()
    {
        foreach (var item in _components)
        {
            item.Value.OnModelChanged();
        }
    }

    // 角色出生
    public virtual void OnSpawn()
    {
        DeployFinished = true;
        CallFunction("OnSpawn");
    }

    // 受到攻击（不一定掉血）
    public virtual void OnHit(int damage)
    {
        //damage = 0;
        if (damage <= 0)
        {
            return;
        }

        if (CurrentShield > 0)
        {
            // 有护盾，先扣护盾
            CurrentShield -= damage;

            if (CurrentShield <= 0)
            {
                OnShieldBreak();
            }
        }
        else
        {
            // 扣血
            CurrentHP -= damage;

            // 死亡
            if (CurrentHP <= 0)
            {
                OnDeath();
            }
        }


        // 显示血条
        if (_hud != null)
        {
            ShowHealthBar();

            if (damage > 0)
            {
                // TODO 将来护盾伤害特殊表示
                _hud.SetHealth(CurrentHP);
                _hud.FloatingBlood(damage);
            }
        }
    }

    // 受到治疗
    public virtual void OnHeal(int value)
    {
    }

    // 受到伤害
    public virtual void OnTakeDamage(int damage)
    {
    }

    // 死亡时调用
    public virtual void OnDeath()
    {
        // 死亡标记
        IsDead = true;
        NeedToRemove = true;

        // 死亡的单位不受碰撞
        SetCollisionEnable(false);

        // 隐藏血条
        if (_hud != null)
        {
            _hud.HideHealthBar();
        }

        if (OnActorDeath != null)
        {
            OnActorDeath(this);
        }

        // 播放死亡特效应该在销毁物体之后
        //CallFunction("OnDeath");

        // 死亡事件处理
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        // 播放动画

        // 延时
        yield return new WaitForSeconds(1f);

        // 删除对象
        if (!IsBuilding())
            CreateParticle("Eff_SiWang", AttachType.WORLDPOINT, Position, 2, 2);
        CallFunction("OnDeath");
        Destroy(gameObject);
    }

    // 当护盾破碎
    public virtual void OnShieldBreak()
    {
        CallFunction("OnShieldBreak");
    }

}
