using LitJson;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class MapGridEditor : MonoBehaviour
{

    public int _width;
    public int _height;

    private Mesh _mesh;
    private Dictionary<string, Occlusion> _occlusionDictionary = new Dictionary<string, Occlusion>();  // 障碍网格字典
    private Material _material;
    private Material[] _occlusionMaterial;  // 各种颜色标记

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Vector3 _originPosition;    // 原点坐标
    private float _cellWidth;
    private float _cellHeight;
    private int _currentMarkCellWeight = 0;

    struct Occlusion
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
        if (_width == 0)
        {
            _width = 1;
        }

        if (_height == 0)
        {
            _height = 1;
        }
        _mesh = new Mesh();
        _material = new Material(Shader.Find("Shader/BlackLine"));
        _material.color = new Color(0, 1, 0, 1f);
        _occlusionMaterial = new Material[4];
        for (int i = 0; i < 4; i++)
        {
            Material material = Instantiate<Material>(_material);
            _occlusionMaterial[i] = material;
        }
        _occlusionMaterial[0].color = new Color(0, 0, 0, 0);
        _occlusionMaterial[1].color = Color.white;
        _occlusionMaterial[2].color = Color.yellow;
        _occlusionMaterial[3].color = Color.red;
    }

    void Start()
    {
        CreateGrid();
        LoadMapOcclusionConfig();
    }

    public void SetCurrentMarkCellWeight(int weight)
    {
        _currentMarkCellWeight = weight;
    }

    // 世界坐标转为格子坐标
    public Vector2 World2Cell(Vector3 pos)
    {
        Vector2 cell = new Vector3();
        cell.x = Mathf.FloorToInt((pos.x - _originPosition.x) / _cellWidth);
        cell.y = Mathf.FloorToInt((pos.z - _originPosition.z) / _cellHeight);
        return cell;
    }

    // 格子坐标转为世界坐标
    public Vector3 Cell2World(Vector2 cell)
    {
        Vector3 pos = new Vector3();
        pos.x = _originPosition.x + cell.x * _cellWidth + _cellWidth / 2;
        pos.y = 0;
        pos.z = _originPosition.z + cell.y * _cellHeight + _cellHeight / 2;
        return pos;
    }

    private void CreateGrid()
    {
        Vector3 scale = transform.localScale;
        // 原点坐标为左下角
        _originPosition = transform.position + new Vector3(-scale.x / 2, 0.3f, -scale.z / 2);

        // 网格的长宽
        float width = scale.x;
        float height = scale.z;

        // 计算每个格子长宽
        _cellWidth = width / _width;
        _cellHeight = height / _height;
        const float WIDTH = 0.5f;

        // 行 注意等于号，以便形成封闭的格子
        for (int i = 0; i <= _height; ++i)
        {
            Vector3 pos = _originPosition + new Vector3(0, 0, i * _cellHeight);
            AddLine(pos, pos + new Vector3(width, 0, 0), WIDTH);
        }

        // 列
        for (int i = 0; i <= _width; ++i)
        {
            Vector3 pos = _originPosition + new Vector3(i * _cellWidth, 0, 0);
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

        Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, 0);

        if (_occlusionDictionary.Count > 0)
            foreach (string key in _occlusionDictionary.Keys)
            {
                string cellPos = key;
                string[] posStr = cellPos.Split('.');
                int x = int.Parse(posStr[0]);
                int y = int.Parse(posStr[1]);
                int weight = _occlusionDictionary[key].weight;
                Graphics.DrawMesh(_occlusionDictionary[key]._mesh, Cell2World(new Vector2(x, y)), Quaternion.identity, _occlusionMaterial[weight], 0);
            }
    }

    private void OnDrawGizmos()
    {
        Vector3 scale = transform.localScale;
        Vector3 leftDown = transform.position + new Vector3(-scale.x / 2, 0, -scale.z / 2);
        Vector3 rightDown = transform.position + new Vector3(scale.x / 2, 0, -scale.z / 2);
        Vector3 leftUp = transform.position + new Vector3(-scale.x / 2, 0, scale.z / 2);
        Vector3 rightUp = transform.position + new Vector3(scale.x / 2, 0, scale.z / 2);

        Gizmos.color = Color.red;
        DrawCube(leftDown, rightDown, rightUp, leftUp);

        Gizmos.color = Color.white;

        float width = scale.x;
        float height = scale.z;
        float cellWidth = width / _width;
        float cellHeight = height / _height;
        for (int i = 0; i < _width; ++i)
        {
            for (int j = 0; j < _height; ++j)
            {
                Vector3 startPos = leftDown + new Vector3(i * cellWidth, 0, j * cellHeight);
                DrawCube(startPos, startPos + new Vector3(cellWidth, 0, 0), startPos + new Vector3(cellWidth, 0, cellHeight), startPos + new Vector3(0, 0, cellHeight));
            }
        }
    }

    private void DrawCube(Vector3 leftDown, Vector3 rightDown, Vector3 rightUp, Vector3 leftUp)
    {
        Gizmos.DrawLine(leftDown, rightDown);
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(rightDown, rightUp);
        Gizmos.DrawLine(leftUp, rightUp);
    }


    public void MarkOcclusion(Vector3 occlusionPos)
    {
        // 为 0 的时候取消标记，但必须保留一个，不然加载的时候出错
        if (_currentMarkCellWeight == 0 && _occlusionDictionary.Count > 1)
        {
            if (_occlusionDictionary.ContainsKey(occlusionPos.x.ToString() + '.' + occlusionPos.y.ToString()))
            {
                // 这里不一定 remove 也可以直接设置 weight 为 0
                _occlusionDictionary.Remove(occlusionPos.x.ToString() + '.' + occlusionPos.y.ToString());
            }
        }
        else
        {
            Occlusion occ = new Occlusion(GenerateMesh(), _currentMarkCellWeight);
            _occlusionDictionary[occlusionPos.x.ToString() + '.' + occlusionPos.y.ToString()] = occ;
        }
    }

    /// 加载地图配置
    public void LoadMapOcclusionConfig()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/MapOcclusionConfig"));
        for (int i = 0; i < jsondata.Count; ++i)
        {
            string weight = jsondata[i]["weight"].ToString();
            Occlusion occlusion = new Occlusion(GenerateMesh(), int.Parse(weight));
            _occlusionDictionary[jsondata[i]["position"].ToString()] = occlusion;
        }
    }


    public Mesh GenerateMesh()
    {
        Vector3[] Vertices = new Vector3[4];
        int[] Triangles = new int[6];
        Vertices[0].x = -(_cellWidth * 0.5f);
        Vertices[0].z = -(_cellHeight * 0.5f);
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

    public void SaveMap()
    {
        string configFilePath = Application.dataPath + @"/Resources/config/MapOcclusionConfig.json";
        if (!File.Exists(configFilePath))
        {
            Debug.LogError("dont exist configFile: " + configFilePath);
            return;
        }

        StringBuilder configStrBuilder = new StringBuilder();
        foreach (string key in _occlusionDictionary.Keys)
        {
            configStrBuilder.Append("{");
            configStrBuilder.Append(AddProperty("position", key));
            configStrBuilder.Append(AddProperty("weight", _occlusionDictionary[key].weight.ToString()));
            // 移除最后一个分号
            configStrBuilder.Remove(configStrBuilder.Length - 1, 1);
            configStrBuilder.Append("}");
            configStrBuilder.Append(",");
        }
        // 移除最后一个分号
        configStrBuilder.Remove(configStrBuilder.Length - 1, 1);
        configStrBuilder.Insert(0, "[");
        configStrBuilder.Append("]");

        FileInfo fileInfo = new FileInfo(configFilePath);
        StreamWriter sw = fileInfo.CreateText();
        sw.WriteLine(configStrBuilder);
        sw.Close();
        sw.Dispose();

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public string AddProperty(string propertyName, string value)
    {
        StringBuilder propertyStr = new StringBuilder();
        propertyStr.Append("\"");
        propertyStr.Append(propertyName);
        propertyStr.Append("\"");
        propertyStr.Append(":");
        propertyStr.Append("\"");
        propertyStr.Append(value);
        propertyStr.Append("\"");
        propertyStr.Append(",");

        return propertyStr.ToString();
    }
}
