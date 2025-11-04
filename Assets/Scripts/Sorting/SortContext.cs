using System;
using System.Collections.Generic;

public interface ISortStrategy<T>
{
    void Sort(T[] array, IComparer<T> comparer);
}

public interface ISortContext<T>
{
    void SetStrategy(ISortStrategy<T> strategy);
    void Sort(T[] array);
}

public class SortContext<T> : ISortContext<T>
{
    private ISortStrategy<T> strategy;
    private IComparer<T> comparer;

    public SortContext()
    {
        comparer = Comparer<T>.Default;
    }

    public void SetStrategy(ISortStrategy<T> strategy)
    {
        this.strategy = strategy;
    }

    public void Sort(T[] array)
    {
        if (strategy == null)
        {
            return;
        }
        strategy.Sort(array, comparer);
    }
}

// 1. 버블 정렬
public class BubbleSortStrategy<T> : ISortStrategy<T>
{
    private IComparer<T> comparer;

    public BubbleSortStrategy()
    {
    }

    public BubbleSortStrategy(IComparer<T> comparer)
    {
        this.comparer = comparer;
    }

    public void Sort(T[] array, IComparer<T> comparer)
    {
        IComparer<T> actualComparer = this.comparer ?? comparer;
        int n = array.Length;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (actualComparer.Compare(array[j], array[j + 1]) > 0)
                {
                    T temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }
}

// 2. 삽입 정렬
public class InsertionSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] array, IComparer<T> comparer)
    {
       
    }
}

// 3. 선택 정렬
public class SelectionSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] array, IComparer<T> comparer)
    {

    }
}

// 4. 병합 정렬
public class MergeSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] array, IComparer<T> comparer)
    {
    }
}

// 5. 퀵 정렬
public class QuickSortStrategy<T> : ISortStrategy<T>
{
    public QuickSortStrategy()
    {
    }

    public QuickSortStrategy(IComparer<T> comparer)
    {
    }

    public void Sort(T[] array, IComparer<T> comparer)
    {
    }
}

// 6. 힙 정렬
public class HeapSortStrategy<T> : ISortStrategy<T>
{
    public void Sort(T[] array, IComparer<T> comparer)
    {
    }
}
