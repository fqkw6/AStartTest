using UnityEngine;
using System.Collections;

public partial class EventID
{
    public const string ACTOR_PROPERTY_CHANGE = "ACTOR_PROPERTY_CHANGE";  // 玩家属性改变
}

public class ActorStateBase : FSMStateBase
{
    private Actor _owner;

    protected Actor Owner
    {
        get
        {
            if (_owner == null) {
                _owner = _fsm.Owner as Actor;
            }

            return _owner;
        }
    }

    // 当玩家数据改变的时候
    //public virtual void OnActorPropertyChange()
    //{

    //}
}
