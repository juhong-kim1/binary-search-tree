using System;
using UnityEngine;

public class BinaryTreeTest : MonoBehaviour
{
    public BinaryTreeVisualizer treeVisualizer;

    [SerializeField] private int nodeCount = 10;
    [SerializeField] private int minKey = 1;
    [SerializeField] private int maxKey = 1000;

    private PriorityQueue<string, int> pq = new PriorityQueue<string, int>();

    private void Start()    
    {
        GenerateRandomTree();

        var scheduler = new PriorityQueue<string, int>();

        scheduler.Enqueue(new string("Render"), 2);
        scheduler.Enqueue(new string("Physics"), 1);
        scheduler.Enqueue(new string("AI"), 3);

        Debug.Log($"작업 수: {scheduler.Count}"); // 출력: 3

        string task = scheduler.Dequeue();
        Debug.Log($"첫 번째 작업: {task}"); // 출력: Physics

        task = scheduler.Peek();
        Debug.Log($"다음 작업: {task}"); // 출력: Render

        scheduler.Clear();
        Debug.Log($"Clear 후 작업 수: {scheduler.Count}"); // 출력: 0


    }

    public void GenerateRandomTree()
    {
        var tree = new VisualizableBST<int, string>();

        int addedNodes = 0;
        while (addedNodes < nodeCount)
        {
            int key = UnityEngine.Random.Range(minKey, maxKey + 1);

            if (!tree.ContainsKey(key))
            {
                string value = $"V-{key}";
                tree.Add(key, value);
                addedNodes++;
            }
        }

        treeVisualizer.VisualizeTree(tree);
    }

    [ContextMenu("Generate New Random Tree")]
    public void RegenerateTree()
    {
        GenerateRandomTree();
    }
}