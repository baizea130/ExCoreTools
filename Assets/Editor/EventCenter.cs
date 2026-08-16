using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IGameEvent { }
public static class EventCenter
{
    /// <summary>
    /// <para>事件字典</para>
    /// 使用方法为定义专属结构体，获取其类型作为该字典的Key，相应的，对字典操作时的传参也是相应结构体
    /// </summary>
    private static Dictionary<Type, Delegate> mEventDict = new Dictionary<Type, Delegate>();
    /// <summary>
    /// 为事件增加监听
    /// </summary>
    /// <typeparam name="T">实现IGameEvent的类，用作Dict的Key</typeparam>
    /// <param name="action">待增加的行为</param>
    public static void AddListener<T>(Action<T> action) where T : IGameEvent
    {
        Type key = typeof(T);
        if (!mEventDict.TryGetValue(key, out var item))
        {
            mEventDict[key] = action;
            return;
        }
        Action<T> existing = (Action<T>)item;
        existing += action;
        mEventDict[key] = existing;
    }
    /// <summary>
    /// 为事件移除监听器
    /// </summary>
    /// <param name="action">待移除的行为</param>
    public static void RemoveListener<T>(Action<T> action) where T : IGameEvent
    {
        Type key = typeof(T);
        if (!mEventDict.TryGetValue(key, out var item))
        {
            return;
        }
        Action<T> existing = (Action<T>)item;
        existing -= action;
        if (existing == null) 
        {
            mEventDict.Remove(key);
            return;
        }
        mEventDict[key] = existing;
    }
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="param">待触发的行为</param>
    public static void TriggerEvent<T>(T param) where T : IGameEvent
    {
        Type key = typeof(T);
        if (!mEventDict.TryGetValue(key, out var item))
        {
            Debug.LogError($"不存在事件：{key.Name}");
            return;
        }
        (item as Action<T>)?.Invoke(param);
    }
    /// <summary>
    /// 清空事件字典
    /// </summary>
    public static void Clear()
    {
        mEventDict.Clear();
    }
    /// <summary>
    /// 查看所有注册事件的Key
    /// </summary>
    public static void DebugEventCenterName()
    {
        Debug.Log("=======================");
        for (int i = 0; i < mEventDict.Count; i++)
        {
            var key = mEventDict.Keys.ElementAt(i);
            Debug.Log(i + ".key:" + key);
        }
        Debug.Log("=======================");
    }
}
