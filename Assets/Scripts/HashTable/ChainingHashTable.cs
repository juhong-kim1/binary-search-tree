using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.6;

    //private KeyValuePair<TKey, TValue>[] table;

    private LinkedList<KeyValuePair<TKey, TValue>>[] table;

    private int size;
    private int count;

    public ChainingHashTable()
    {
        table = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetIndex(key);

            if (table[index] != null)
            {
                foreach (var kvp in table[index])
                {
                    if (kvp.Key.Equals(key))
                        return kvp.Value;
                }
            }

            throw new KeyNotFoundException("해당 키가 존재하지 않습니다.");
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetIndex(key);

            if (table[index] == null)
                table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();

            var node = table[index].First;
            while (node != null)
            {
                if (node.Value.Key.Equals(key))
                {
                    node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
                node = node.Next;
            }

            table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }
    }


    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();

            foreach (var list in table)
            {
                if (list != null)
                {
                    foreach (var kvp in list)
                    {
                        keys.Add(kvp.Key);
                    }
                }
            }

            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();

            foreach (var list in table)
            {
                if (list != null)
                {
                    foreach (var kvp in list)
                    {
                        values.Add(kvp.Value);
                    }
                }
            }
            return values;
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    private int GetIndex(TKey key, int size)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hash = key.GetHashCode();
        return Math.Abs(hash) % size;
    }

    private int GetIndex(TKey key)
    {
        return GetIndex(key, this.size);
    }

    public void Add(TKey key, TValue value)
    {
        if ((double)count / size >= LoadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);

        if (table[index] == null)
        {
            table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        foreach (var kvp in table[index])
        {
            if (kvp.Key.Equals(key))
            {
                throw new ArgumentException("키 중복");
            }
        }

        table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }


    public void Resize()
    {
        int newSize = size * 2;
        var newTable = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];

        for (int i = 0; i < size; ++i)
        {
            if (table[i] == null)
                continue;

            foreach (var kvp in table[i])
            {
                int newIndex = GetIndex(kvp.Key, newSize);

                if (newTable[newIndex] == null)
                    newTable[newIndex] = new LinkedList<KeyValuePair<TKey, TValue>>();

                newTable[newIndex].AddLast(kvp);
            }
        }

        size = newSize;
        table = newTable;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null)
            throw new ArgumentNullException(nameof(item.Key));

        if (TryGetValue(item.Key, out TValue value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);

        if (table[index] == null)
            return false;

        foreach (var kvp in table[index])
        {
            if (kvp.Key.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if (array.Length - arrayIndex < count)
            throw new ArgumentException("배열의 공간이 부족합니다.");

        int i = arrayIndex;

        foreach (var list in table)
        {
            if (list != null)
            {
                foreach (var kvp in list)
                {
                    array[i++] = kvp;
                }
            }
        }
    }


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var list in table)
        {
            if (list != null)
            {
                foreach (var kvp in list)
                {
                    yield return kvp;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    table[index].Remove(kvp);
                   // kvp = default;
                    --count;
                    return true;
                }
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    value = kvp.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
