using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GraphSearch 
{
    private Graph graph;
    private Tile tile;
    private Map map;
    public List<GraphNode> path = new List<GraphNode>();
    public List<Tile> tiles = new List<Tile>();

    public void Init(Map map)
    {
        this.map = map;
    }

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

    public bool Dikjstra(GraphNode start, GraphNode goal)
    {
        path.Clear();
        graph.ReSetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>(); //현재까지의 누적거리가 작은 순서대로 노드를 꺼내기 위한 큐

        var distances = new int[graph.nodes.Length]; //그래프의 모든 노드의 현재까지의 최소 거리를 저장할 배열

        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight; //일단 스타트에서 스타트 노드까지의 거리를 초기화?
        pQueue.Enqueue(start, distances[start.id]); //시작 노드를 우선순위 큐에 넣음
        

        bool success = false;
        while (pQueue.Count > 0)
        { 
            var currentNode = pQueue.Dequeue(); //큐에서 현재까지의 거리가 가장 짧은 노드 꺼냄
            if (visited.Contains(currentNode)) continue; //방문한 노드 제외

            if (currentNode == goal) //현재노드가 목표면 탐색 성공하고 루프종료
            { 
                success = true;
                break;
            }
            
            visited.Add(currentNode); //현재 노드 방문 처리

            foreach (var adj in currentNode.neighbors)
            {
                if (!adj.CanVisit || visited.Contains(adj)) //노드들의 이웃노드 확인(방문할수 없거나 비시트에 들어가있으면 안함)
                    continue;

                var newDistance = distances[currentNode.id] + adj.weight; //이웃노드까지의 새로운 거리 계산

                if (distances[adj.id] > newDistance) //새로 계산한 거리가 기존보다 짧으면 변경
                {
                    distances[adj.id] = newDistance;
                    adj.previous = currentNode;
                    pQueue.Enqueue(adj, newDistance);
                }
            }
        
        }

        if (!success) //큐를 다 돌았는데 목표를 찾지못하면 경로없음
        {
            return false;
        }

        GraphNode step = goal;
        while (step != null) //
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true; //경로탐색 성공
    }

    protected int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    protected int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % map.cols;
        int ay = a.id / map.cols;

        int bx = b.id % map.cols;
        int by = b.id / map.cols;

        return Mathf.Abs(ax-bx) + Mathf.Abs(ay-by);
    }

    public bool AStar(GraphNode start, GraphNode goal)
    {
        path.Clear();

        graph.ReSetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();

        var distances = new int[graph.nodes.Length];
        var scores = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        scores[start.id] = distances[start.id] + Heuristic(start, goal);

        pQueue.Enqueue(start, scores[start.id]);

        bool success = false;

        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();

            if (visited.Contains(currentNode))
                continue;

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            foreach (var adj in currentNode.neighbors)
            {
                if (!adj.CanVisit || visited.Contains(adj))
                    continue;

                var newDistance = distances[currentNode.id] + adj.weight;
                if (distances[adj.id] > newDistance)
                {
                    distances[adj.id] = newDistance;
                    scores[adj.id] = distances[adj.id] + Heuristic(adj, goal);
                    adj.previous = currentNode;

                    pQueue.Enqueue(adj, scores[adj.id]);
                }

            }

        }

        if (!success)
        {
            return false;
        }

        GraphNode step = goal;
        while (step != null) //
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;
    }


    public bool AStar(Tile startTile, Tile endTile)
    {
        tiles.Clear();

        foreach (var tile in map.tiles)
        {
            tile.Clear();
        }

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, int>();

        var distances = new int[map.tiles.Length];
        var scores = new int[map.tiles.Length];
        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }

        distances[startTile.id] = startTile.Weight;
        scores[startTile.id] = distances[startTile.id] + Heuristic(startTile, endTile);

        pQueue.Enqueue(startTile, scores[startTile.id]);

        bool success = false;

        while (pQueue.Count > 0)
        {
            var currentTile = pQueue.Dequeue();

            if (visited.Contains(currentTile))
                continue;

            if (currentTile == endTile)
            {
                success = true;
                break;
            }

            visited.Add(currentTile);

            foreach (var adj in currentTile.adjacents)
            {
                if (adj == null || !adj.CanMove || visited.Contains(adj) )
                    continue;

                var newDistance = distances[currentTile.id] + adj.Weight;
                if (distances[adj.id] > newDistance)
                {
                    distances[adj.id] = newDistance;
                    scores[adj.id] = distances[adj.id] + Heuristic(adj, endTile);
                    adj.previous = currentTile;

                    pQueue.Enqueue(adj, scores[adj.id]);
                }

            }

        }

        if (!success)
        {
            return false;
        }

        Tile step = endTile;
        while (step != null)
        {
            tiles.Add(step);
            step = step.previous;
        }

        tiles.Reverse();
        return true;
    }
}
