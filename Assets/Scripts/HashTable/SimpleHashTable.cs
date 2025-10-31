using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occuiped;

    private int size;
    private int count;

    public SimpleHashTable()
    {
        table = new KeyValuePair<TKey, TValue>[DefaultCapacity];
        occuiped = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;      
    }

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


    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException("키 없음!");
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            int index = GetIndex(key);
            if (occuiped[index] && table[index].Key.Equals(key))
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else if (!occuiped[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occuiped[index] = true;
                ++count;
            }
            else
            {
                throw new InvalidOperationException("해시 충돌!");
            }
        }
    }

    public ICollection<TKey> Keys => Enumerable.Range(0, size).Where(i => occuiped[i]).Select(i => table[i].Key).ToList();

    public ICollection<TValue> Values => Enumerable.Range(0, size).Where(i => occuiped[i]).Select(i => table[i].Value).ToList();

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if ((double)count / size >= LoadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);
        if (!occuiped[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occuiped[index] = true;
            ++count;
        }
        else if (table[index].Key.Equals(key))
        {
            throw new ArgumentException("키 중복");
        }
        else
        {
            throw new InvalidOperationException("해시 충돌");
        }
    }

    public void Resize()
    {
        int newSize = size * 2;
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; ++i)
        {
            if (!occuiped[i])
                continue;

            int newIndex = GetIndex(table[i].Key, newSize);
            if (newOccupied[newIndex])
            {
                throw new InvalidOperationException("해시 충돌");
            }

            newTable[newIndex] = table[i];
            newOccupied[newIndex] = true;
        }

        size = newSize;
        table = newTable;
        occuiped = newOccupied;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occuiped, 0, size);
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
        return occuiped[index] && table[index].Key.Equals(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("배열 크기가 충분하지 않습니다.");

        int currentIndex = arrayIndex;
        for (int i = 0; i < size; ++i)
        {
            if (occuiped[i])
            {
                array[currentIndex++] = table[i];
            }
        }
    }


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; ++i)
        {
            if (occuiped[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);
        if (occuiped[index] && table[index].Key.Equals(key))
        {
            occuiped[index] = false;
            table[index] = default;
            --count;
            return true;
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
        if (occuiped[index] && table[index].Key.Equals(key))
        {
            value = table[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
