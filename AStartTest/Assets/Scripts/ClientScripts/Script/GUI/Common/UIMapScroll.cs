using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;
using UnityEngine.EventSystems;
using ETouch;

// 地图的拖动组件
public class UIMapScroll : MonoBehaviour
{
    public RectTransform _mapBg;
    public bool _isWorldMap = false;
    
#if DEBUG_SCALE
    private GUIText _guiText;
#endif


    private void Start()
    {
#if DEBUG_SCALE
        _guiText = GetComponent<GUIText>();
#endif

        // 初始化当前的缩放
        if (!_isWorldMap) {
            _mapBg.transform.localScale = Vector3.one * Game.Instance.CityScaleFactor;
        } else {
            _mapBg.transform.localScale = Vector3.one * Game.Instance.WorldScaleFactor;
        }
    }

    private void OnEnable()
    {
        EasyTouch.On_Swipe += On_Drag;
        EasyTouch.On_Pinch += On_Pinch;
    }

    private void OnDisable()
    {
        EasyTouch.On_Swipe -= On_Drag;
        EasyTouch.On_Pinch -= On_Pinch;
    }

    private bool InChild(Transform t)
    {
        if (t == transform || t == null) {
            return true;
        }

        Transform parent = t.parent;
        while (parent != null) {
            if (parent == transform) {
                return true;
            }
            parent = parent.parent;
        }

        return false;
    }

    private void ScaleMap(float scale = 0)
    {
        if (scale > 0) {
            _mapBg.transform.localScale = Vector3.one*scale;
            if (!_isWorldMap) {
                Game.Instance.CityScaleFactor = scale;
            } else {
                Game.Instance.WorldScaleFactor = scale;
            }
        } else {
            if (!_isWorldMap) {
                _mapBg.transform.localScale = Vector3.one * Game.Instance.CityScaleFactor;
            } else {
                _mapBg.transform.localScale = Vector3.one * Game.Instance.WorldScaleFactor;
            }
        }
    }

    private void On_Drag(ETouch.Gesture gesture)
    {
        // 调整地图位置
        if (_mapBg == null) return;

        GameObject go = EventSystem.current.currentSelectedGameObject;
        //Debug.Log(go);

        if (go != null) {
            if (!InChild(go.transform)) {
                return;
            }
        }

        Vector3 pos = _mapBg.transform.localPosition;
        pos.x += gesture.deltaPosition.x;
        pos.y += gesture.deltaPosition.y;
        _mapBg.transform.localPosition = pos;
        CheckBound();
    }

    private void On_Pinch(ETouch.Gesture gesture)
    {
        // 进行缩放
        if (!_isWorldMap) {
            Game.Instance.CityScaleFactor += gesture.deltaPinch / 1000;
            _mapBg.transform.localScale = Vector3.one * Game.Instance.CityScaleFactor;
        } else {
            Game.Instance.WorldScaleFactor += gesture.deltaPinch / 1000;
            _mapBg.transform.localScale = Vector3.one * Game.Instance.WorldScaleFactor;
        }

        CheckBound();
    }

    private void CheckBound()
    {
        Vector3 pos = _mapBg.transform.localPosition;
        float scale = _mapBg.transform.localScale.x;
        float xSize = _mapBg.sizeDelta.x;
        float ySize = _mapBg.sizeDelta.y;

        float minX = -(xSize * scale - GameConfig.SCREEN_WIDTH) / 2;
        float maxX = (xSize * scale - GameConfig.SCREEN_WIDTH) / 2;
        float minY = -(ySize * scale - GameConfig.SCREEN_HEIGHT) / 2;
        float maxY = (ySize * scale - GameConfig.SCREEN_HEIGHT) / 2;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _mapBg.transform.localPosition = pos;
    }
#if DEBUG_SCALE
    private void OnGUI()
    {
        if (_guiText != null) {
            _guiText.text = string.Format("scale:{0}    zoom:{1}", Game.Instance.ScaleFactor, Game.Instance.ScaleFactor);
            GUI.Label(new Rect(100, 0, 300, 30), _guiText.text);
        }
    }
#endif
}
