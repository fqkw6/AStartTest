using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using ETouch;

public partial class EventID
{
    public const string EVENT_HERO_SELECT_DETAIL = "EVENT_HERO_SELECT_DETAIL";
    public const string EVENT_HERO_SELECT_EQUIP = "EVENT_HERO_SELECT_EQUIP";
    public const string EVENT_HERO_SELECT_STAR = "EVENT_HERO_SELECT_STAR";
    public const string EVENT_HERO_SELECT_EXP = "EVENT_HERO_SELECT_EXP";
    public const string EVENT_HERO_SELECT_SKILL = "EVENT_HERO_SELECT_SKILL";

}

//英雄管理界面
public class UINewHeroView : UIWindow
{
    public const string Name = "Hero/UINewHeroView";
    public Text _txtHeroName;
    public UIStarPanel _starPanel;
    public Image _imageHeroHeadIcon;
    public Image _imageHeroHeadIconBg;
    public Image _imageHeroAttribute;
    public Text _txtHeroLevel;
    public Text _txtHeroNameQuality;
    public Text _txtHeroForce;

    public PanelHeroDetail _heroDetail;
    public PanelHeroAdvance _heroAdvance;
    public PanelHeroStarUp _heroStarUp;
    public PanelHeroExpBook _heroExpBook;
    public PanelHeroSkill _heroSkill;

    public GameObject _objDetail;

    public Button _leftButton;
    public Button _rightButton;

    public Sprite[] _sprType;

    private GameObject _goObject;
    private GameObject _goBgEffect;
    private GameObject _modelFloor;
    private HeroInfo _currentInfo;
    private int _currentHeroIndex = 0;
    private Camera _modelCamera = null;
    private GameObject _curModel = null;
    private string _curModelName;
    private float _currentRotate = 180;
    private GameObject _currentPanel;

    public override void OnOpenWindow()
    { 
        GameObject go = GameObject.Find("ModelCamera");
        if (go != null) {
            _modelCamera = go.GetComponent<Camera>();
        }

        EasyTouch.On_Swipe += OnRotate;
        EventDispatcher.AddEventListener(EventID.EVENT_UI_REFRSEH_HERO_ATTR,RefreshHeroAttr);

        _heroDetail.gameObject.SetActive(true);
        _heroAdvance.gameObject.SetActive(false);
        _heroStarUp.gameObject.SetActive(false);
        _heroExpBook.gameObject.SetActive(false);
        _heroSkill.gameObject.SetActive(false);

        EventDispatcher.AddEventListener(EventID.EVENT_HERO_SELECT_DETAIL, OnSelectDetail);
        EventDispatcher.AddEventListener(EventID.EVENT_HERO_SELECT_EQUIP, OnSelectEquip);
        EventDispatcher.AddEventListener(EventID.EVENT_HERO_SELECT_EXP, OnSelectExp);
        EventDispatcher.AddEventListener(EventID.EVENT_HERO_SELECT_STAR, OnSelectStar);
        EventDispatcher.AddEventListener(EventID.EVENT_HERO_SELECT_SKILL, OnSelectSkill);

        UIManager.Instance.OpenWindow<UIHeroMenuView>();

        _leftButton.GetComponent<Image>().DOFade(0, 1).SetLoops(9999, LoopType.Yoyo);
        _rightButton.GetComponent<Image>().DOFade(0, 1).SetLoops(9999, LoopType.Yoyo);
    }

    public override void OnCloseWindow()
    {
        if (_goObject != null) {
            Destroy(_goObject);
        }

        if (_goBgEffect != null) {
            Destroy(_goBgEffect);
        }

        EasyTouch.On_Swipe -= OnRotate;
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_REFRSEH_HERO_ATTR, RefreshHeroAttr);

        EventDispatcher.RemoveEventListener(EventID.EVENT_HERO_SELECT_DETAIL, OnSelectDetail);
        EventDispatcher.RemoveEventListener(EventID.EVENT_HERO_SELECT_EQUIP, OnSelectEquip);
        EventDispatcher.RemoveEventListener(EventID.EVENT_HERO_SELECT_EXP, OnSelectExp);
        EventDispatcher.RemoveEventListener(EventID.EVENT_HERO_SELECT_STAR, OnSelectStar);
        EventDispatcher.RemoveEventListener(EventID.EVENT_HERO_SELECT_SKILL, OnSelectSkill);

