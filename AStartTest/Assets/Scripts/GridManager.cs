using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    private static GridManager s_Instance = null;
    public static GridManager instance
    {
        get 
        {
            if(s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GridManager)) as GridManager;
                if (s_Instance == null)
                    print("Could not locate a GrigManager " + "object. \n You have to have exactly " + "one GridManager in the Scene.");
            }
            return s_Instance;
        }
    }
    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;
    public bool showObstacleBlocks = true;
    private Vector3 origin = new Vector3();
    private GameObject[] obstacleList;
    public Node[,] nodes { get; set; }
    public Vector3 Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    void Awake()
    {
        obstacleList = GameObject.FindGameObjectsWithTag("Obscatle");
        CalculateObscatles();
    }

    // 搜索地图上所有的障碍物
    void CalculateObscatles()
    {
        nodes = new Node[numOfColumns, numOfRows];
        int index = 0;
        for(int i = 0; i < numOfColumns; i++)
        {
            for(int j = 0; j < numOfRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index);
                Node node = new Node(cellPos);
                nodes[i, j] = node;
                index++;
            }
        }
        if(obstacleList != null && obstacleList.Length > 0)
        {
            // 遍历地图上的障碍物，在列表中对它进行标记
            foreach(GameObject data in obstacleList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);
                nodes[row, col].MarsksObstacle();
            }
        }
    }
    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize/2.0f);
        cellPosition.z += (gridCellSize/2.0f);
        return cellPosition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, 0f, zPosInGrid);
    }

    public int GetGridIndex(Vector3 pos)
    {
        if(!IsInBounds(pos))
        {
            return -1;
        }
        pos -= Origin;
        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);
        return (row * numOfColumns + col);
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;
        return (pos.x >= Origin.x && pos.x <= Origin.x + width && pos.z <= Origin.z + height && pos.z >= Origin.z);
    }

    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }

    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    // 被AStar类用于检索特定节点的邻接点
    public void GetNeighbours(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);
        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        // 底部
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 顶部
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 右边
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 左边
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
    }

    void AssignNeighbour(int row, int column, ArrayList neighbors)
    {
        if(row != -1 && column != -1 && row < numOfRows && column < numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];
            if(!nodeToAdd.bObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }
    }

    // 绘制地图和障碍物
    void OnDrawGizmos()
    {
        if(showGrid)
        {
            DebugDrawGrid(transform.position, numOfRows, numOfColumns, gridCellSize, Color.blue);
        }
        Gizmos.DrawSphere(transform.position, 0.5f);
        if(showObstacleBlocks)
        {
            Vector3 cellSize = new Vector3(gridCellSize, 1.0f, gridCellSize);
            if(obstacleList != null && obstacleList.Length > 0)
            {
                foreach(GameObject data in obstacleList)
                {
                    Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(data.transform.position)), cellSize);
                }
            }
        }
    }

    public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numRows * cellSize);

        // 绘制水平网格线条
        for(int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0f, 0f, 1f);
            Vector3 endPos = startPos + width * new Vector3(1f, 0f, 0f);
            Debug.DrawLine(startPos, endPos, color);
        }

        // 绘制垂直网格线条
        for(int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1f, 0f, 0f);
            Vector3 endPos = startPos + height * new Vector3(0f, 0f, 1f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }
}
