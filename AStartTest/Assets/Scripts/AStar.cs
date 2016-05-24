using UnityEngine;
using System.Collections;

public class AStar
{
    public static PriorityQueue closedList, openList;

    // 计算带价值
    private static float HeuristicEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }


    public static ArrayList FindPath(Node start, Node goal)
    {
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0f;
        start.estimatedCost = HeuristicEstimateCost(start, goal);
        closedList = new PriorityQueue();
        Node node = null;
        while (openList.Length != 0)
        {
            node = openList.First();

            // 如果是目标节点
            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }

            // 创建一个 ArrayList 保存邻居节点
            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);

            // 开始检查当前节点相邻四个节点
            for (int i = 0; i < neighbours.Count; i++)
            {
                Node neighbourNode = (Node)neighbours[i];
                if (!closedList.Contains(neighbourNode))
                {
                    // 当前节点到邻居节点估值
                    float cost = HeuristicEstimateCost(node, neighbourNode);
                    // 开始节点到邻居节点估值
                    float totalCost = node.nodeTotalCost + cost;
                    // 邻居节点到目标节点估值
                    float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);
                    // 开始节点到邻居节点估值
                    neighbourNode.nodeTotalCost = totalCost;
                    neighbourNode.parent = node;
                    // 开始节点经过邻居节点到达目标节点的估值
                    neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost;
                    // 检查完估值加入开放列表
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Push(neighbourNode);
                    }
                }
            }

            // 当前节点检查完毕放入闭合列表
            closedList.Push(node);
            // 从开放列表移除检查过的节点
            openList.Remove(node);
        }

        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }
        return CalculatePath(node);
    }

    // 参数应该是终点
    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        // 顺序翻转，即成为从起点出发的路径
        list.Reverse();
        return list;
    }
}
