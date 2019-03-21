using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NZQLA;

public class PoolManager<T> :IPoolCreateItem<T>, IRecycleItem<T>, IPoolGetItem<T>
{
    private Queue<T> poolQueue = new Queue<T>();

    public virtual T CreateItem()
    {
        return Activator.CreateInstance<T>();
    }

    public T GetOneItem()
    {
        if (poolQueue.Count >= 1)
        {
            return poolQueue.Dequeue();
        }
        else
        {
            return CreateItem();
        }
    }

    public virtual void RecycleItem(T data)
    {
        if (data.GetType().IsValueType)
        {
            data = default(T);
        }
        poolQueue.Enqueue(data);
    }



}


public interface IPoolCreateItem<T>
{
    T CreateItem();
}

public interface IPoolGetItem<T>
{
    T GetOneItem();
}

public interface IRecycleItem<T>
{
    void RecycleItem(T data);
}

public interface IPoolCloneItem<T>
{
    T CloneOneItem(T prefab);
}