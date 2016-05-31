using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ETouch;
using Pathfinding;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
    public Toggle _toggleErase;
    public Toggle _toggleGrass;
    public Toggle _toggleRiver;
    public MapGrid _mapGrid;
    public Camera _camera;
    public Transform _map;

    //public Dropdown _weightDropdown;

    private Vector2 _startCell;
    private Vector2 _endCell;

    // Use this for initialization
    void Start()
    {
        _toggleErase.isOn = true;
        _toggleGrass.isOn = false;
        _toggleRiver.isOn = false;
    }

    void OnEnable()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
        EasyTouch.On_TouchUp += OnTouchUp;
        EasyTouch.On_Swipe += OnSwipe;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        EasyTouch.On_TouchUp -= OnTouchUp;
        EasyTouch.On_Swipe -= OnSwipe;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTouchStart(Gesture gesture)
    {
        _startCell = ScreenToCell(gesture.position);
        _endCell = Vector3.zero;
    }

    private void OnSwipe(Gesture gesture)
    {
        _endCell = ScreenToCell(gesture.position);

    }

    private void DrawBox()
    {

    }

    void OnDrawGizmos()
    {
        if (_toggleErase.isOn)
        {
            return;
        }

        Vector3 start = _mapGrid.CellToWorld(_startCell);
        Vector3 end = _mapGrid.CellToWorld(_endCell);

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(start, 1);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(end, 1);

        if (_startCell.sqrMagnitude <= 0 || _endCell.sqrMagnitude <= 0)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere((end + start) / 2, 1);

        Color color = Color.gray;
        color.a = 0.7f;
        Gizmos.color = color;
        Gizmos.DrawCube((end + start) / 2, new Vector3(Mathf.Abs(end.x - start.x) + _mapGrid.GetCellWidth(), 1, Math.Abs(end.z - start.z) + _mapGrid.GetCellHeight()));
    }

    // 屏幕坐标(触摸坐标)转换为世界坐标
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

    // 屏幕坐标转为格子坐标
    public Vector2 ScreenToCell(Vector2 position)
    {
        Vector3 pos = ScreenToWorld(position);
        return _mapGrid.WorldToCell(pos);
    }

    public void OnClickCreate()
    {
        if (_toggleRiver.isOn)
        {
            CreateRiver();
        }
        else if (_toggleGrass.isOn)
        {
            CreateGrass();
        }
    }

    private void OnTouchUp(Gesture gesture)
    {
        _endCell = ScreenToCell(gesture.position);
    }

    // 创建河流 不可走动的掩码
    private void CreateRiver()
    {
        Vector3 start = _mapGrid.CellToWorld(_startCell);
        Vector3 end = _mapGrid.CellToWorld(_endCell);

        GameObject go = new GameObject("River");
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x) + _mapGrid.GetCellWidth(), 4, Math.Abs(end.z - start.z) + _mapGrid.GetCellHeight());

        go.layer = LayerMask.NameToLayer("NotWalkable");
        go.AddComponent<BoxCollider>();
        go.transform.SetParent(_map);
        go.transform.position = (end + start) / 2;
        go.transform.localScale = size;
    }

    // 创建草地  可以走动，但是消耗较大，步行单位行动时会尽量避免草地
    private void CreateGrass()
    {
        Vector3 start = _mapGrid.CellToWorld(_startCell);
        Vector3 end = _mapGrid.CellToWorld(_endCell);

        GameObject go = new GameObject("Grass");
        Vector3 size = new Vector3(Mathf.Abs(end.x - start.x) + _mapGrid.GetCellWidth(), 1, Math.Abs(end.z - start.z) + _mapGrid.GetCellHeight());
        Vector3 center = (end + start) / 2;

        go.transform.SetParent(_map);
        go.transform.position = center;
        go.transform.localScale = Vector3.one;

        var graph = go.AddComponent<GraphUpdateScene>();
        graph.updateErosion = true;
        graph.convex = true;
        graph.applyOnStart = false;
        graph.applyOnScan = true;
        graph.modifyWalkability = true;
        graph.setWalkability = true;
        graph.penaltyDelta = 10000;
        graph.modifyTag = false;

        float halfX = size.x / 2;
        float halfY = size.z / 2;
        graph.points = new Vector3[4];
        graph.points[0] = new Vector3(-halfX, 0, -halfY);
        graph.points[1] = new Vector3(halfX, 0, -halfY);
        graph.points[2] = new Vector3(halfX, 0, halfY);
        graph.points[3] = new Vector3(-halfX, 0, halfY);
    }
}
