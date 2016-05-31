using System;
using LitJson;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

// 地表坐标 格子 每个场景都要配置一个地表格子的GameObject
public class MapGrid : MonoBehaviour
{
    public int _width;
    public int _height;
    public bool _showDebugGrid;

    [NonSerialized]
    public static float CellSize = 0;   // 格子大小，宽高要一致，用于配置的格子大小到实际世界坐标距离的转换

    private Mesh _mesh;
    private Material _material;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Vector3 _originPosition; // 原点坐标
    private float _cellWidth;
    private float _cellHeight;
    private int _currentMarkCellWeight = 0;

    public class Occlusion
    {
        public Mesh _mesh;
        public int weight;

        public Occlusion(Mesh mesh, int weig)
        {
            _mesh = mesh;
            weight = weig;
        }
    }

    private void Awake()
    {
        if (_width == 0) {
            _width = 1;
        }

        if (_height == 0) {
            _height = 1;
        }
        _mesh = new Mesh();
        _material = new Material(Shader.Find("Shader/BlackLine"));
        _material.color = new Color(0, 1, 0, 0.5f);

        CreateGrid();
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public float GetCellWidth()
    {
        return _cellWidth;
    }

    public float GetCellHeight()
    {
        return _cellHeight;
    }

    public void SetCurrentMarkCellWeight(int weight)
    {
        _currentMarkCellWeight = weight;
    }

    // 世界坐标转为格子坐标
    public Vector2 WorldToCell(Vector3 pos)
    {
        Vector2 cell = new Vector3();
        cell.x = Mathf.Clamp(Mathf.FloorToInt((pos.x - _originPosition.x)/_cellWidth), 0, _width);
        cell.y = Mathf.Clamp(Mathf.FloorToInt((pos.z - _originPosition.z)/_cellHeight), 0, _height);
        return cell;
    }

    // 格子坐标转为世界坐标
    public Vector3 CellToWorld(Vector2 cell)
    {
        Vector3 pos = new Vector3();
        pos.x = _originPosition.x + cell.x*_cellWidth + _cellWidth/2;
        pos.y = 0;
        pos.z = _originPosition.z + cell.y*_cellHeight + _cellHeight/2;
        return pos;
    }

    private void CreateGrid()
    {
        // 通过scale决定地图的实际大小，因为要有一个平面接收触摸事件，所以MapGrid必须要进行缩放
        Vector3 scale = transform.localScale;
        // 原点坐标为左下角
        _originPosition = transform.position + new Vector3(-scale.x/2, 0.3f, -scale.z/2);

        // 网格的长宽
        float width = scale.x;
        float height = scale.z;

        // 计算每个格子长宽

        _cellWidth = width/_width;
        _cellHeight = height/_height;

        // 宽和高是一致的
        CellSize = _cellWidth;

        const float WIDTH = 0.5f;

        // 行 注意等于号，以便形成封闭的格子
        for (int i = 0; i <= _height; ++i) {
            Vector3 pos = _originPosition + new Vector3(0, 0, i*_cellHeight);
            AddLine(pos, pos + new Vector3(width, 0, 0), WIDTH);
        }

        // 列
        for (int i = 0; i <= _width; ++i) {
            Vector3 pos = _originPosition + new Vector3(i*_cellWidth, 0, 0);
            AddRow(pos, pos + new Vector3(0, 0, height), WIDTH);
        }

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
    }

    // 行
    private void AddLine(Vector3 start, Vector3 end, float width)
    {
        int index = _vertices.Count;
        _vertices.Add(start);
        _vertices.Add(end);
        _vertices.Add(end + new Vector3(0, 0, width));
        _vertices.Add(start + new Vector3(0, 0, width));

        _triangles.Add(index);
        _triangles.Add(index + 1);
        _triangles.Add(index + 3);
        _triangles.Add(index + 3);
        _triangles.Add(index + 1);
        _triangles.Add(index + 2);
    }

    // 列
    private void AddRow(Vector3 start, Vector3 end, float width)
    {
        int index = _vertices.Count;
        _vertices.Add(start);
        _vertices.Add(start + new Vector3(width, 0, 0));
        _vertices.Add(end + new Vector3(width, 0, 0));
        _vertices.Add(end);

        _triangles.Add(index);
        _triangles.Add(index + 1);
        _triangles.Add(index + 3);
        _triangles.Add(index + 3);
        _triangles.Add(index + 1);
        _triangles.Add(index + 2);
    }

    private void Update()
    {
        //Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, 0);
    }

    private void OnDrawGizmos()
    {
        //if (!_showDebugGrid) {
        //    return;
        //}

        //Vector3 scale = transform.localScale;
        //Vector3 leftDown = transform.position + new Vector3(-scale.x/2, 0, -scale.z/2);
        //Vector3 rightDown = transform.position + new Vector3(scale.x/2, 0, -scale.z/2);
        //Vector3 leftUp = transform.position + new Vector3(-scale.x/2, 0, scale.z/2);
        //Vector3 rightUp = transform.position + new Vector3(scale.x/2, 0, scale.z/2);

        //Gizmos.color = Color.red;
        //DrawCube(leftDown, rightDown, rightUp, leftUp);

        //Gizmos.color = Color.white;

        //float width = scale.x;
        //float height = scale.z;

        //float cellWidth = width/_width;
        //float cellHeight = height/_height;
        //for (int i = 0; i < _width; ++i) {
        //    for (int j = 0; j < _height; ++j) {
        //        Vector3 startPos = leftDown + new Vector3(i*cellWidth, 0, j*cellHeight);
        //        DrawCube(startPos, startPos + new Vector3(cellWidth, 0, 0), startPos + new Vector3(cellWidth, 0, cellHeight), startPos + new Vector3(0, 0, cellHeight));
        //    }
        //}

        //// 原点
        //Gizmos.DrawSphere(leftDown, 1);
    }

    private void DrawCube(Vector3 leftDown, Vector3 rightDown, Vector3 rightUp, Vector3 leftUp)
    {
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(rightDown, rightUp);
        Gizmos.DrawLine(leftUp, rightUp);
    }
    
    public Mesh GenerateMesh()
    {
        Vector3[] Vertices = new Vector3[4];
        int[] Triangles = new int[6];
        Vertices[0].x = -(_cellWidth*0.5f);
        Vertices[0].z = -(_cellHeight*0.5f);
        Vertices[1].x = Vertices[0].x + _cellWidth;
        Vertices[1].z = Vertices[0].z;
        Vertices[2].x = Vertices[0].x;
        Vertices[2].z = Vertices[0].z + _cellHeight;
        Vertices[3].x = Vertices[0].x + _cellWidth;
        Vertices[3].z = Vertices[0].z + _cellHeight;
        Triangles[0] = 3;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 2;
        Triangles[4] = 1;
        Triangles[5] = 0;

        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        return mesh;
    }
}