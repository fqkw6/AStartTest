using ETouch;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfigMapOcclusion : MonoBehaviour
{
    public Dropdown _weightDropdown;
    public MapGridEditor _mapGrid;
    private Camera _camera; // 场景摄像机


    void OnEnable()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
    }

    void Start()
    {
        _camera = Camera.main;
    }

    private void OnTouchStart(Gesture gesture)
    {
        Vector3 pos = ScreenToWorld(gesture.position);
        Vector3 cell = _mapGrid.World2Cell(pos);

        // 标记障碍物遮挡
        _mapGrid.MarkOcclusion(cell);
    }

    public Vector3 ScreenToWorld(Vector3 point)
    {
        if (_camera == null) return Vector3.zero;
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f, GameLayer.GroundMask))
        {
            Vector3 pos = hit.point;
            return pos;
        }
        return Vector3.zero;
    }

    public void SaveMap()
    {
        _mapGrid.SaveMap();
    }

    // 设置该格子权重
    public void SetMarkCellWeight()
    {
        _mapGrid.SetCurrentMarkCellWeight(_weightDropdown.value);
    }
}
