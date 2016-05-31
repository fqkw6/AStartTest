using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum TavernBuyAction
{
    BUY_MONEY_1,
    BUY_MONEY_10,
    BUY_GOLD_1,
    BUY_GOLD_10,
}

// 酒馆购买成功获得物品
public class UITavernGetItemView : UIWindow
{
    public const string Name = "Tavern/UITavernGetItemView";
    public Text _txtTitle;

    public Transform _startPosition;
    public ItemWidget[] _listWidget;

    private List<ItemInfo> _itemList;
    private int _itemID;
    private int _count;
    private TavernBuyAction _buyAction;
    private List<Vector3> _originPosition = new List<Vector3>();
    private GameObject _goEffect = null;    // 抽到英雄的动画

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        _itemList = param[0] as List<ItemInfo>;
        _itemID = (int)param[1];
        _count = (int)param[2];
        _buyAction = (TavernBuyAction)param[3];
    }

    public override void OnCloseWindow()
    {
        if (_goEffect != null)
        {
            Destroy(_goEffect);
        }
    }

    public override void OnRefreshWindow()
    {
        if (_itemID > 0)
        {
            ItemsConfig cfg = ItemsConfigLoader.GetConfig(_itemID);
            _txtTitle.text = string.Format(Str.Get("UI_TAVERN_BUY_EXP"), (_count > 1 ? cfg.Name + "x" + _count : cfg.Name));
        }

        for (int i = 0; i < _listWidget.Length; ++i)
        {
            ItemWidget widget = _listWidget[i];
            if (i < _itemList.Count)
            {
                widget.gameObject.SetActive(true);
                ItemInfo itemInfo = _itemList[i];
                widget.SetInfo(itemInfo);
                _originPosition.Add(widget.transform.position);
                widget.gameObject.SetActive(false);
            }
            else
            {
                widget.gameObject.SetActive(false);
            }
        }

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        const float TIME = 0.2f;
        for (int i = 0; i < Mathf.Min(_itemList.Count, _listWidget.Length); ++i)
        {
            ItemWidget widget = _listWidget[i];
            widget.gameObject.SetActive(true);
            widget.transform.position = _startPosition.transform.position;
            widget.transform.localScale = Vector3.one * 0.1f;
            widget.transform.Rotate(0, 0, -270);

            Sequence seq = DOTween.Sequence();
            seq.Join(widget.transform.DOMove(_originPosition[i], TIME));
            seq.Join(widget.transform.DOScale(Vector3.one, TIME));
            seq.Join(widget.transform.DORotate(Vector3.zero, TIME));
            seq.Play();

            if (widget._info.IsCard())
            {
                ItemsConfig itemCfg = ItemsConfigLoader.GetConfig(widget._info.ConfigID);
                HeroConfig heroCfg = HeroConfigLoader.GetConfig(itemCfg.MatchHero);
                // 创建模型
                GameObject prefab = Resources.Load<GameObject>("Effect/UI/Eff_chouka");
                GameObject go = Instantiate(prefab);
                go.transform.position = Vector3.zero;
                // 等待动画播放完出现底座才显示模型
                yield return new WaitForSeconds(2);
                CreateHeroModel(heroCfg, go);
                yield return new WaitForSeconds(3);
                Destroy(go);
            }
            yield return new WaitForSeconds(TIME);
        }
    }


    private GameObject CreateHeroModel(HeroConfig heroCfg, GameObject parentTransform)
    {
        string heroModelName = heroCfg.HeroModel;
        string modelPath = "Model/Hero/" + heroModelName;
        GameObject modelPrefab = Resources.Load<GameObject>(modelPath);
        GameObject _curModel = Instantiate(modelPrefab);
        _curModel.transform.SetParent(parentTransform.transform);
        _curModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        _curModel.transform.localScale = new Vector3(3, 3, 3);
        _curModel.transform.localPosition = new Vector3(0, -3.25f, 0);
        Utils.SetLayer(_curModel.transform, UIManager.UIModel);

        BoxCollider col = _curModel.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 1.2f, 0);
        col.size = new Vector3(2.5f, 3.5f, 1);

        if (_curModel != null)
        {
            Animation anim = _curModel.GetComponent<Animation>();
            if (anim != null)
            {
                anim.Play("idle_re");
            }
        }
        return _curModel;
    }

    // 再次购买
    public void OnClickBuyMore()
    {
        switch (_buyAction)
        {
            case TavernBuyAction.BUY_MONEY_1:
                ShopManager.Instance.RequestTavernMoneyBuy1();
                break;
            case TavernBuyAction.BUY_MONEY_10:
                ShopManager.Instance.RequestTavernMoneyBuy10();
                break;
            case TavernBuyAction.BUY_GOLD_1:
                ShopManager.Instance.RequestTavernGoldBuy1();
                break;
            case TavernBuyAction.BUY_GOLD_10:
                ShopManager.Instance.RequestTavernGoldBuy10();
                break;
        }

        CloseWindow();
    }

    private void OnHeroSuc(int heroCfgID)
    {
        GameObject prefab = Resources.Load<GameObject>("Effect/UI/Eff_chouka");
        _goEffect = Instantiate(prefab);
        _goEffect.transform.position = Vector3.zero;
    }

}
