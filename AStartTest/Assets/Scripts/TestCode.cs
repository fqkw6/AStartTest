using UnityEngine;
using System.Collections;

public class TestCode : MonoBehaviour
{
    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public ArrayList pathArray;
    GameObject objStartCube, objEndCube;
    private float elapsedTime = 0f;

    // 路径搜寻间隔
    public float intervalTime = 1f;

    void Start()
    {
        objStartCube = GameObject.FindGameObjectWithTag("Start");
        objEndCube = GameObject.FindGameObjectWithTag("End");
        pathArray = new ArrayList();
        FindPath();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0f;
            FindPath();
        }
    }

    void FindPath()
    {
        startPos = objStartCube.transform;
        endPos = objEndCube.transform;
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));
        pathArray = AStar.FindPath(startNode, goalNode);
    }

    // 绘制路径
    void OnDrawGizmos()
    {
        if (pathArray == null) return;
        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            }
        }
    }
}
