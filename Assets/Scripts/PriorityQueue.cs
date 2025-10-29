using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    public List<(TElement Element, TPriority Priority)> heap;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
    }

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));

        HeapifyUp(Count-1);
    }

    public TElement Dequeue()
    {
        if (heap == null)
            throw new InvalidOperationException();

        var root = heap[0];

        heap[0] = heap[Count - 1];
        heap.RemoveAt(heap.Count-1);

        HeapifyDown(0);

        return root.Element;

        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 저장
        // 3. 마지막 요소를 루트로 이동
        // 4. HeapifyDown으로 힙 속성 복구
        // 5. 저장된 루트 요소 반환
    }

    public TElement Peek()
    {
        if (heap == null)
            throw new InvalidOperationException();

        var root = heap[0];

        return root.Element;

        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 반환
    }

    public void Clear()
    {
        if (heap == null)
            throw new InvalidOperationException();

        heap ?.Clear();

        // TODO: 구현
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (heap[index].Priority.CompareTo(heap[parent].Priority) < 0)
            { 
                 var temp = heap[parent];
                 heap[parent] = heap[index];
                 heap[index] = temp;
            }
            else
            {
                return;
            }

            index = parent;
        }

        // TODO: 구현
        // 현재 노드가 부모보다 작으면 교환하며 위로 이동
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int leftSon = 2 * index + 1;
            int rightSon = 2 * index + 2;
            int smallest = index;

            if (leftSon < heap.Count && heap[leftSon].Priority.CompareTo(heap[smallest].Priority) < 0)
                smallest = leftSon;

            if (rightSon < heap.Count && heap[rightSon].Priority.CompareTo(heap[smallest].Priority) < 0)
                smallest = rightSon;

            if (smallest == index)
                return;

            var temp = heap[index];
            heap[index] = heap[smallest];
            heap[smallest] = temp;

            index = smallest;
        }
    }
}
