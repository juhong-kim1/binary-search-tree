using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GraphSearch 
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();


    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>(); //중복을 허용하지않는 집합임
        var stack = new Stack<GraphNode>(); 

        visited.Add(node);
        stack.Push(node);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            path.Add(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor); 
                stack.Push(neighbor);
            }
        }
    }

    public void DFS_Recursive(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();

        DFS_Recursive(node, visited);
    }

    public void DFS_Recursive(GraphNode node, HashSet<GraphNode> visited)
    {
        if (node == null || !node.CanVisit) return;
        if (visited.Contains(node)) return;

        visited.Add(node);

        path.Add(node);

        foreach (var neighbor in node.neighbors)
        {


            DFS_Recursive(neighbor, visited);
        }
    }


    public void BFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        visited.Add(node);
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);
            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);  
                queue.Enqueue(neighbor);
            }
            //foreach (var neighbor in current.neighbors)
            //{
            //    if (!neighbor.CanVisit || visited.Contains(neighbor) || queue.Contains(neighbor))
            //        continue;
            //    if (!visited.Contains(neighbor))
            //    {
            //        visited.Add(neighbor);
            //        queue.Enqueue(neighbor);
            //    }
            //}
        }
    }

    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(startNode);

        bool success = false;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);
            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
            //foreach (var neighbor in current.neighbors)
            //{
            //    if (!neighbor.CanVisit || visited.Contains(neighbor) || queue.Contains(neighbor))
            //        continue;
            //    if (!visited.Contains(neighbor))
            //    {
            //        visited.Add(neighbor);
            //        queue.Enqueue(neighbor);
            //    }
            //}
        }

        if (!success)
        {
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;

        }

        path.Reverse();
        return true;
    }
}
