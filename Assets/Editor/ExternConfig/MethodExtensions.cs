using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class MethodExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                comp = Undo.AddComponent<T>(go);
            else
                comp = go.AddComponent<T>();
#else
            comp = go.AddComponent<T>();
#endif
        }
        return comp;
    }
    /// <summary>
    /// 在Assets/Resources/Config下创建一个名称和SO定义脚本类同名的SO，若已存在则获取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateSO<T>() where T : ScriptableObject
    {
        string dir = "Assets/Resources/SO";
        string path = $"{dir}/{typeof(T).Name}.asset";
        T existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null)
            return existing;

        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }
    /// <summary>
    /// 自动给两个类中名称/类型相同的字段建立关联，用于写入/读取配置文件SO
    /// </summary>
    /// <param name="target">目标类</param>
    /// <param name="source">源类</param>
    public static void AutoMap(object target, object source)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var sourceType = source.GetType();
        var targetType = target.GetType();

        // 收集源对象的所有字段
        var sourceFields = new Dictionary<string, FieldInfo>();
        var t = sourceType;
        while (t != null && t != typeof(object))
        {
            foreach (var f in t.GetFields(flags))
                if (!sourceFields.ContainsKey(f.Name))
                    sourceFields[f.Name] = f;
            t = t.BaseType;
        }

        // 遍历目标对象字段，同名同类型则赋值
        t = targetType;
        while (t != null && t != typeof(object))
        {
            foreach (var f in t.GetFields(flags))
            {
                if (sourceFields.TryGetValue(f.Name, out var srcField)
                    && srcField.FieldType == f.FieldType)
                {
                    f.SetValue(target, srcField.GetValue(source));
                }
            }
            t = t.BaseType;
        }
    }
    public static void Execute(string token, [CallerFilePath] string src = "", [CallerLineNumber] int line = -1)
    {
        string fileName = Path.GetFileNameWithoutExtension(src);
        InternalCall(token, fileName, line);
    }
    private static void InternalCall(string token, string fileName, int line)
    {
        Debug.Log($"{token}:{fileName}:{line}");
    }
}
