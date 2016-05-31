using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public partial class EventID
{
    public const string EVENT_EXPBOOK_COST = "EVENT_EXPBOOK_COST";  // 吃经验书
}

public class PanelHeroExpBook : MonoBehaviour
{
    public Text _txtHeroLevel;
    public Text _txtHeroLevelNext;
    public Image _imageExpPrg;
    public Text _txtExpPrg;
    public ExpItemWidget[] _itemWidget;

    private float _nextFillAmount;
    private Sequence _curSeq;

    void OnEnable()
    {
        EventDispatcher.AddEventListener<int, int>(EventID.EVENT_EXPBOOK_COST, SetExpProgress);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveEventListener<int, int>(EventID.EVENT_EXPBOOK_COST, SetExpProgress);
    }

    public void SetInfo(HeroInfo info)
    {
        if (info == null) return;

        _txtHeroLevel.text = info.Level.ToString();
        _txtHeroLevelNext.text = (info.Level + 1).ToString();
        _itemWidget[0].SetInfo(GameConfig.ITEM_CONFIG_ID_EXP_1, info);
        _itemWidget[1].SetInfo(GameConfig.ITEM_CONFIG_ID_EXP_2, info);
        _itemWidget[2].SetInfo(GameConfig.ITEM_CONFIG_ID_EXP_3, info);
        _itemWidget[3].SetInfo(GameConfig.ITEM_CONFIG_ID_EXP_4, info);
        _itemWidget[4].SetInfo(GameConfig.ITEM_CONFIG_ID_EXP_5, info);

        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(info.Level);
        if (expCfg != null)
        {
            if (expCfg.ExpRequire == 0)
            {
                _imageExpPrg.fillAmount = 1;
                _txtExpPrg.gameObject.SetActive(false);
            }
            else
            {
                if(_curSeq != null)
                _curSeq.Kill();
                Sequence seq = DOTween.Sequence();
                _curSeq = seq;
                seq.Join(_imageExpPrg.DOFillAmount(1.0f * info.Exp / expCfg.ExpRequire, 0.5f));
                seq.Play();
                _txtExpPrg.text = string.Format("{0}/{1}", info.Exp, expCfg.ExpRequire);
                _txtExpPrg.gameObject.SetActive(true);
            }
        }
    }

    // 设置当前经验进度
    public void SetExpProgress(int clientCurrentLevel, int clientCurrentExp)
    {
        _txtHeroLevel.text = clientCurrentLevel.ToString();
        _txtHeroLevelNext.text = (clientCurrentLevel + 1).ToString();
        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(clientCurrentLevel);
        if (expCfg != null)
        {
            if (expCfg.ExpRequire == 0)
            {
                _imageExpPrg.fillAmount = 1;
                _txtExpPrg.gameObject.SetActive(false);
            }
            else
            {
                _nextFillAmount = 1.0f * clientCurrentExp / expCfg.ExpRequire;
                _curSeq.Kill();
                Sequence seq = DOTween.Sequence();
                _curSeq = seq;
                seq.Join(_imageExpPrg.DOFillAmount(_nextFillAmount, 0.5f));
                seq.Play();
                _txtExpPrg.text = string.Format("{0}/{1}", clientCurrentExp, expCfg.ExpRequire);
                _txtExpPrg.gameObject.SetActive(true);
            }
        }
    }
}
