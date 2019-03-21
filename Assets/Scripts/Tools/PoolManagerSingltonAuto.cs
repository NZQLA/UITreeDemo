using UnityEngine;
using System.Collections;

/// <summary>
/// 为PoolManager添加单例机制
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolManagerSingltonAuto<T> : PoolManager<T>
{
    private static readonly object objLock = new object();

    private static PoolManagerSingltonAuto<T> Ins;


    /// <summary>获取实例</summary>
    public static PoolManagerSingltonAuto<T> GetIns()
    {
        if (Ins == null)
        {
            lock (objLock)
            {
                Ins = new PoolManagerSingltonAuto<T>();
            }
        }
        return Ins;
    }

}
