using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    public UiGraphNode nodePrefab;
    public List<UiGraphNode> uiNodes;

    public Transform uiNodeRoot;

    private Graph graph;

    public enum AlgorithmType
    {
        DFS,
        BFS,
        DFS_Recursive,
        PathFindingBFS,
    }

    private void Start()
    {
        int[,] map = new int[5, 5]
        {
            {1,-1,1,1,1 },
            {1,-1,1,1,1 },
            {1,-1,1,1,1 },
            {1,-1,1,1,1 },
            {1,1,1,1,1 },
        };

        graph = new Graph();
        graph.Init(map); //여기서 그래프를 초기화 해줌 위 배열로
        InitUINodes(graph); //Ui 그려줌
    }

    public AlgorithmType algorithmType;
    public int startIndex;
    public int endIndex;

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph); //그래프 할당
        switch(algorithmType)
        {
            case AlgorithmType.DFS:
                search.DFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.BFS:
                search.BFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.DFS_Recursive:
                search.DFS_Recursive(graph.nodes[startIndex]);
                break;
        }
        ResetUINodes();

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var uiNode = uiNodes[node.id];
            uiNode.SetColor(Color.Lerp(Color.red, Color.green, (float)i / search.path.Count));
            uiNode.SetText($"ID: {node.id.ToString()} \nWeight: {node.weight} \nPath: {i}");
        }
    }

    private void InitUINodes(Graph graph)
    {
        uiNodes = new List<UiGraphNode>();
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }

    private void ResetUINodes()
    {
        foreach (var uiNode in uiNodes)
        {
            uiNode.Reset();
        }
    }
}