        UIManager.Instance.CloseWindow<UIHeroMenuView>();
    }

    public override void OnBindData(params object[] param)
    {
        if (param.Length > 0) {
            SetHeroInfo(param[0] as HeroInfo);
        } else {
            if (UserManager.Instance.HeroList.Count > 0) {
                SetHeroInfo(UserManager.Instance.HeroList[0]);
            }
        }
    }

    public override void OnRefreshWindow()
    {
        SetHeroInfo(_currentInfo, false);
    }

    public virtual void OnRotate(ETouch.Gesture gesture)
    {
        if (_curModel == null || _modelCamera == null) {
            return;
        }

        GameObject go = EventSystem.current.currentSelectedGameObject;
        if (go != gameObject) {
            return;
        }

        float offset = gesture.deltaPosition.x;
        _currentRotate -= offset;

        Ray ray = _modelCamera.ScreenPointToRay(gesture.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            _goObject.transform.Rotate(Vector3.up, -offset);
            //_goObject.transform.localRotation = Quaternion.Euler(new Vector3(-15, _currentRotate, 0));
        }
    }

    private void RefreshHeroAttr()
    {
        _txtHeroLevel.text = "LV." + _currentInfo.Level.ToString();
    }

    public void SetHeroInfo(HeroInfo info, bool fullRefresh = true)
    {
        if (info == null) return;

        SetCurrentHeroIndex(info);
        _currentInfo = info;

        _txtHeroNameQuality.text = info.GetName();
        _txtHeroNameQuality.color = ResourceManager.Instance.GetColorByQuality(_currentInfo.StarLevel);
        _txtHeroName.text = info.Cfg.HeroName;
        _txtHeroForce.text = info.FightingScore.ToString();
        _imageHeroAttribute.sprite = _sprType[info.Cfg.AttackType - 1];
        _imageHeroHeadIcon.sprite = ResourceManager.Instance.GetHeroIcon(info.ConfigID);
        _imageHeroHeadIconBg.sprite = ResourceManager.Instance.GetHeroIconBgByStar(info.StarLevel);
        RefreshHeroAttr();

        _starPanel.SetStar(_currentInfo.StarLevel);
        
        if (_heroDetail != null) {
            _heroDetail.SetHeroInfo(_currentInfo);
        }
        if (_heroAdvance != null) {
            _heroAdvance.SetHeroInfo(_currentInfo);
        }
        if (_heroStarUp != null) {
            _heroStarUp.SetHeroInfo(_currentInfo);
        }
        if (_heroExpBook != null) {
            _heroExpBook.SetInfo(_currentInfo);
        }
        if (_heroSkill != null) {
            _heroSkill.SetHeroInfo(_currentInfo);
        }

        ShowModel(_currentInfo.Cfg.HeroModel);

        UpdateButton();
    }

    private void ShowModel(string modelName)
    {
        if (_curModelName == modelName) {
            return;
        }

        _curModelName = modelName;

        if (_curModel != null) {
            Destroy(_curModel);
        }

        if (_goObject == null) {
            _goObject = new GameObject();
            _goObject.transform.localPosition = new Vector3(-6, -6.8f, 20);
            _goObject.transform.localScale = Vector3.one;
            _goObject.transform.Rotate(-12, 0, 0);

        }

        if (_goBgEffect == null) {
            GameObject effPrefab = Resources.Load<GameObject>("Model/eff");
            _goBgEffect = Instantiate(effPrefab);
            _goBgEffect.transform.localPosition = new Vector3(-6, -5.2f, 20);
        }

        if (_modelFloor == null) {
            GameObject floorPrefab = Resources.Load<GameObject>("Model/ditai");
            _modelFloor = Instantiate(floorPrefab);
            _modelFloor.transform.SetParent(_goObject.transform);
            //_modelFloor.transform.localScale = Vector3.one * 13;
            _modelFloor.transform.localPosition = new Vector3(0, 0, 0);
            _modelFloor.transform.localRotation = Quaternion.Euler(new Vector3(270, 180, 180));
            Utils.SetLayer(_modelFloor.transform, UIManager.UIModel);
        }

        string path = "Model/Hero/" + modelName;
        GameObject prefab = Resources.Load<GameObject>(path);
        _curModel = Instantiate(prefab);
        _curModel.transform.SetParent(_goObject.transform);
        _curModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //_curModel.transform.localScale = Vector3.one*4;
        _curModel.transform.localPosition = new Vector3(0, 1.3f, 0);
        Utils.SetLayer(_curModel.transform, UIManager.UIModel);
        
        BoxCollider col = _curModel.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 1.2f, 0);
        col.size = new Vector3(2.5f, 3.5f, 1);

        _currentRotate = 0;

        if (_curModel != null) {
            Animation anim = _curModel.GetComponent<Animation>();
            if (anim != null) {
                anim.Play("idle_re");
            }
        }
    }

    private void OnSelectDetail()
    {
        _currentPanel.gameObject.SetActive(false);
        _objDetail.gameObject.SetActive(true);
    }

    private void OnSelectExp()
    {
        if (_currentPanel != null) {
            _currentPanel.gameObject.SetActive(false);
        }

        _heroExpBook.gameObject.SetActive(true);
        _objDetail.gameObject.SetActive(false);
        _currentPanel = _heroExpBook.gameObject;
        _heroExpBook.SetInfo(_currentInfo);
    }

    private void OnSelectStar()
    {
        if (_currentPanel != null) {
            _currentPanel.gameObject.SetActive(false);
        }

        _heroStarUp.gameObject.SetActive(true);
        _objDetail.gameObject.SetActive(false);
        _currentPanel = _heroStarUp.gameObject;
    }

    private void OnSelectEquip()
    {
        if (_currentPanel != null) {
            _currentPanel.gameObject.SetActive(false);
        }

        _heroAdvance.gameObject.SetActive(true);
        _objDetail.gameObject.SetActive(false);
        _currentPanel = _heroAdvance.gameObject;
    }

    private void OnSelectSkill()
    {
        if (_currentPanel != null) {
            _currentPanel.gameObject.SetActive(false);
        }

        _heroSkill.gameObject.SetActive(true);
        _objDetail.gameObject.SetActive(false);
        _currentPanel = _heroSkill.gameObject;
        _heroSkill.SetHeroInfo(_currentInfo);
    }

    // 设置当前选择了哪个英雄
    public void SetCurrentHeroIndex(HeroInfo info)
    {
        List<HeroInfo> heroList = UserManager.Instance.HeroList;
        for (int i = 0; i < heroList.Count; ++i) {
            if (heroList[i].ConfigID == info.ConfigID) {
                _currentHeroIndex = i;
                break;
            }
        }
    }

    // 获取上一个英雄信息
    public HeroInfo GetPrevHero()
    {
        List<HeroInfo> heroList = UserManager.Instance.HeroList;
        _currentHeroIndex = Mathf.Max(_currentHeroIndex - 1, 0);
        if (_currentHeroIndex < heroList.Count) {
            return heroList[_currentHeroIndex];
        }
        return null;
    }

    // 获取下一个英雄信息
    public HeroInfo GetNextHero()
    {
        List<HeroInfo> heroList = UserManager.Instance.HeroList;
        _currentHeroIndex = Mathf.Min(_currentHeroIndex + 1, heroList.Count - 1);
        if (_currentHeroIndex < heroList.Count) {
            return heroList[_currentHeroIndex];
        }
        return null;
    }

    // 切换上一个英雄
    public void OnClickLeft()
    {
        if (_currentHeroIndex <= 0) {
            return;
        }

        HeroInfo info = GetPrevHero();
        if (info == null) return;

        SetHeroInfo(info);
    }

    // 切换下一个英雄
    public void OnClickRight()
    {
        if (_currentHeroIndex >= UserManager.Instance.HeroList.Count - 1) {
            return;
        }

        HeroInfo info = GetNextHero();
        if (info == null) return;

        SetHeroInfo(info);
    }


    public void UpdateButton()
    {
        _leftButton.gameObject.SetActive(_currentHeroIndex > 0);
        _rightButton.gameObject.SetActive(_currentHeroIndex < UserManager.Instance.HeroList.Count - 1);
    }
}
