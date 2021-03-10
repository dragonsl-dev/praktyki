using UnityEngine;
using System.Collections.Generic;

public class Container<T> : MonoBehaviour
{
    public Transform storage;
    private Dictionary<Transform, T> lookup = new Dictionary<Transform, T>();
    public event System.Action<Container<T>> OnModify;
    public int Count => storage.childCount;

    public void Shuffle()
    {
        for (int i = 0; i < storage.childCount; i++)
        {
            storage.GetChild(i).SetSiblingIndex(Random.Range(0, storage.childCount));
        }
    }

    public void Add(T obj)
    {
        var transf = ((Component)(object)obj).transform;
        transf.parent = storage.transform;
        lookup.Add(transf, obj);

        OnModify?.Invoke(this);
    }

    public void TakeFrom(Container<T> list, int index)
    {
        var obj = list.Get(index);
        Add(obj);
    }

    public T Get(int index)
    {
        return lookup[storage.GetChild(index)];
    }

    public T[] GetArray()
    {
        List<T> list = new List<T>();
        foreach(Transform i in storage)
        {
            list.Add(lookup[i]);
        }
        return list.ToArray();
    }

    public void Remove(int index)
    {
        Destroy(storage.GetChild(index).gameObject);
    }

    public bool TryRemove(int index)
    {
        if (index > storage.childCount - 1)
            return false;
        else
        {
            Remove(index);
            return true;
        }
    }

    public void Clear()
    {
        foreach (Transform i in storage)
        {
            Destroy(i.gameObject);
        }
    }
}
