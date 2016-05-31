using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.IsolatedStorage;
using DG.Tweening;

public class BattleResultHeroWidget : MonoBehaviour
{
    public Image _heroBg;
    public Image _heroLevelUp;
    public UIProgress _heroExp;
    public Image _heroIcon;
    public Text _heroLevel;
    private HeroInfo _info;
    private int _addExp;

    public void SetInfo(long heroID, int addExp)
    {
        HeroInfo info = UserManager.Instance.GetHeroInfo(heroID);
        if (info == null) {
            gameObject.SetActive(false);
            return;
        }

        _info = info;
        _addExp = addExp;

        _heroBg.sprite = ResourceManager.Instance.GetIconBgByQuality(info.StarLevel);
        _heroIcon.sprite = ResourceManager.Instance.GetHeroIcon(info.ConfigID);
        _heroLevel.text = "Lv " + info.Level;


        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(info.Level);
        if (expCfg.ExpRequire == 0) {
            _heroExp.SetValue(1);
            if (_heroLevelUp != null) _heroLevelUp.gameObject.SetActive(false);
        } else {
            // 设置初始的进度
            _heroExp._image.fillAmount = 1.0f*info.Exp/expCfg.ExpRequire;
            StartCoroutine(ProcessAnimation());

            if (info.Exp + addExp >= expCfg.ExpRequire) {
                if (_heroLevelUp != null) _heroLevelUp.gameObject.SetActive(true);
            } else {
                if (_heroLevelUp != null) _heroLevel.gameObject.SetActive(false);
            }
        }

        _heroExp.SetText(Str.Get("UI_EXP") + "+" + addExp);
    }

    // 处理动画
    IEnumerator ProcessAnimation()
    {
        const float TIME = 0.5f;
        float curFillAmount = 0;
        int curLevel = _info.Level;
        int curExp = _info.Exp;
        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(curLevel);
        int maxExp = expCfg.ExpRequire;

        if (curExp + _addExp < maxExp) {
            // 未升级
            curFillAmount = 1.0f*(curExp + _addExp)/maxExp;
            _heroExp._image.DOFillAmount(curFillAmount, TIME);
        } else {
            while (_addExp > 0) {
                curFillAmount = Mathf.Max(1.0f, 1.0f * (curExp + _addExp) / maxExp);
                _heroExp._image.DOFillAmount(curFillAmount, TIME);
                yield return new WaitForSeconds(TIME);
                _heroExp._image.fillAmount = 0;
                _addExp -= maxExp - curExp;
                curExp = 0;
                ++curLevel;
                expCfg = HeroLevelConfigLoader.GetConfig(curLevel);
                maxExp = expCfg.ExpRequire;
            }
        }
    }
}
