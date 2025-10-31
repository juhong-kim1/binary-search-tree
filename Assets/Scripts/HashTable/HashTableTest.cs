using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HashTableTest : MonoBehaviour
{
    private void Start()
    {
        var hashText = new ChainingHashTable<string, int>();

        hashText.Add("one", 1);
        hashText.Add("two", 2);
        hashText.Add("three", 3);

        foreach(var kvp in hashText)
        {
            Debug.Log($"{kvp.Key}, {kvp.Value}");
        }

        hashText.Remove("one");

        foreach (var kvp in hashText)
        {
            Debug.Log($"{kvp.Key}, {kvp.Value}");
        }

        var right = hashText.ContainsKey("three");

        Debug.Log($"three contains: {right}");

        var no = hashText.ContainsKey("one");

        Debug.Log($"three contains: {no}");

        KeyValuePair<string, int>[] array = new KeyValuePair<string, int>[hashText.Count];

        hashText.CopyTo(array, 0);

        foreach (var kvp in array)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }
}
