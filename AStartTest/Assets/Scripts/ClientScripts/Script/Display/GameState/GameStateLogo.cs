using UnityEngine;
using System.Collections;

public class GameStateLogo : FSMStateBase
{
    public override void OnEnter(params object[] param)
    {
        Game.Instance.ShowMainCity(false);
    }
}
