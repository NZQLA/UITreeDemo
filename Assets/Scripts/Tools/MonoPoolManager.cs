using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MonoPoolManager<T> : PoolManager<T>, IMonoPoolPrefab<T> where T:UnityEngine.Object,IMonoPoolItemRecycle<T>
{
    public T Prefab { get; set; }


    public override T CreateItem()
    {
        return GameObject.Instantiate<T>(Prefab);
    }

    public override void RecycleItem(T data)
    {
        data.MonoItemRecycle();
        base.RecycleItem(data);
    }


}

public interface IMonoPoolPrefab<T>
{
    T Prefab { get; set; }
}

public interface IMonoPoolItemRecycle<in T>
{
    void MonoItemRecycle();
}

