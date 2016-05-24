using UnityEngine;
using System.Collections;
using System;

public class Node : IComparable
{
    public float nodeTotalCost;  // 从开始节点到当前节点的代价值
    public float estimatedCost;  // 从当前节点到目标节点的估计值
    public bool bObstacle;
    public Node parent;
    public Vector3 position;

    public Node()
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
    }

    public Node(Vector3 pos)
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
        this.position = pos;
    }

    public void MarsksObstacle()
    {
        this.bObstacle = true;
    }

    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        if (this.estimatedCost < node.estimatedCost)
            return -1;
        if (this.estimatedCost > node.estimatedCost)
            return 1;
        return 0;
    }
}
