using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GraphNode
{
    public int id;
    public int weight = 1; // 가중치

    public List<GraphNode> neighbors = new List<GraphNode>();


    public GraphNode previous = null;

    public bool CanVisit => neighbors.Count > 0;

}
