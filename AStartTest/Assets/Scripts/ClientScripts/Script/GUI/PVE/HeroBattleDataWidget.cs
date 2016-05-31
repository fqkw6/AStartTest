using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 战斗数据中的widget
public class HeroBattleDataWidget : MonoBehaviour
{
    public UIProgress _prgDamage;
    public UIProgress _prgDamageGet;
    public UIProgress _prgKill;
    public UIProgress _prgTime;

    // Use this for initialization
    void Start () {
	
	}

    public void SetInfo(BattleDataHeroInfo info, int totalDamage, int totalDamageGet, int totalKill, float totalTime)
    {
        _prgDamage.SetValue(1.0f * info.damage / info.totalDamage);
        _prgDamageGet.SetValue(1.0f * info.damageGet / info.totalDamageGet);
        _prgKill.SetValue(1.0f * info.kill / info.totalKill);
        _prgTime.SetValue(info.time / info.totalTime);

        _prgDamage.SetText(info.damage.ToString());
        _prgDamageGet.SetText(info.damageGet.ToString());
        _prgKill.SetText(info.kill.ToString());
        _prgTime.SetText(Utils.GetCountDownString(info.time));
    }
}
