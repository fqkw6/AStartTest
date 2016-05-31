using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using ETouch;

// 触摸摄像机
public class RtsCameraTouch : MonoBehaviour {

    public float _moveSpeed = 25;
    public float _zoomSpeed = 10;

    private RtsCamera _rtsCamera;

    void Awake()
    {
        _rtsCamera = GetComponent<RtsCamera>();
    }
    
    void Start () {
	
	}

    void OnEnable()
    {
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_Pinch += On_Pinch;
    }

    void OnDisable()
    {
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_Pinch -= On_Pinch;
    }

    public bool IsTouchUI()
    {
        GameObject go = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        if (go != null) {
            return true;
        }

        return false;
    }

    void On_Swipe(ETouch.Gesture gesture)
    {
        if (IsTouchUI()) {
            return;
        }

        _rtsCamera.AddToPosition(-gesture.deltaPosition.x * _moveSpeed * Time.deltaTime, 0, -gesture.deltaPosition.y * _moveSpeed * Time.deltaTime);
    }

    void On_Pinch(ETouch.Gesture gesture)
    {
        if (IsTouchUI()) {
            return;
        }

        _rtsCamera.Distance -= gesture.deltaPinch * _zoomSpeed * Time.deltaTime;
    }
}
