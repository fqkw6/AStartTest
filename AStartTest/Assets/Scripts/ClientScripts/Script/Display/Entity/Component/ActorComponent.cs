using UnityEngine;
using System.Collections;

//  actor组件基类 
// 独立性或者复用性较强的逻辑可以独立为一个组件
// 逻辑复杂、接口复杂的功能尽量不要作为组件，因为会增加代码的复杂度
public class ActorComponent : MonoBehaviour
{
    // 创建actor时初始化组件
    public virtual void OnInit(Actor actor)
    {
        
    }

    // 每帧更新时调用
    public virtual void OnUpdate(float dt)
    {
        
    }

    // 每逻辑帧调用
    public virtual void OnTick()
    {
        
    }

    // 销毁actor时调用
    public virtual void OnDestory()
    {
        
    }

    // 模型设置完毕后调用
    public virtual void OnModelChanged()
    {

    }
}
