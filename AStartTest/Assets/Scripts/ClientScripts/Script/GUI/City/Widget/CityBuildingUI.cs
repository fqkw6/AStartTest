using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

// 建筑物的ui
public class CityBuildingUI : MonoBehaviour
{
    public Text _levelText;
    public int _buildingCfgID;
    public Vector3 _offset;

    [NonSerialized] public CityBuilding Building;
    [NonSerialized] public RectTransform Parent;
    [NonSerialized] public long EntityID;
    [NonSerialized] public BuildingInfoPanel _buildingInfoPanelPrefab;
    [NonSerialized] public ProduceInfoPanel _produceInfoPanelPrefab;
    [NonSerialized] public TroopInfoPanel _troopInfoPanelPrefab;
    [NonSerialized] public GameObject _unlockPrefab;

    private BuildingInfo _currentInfo;
    private BuildingInfoPanel _buildingInfoPanel;
    private ProduceInfoPanel _produceInfoPanel;
    private TroopInfoPanel _troopInfoPanel;
    private GameObject _unlock;

    private void Awake()
    {
        IgnoreClick(_levelText.gameObject);
        InvokeRepeating("UpdatePanel", 0, 1);
    }

    private void OnEnable()
    {
        EventDispatcher.AddEventListener<long, bool>(EventID.EVENT_CITY_BUILDING_SHOW_PANEL, OnShowPanel);
        EventDispatcher.AddEventListener<long, bool>(EventID.EVENT_CITY_BUILDING_SHOW_PRODUCE_PANEL, OnShowProducePanel);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveEventListener<long, bool>(EventID.EVENT_CITY_BUILDING_SHOW_PANEL, OnShowPanel);
        EventDispatcher.RemoveEventListener<long, bool>(EventID.EVENT_CITY_BUILDING_SHOW_PRODUCE_PANEL, OnShowProducePanel);
    }

    void LateUpdate()
    {
        if (Building == null || Parent == null || Camera.main == null) return;

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null) return;

        Vector2 uiPos;
        Canvas canvas = UIManager.Instance.Canvas;
        Vector3 buildingPos = Camera.main.WorldToScreenPoint(Building.transform.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, buildingPos, canvas.worldCamera, out uiPos)) {
            rectTransform.anchoredPosition = uiPos;
        }
    }

    private void OnShowPanel(long entityID, bool show)
    {
        if (EntityID != entityID) return;
        if (_buildingInfoPanel) _buildingInfoPanel.Show(show);
    }

    private void OnShowProducePanel(long entityID, bool show)
    {
        if (EntityID != entityID) return;
        if (show && _currentInfo.IsInBuilding()) return;
        if (_produceInfoPanel) {
            if (_produceInfoPanel.gameObject.activeInHierarchy && !show) {
                _currentInfo.LastClickTime.SetTime(0);
            }

            _produceInfoPanel.Show(show);
        }
    }

    public void UpdatePanel()
    {
        BuildingInfo info = CityManager.Instance.GetBuildingByConfigID(_buildingCfgID);
        ProduceBuildingInfo pbinfo = info as ProduceBuildingInfo;
        if (pbinfo == null) return;

        if (_produceInfoPanel != null) {
            if (!_produceInfoPanel.gameObject.activeInHierarchy && !info.IsInBuilding() && pbinfo.GetCurrentProduceValue() > 0 && (!_currentInfo.LastClickTime.IsValid() || pbinfo.LastClickTime.GetTime() >= GameConfig.PRODUCE_REWARD_INTERVAL)) {
                _produceInfoPanel.Show(true);
            }

            _produceInfoPanel.UpdateIconColor();
        }
    }

    public void Refresh()
    {
        BuildingInfo info = CityManager.Instance.GetBuildingByConfigID(_buildingCfgID);
        _currentInfo = info;

        if (_currentInfo == null) {
            // 尚未解锁
            _levelText.transform.parent.gameObject.SetActive(false);
            if (_unlock == null) {
                _unlock = Instantiate(_unlockPrefab);
                _unlock.transform.SetParent(transform, false);
                _unlock.transform.localPosition = Vector3.zero;
            }
        } else {
            if (_unlock != null) {
                Destroy(_unlock);
                _unlock = null;
            }

            EntityID = _currentInfo.EntityID;
            _levelText.transform.parent.gameObject.SetActive(true);
            _levelText.text = _currentInfo.Cfg.BuildingName + "Lv " + _currentInfo.Level;
        }

        if (_buildingInfoPanel == null) {
            _buildingInfoPanel = Instantiate(_buildingInfoPanelPrefab);
            _buildingInfoPanel.transform.SetParent(transform, false);
            _buildingInfoPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            IgnoreClick(_buildingInfoPanel.gameObject);
        }

        if (_buildingInfoPanel != null) {
            _buildingInfoPanel.SetInfo(_currentInfo);
        }

        // 可收获的建筑有收获按钮
        if (_currentInfo is ProduceBuildingInfo) {
            if (_produceInfoPanel == null) {
                _produceInfoPanel = Instantiate(_produceInfoPanelPrefab);
                _produceInfoPanel.transform.SetParent(transform, false);
                _produceInfoPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            if (_produceInfoPanel != null) {
                _produceInfoPanel.SetInfo(_currentInfo);
            }
        }

        // 兵营
        if (_currentInfo is TroopBuildingInfo) {
            if (_troopInfoPanel == null) {
                _troopInfoPanel = Instantiate(_troopInfoPanelPrefab);
                _troopInfoPanel.transform.SetParent(transform, false);
                _troopInfoPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            if (_troopInfoPanel != null) {
                _troopInfoPanel.SetInfo(_currentInfo);
            }
        }
    }

    private void IgnoreClick(GameObject go)
    {
        // 防止点击事件拦截
        CanvasGroup group = go.AddComponent<CanvasGroup>();
        if (group != null) {
            group.blocksRaycasts = false;
            group.interactable = false;
        }
    }
}