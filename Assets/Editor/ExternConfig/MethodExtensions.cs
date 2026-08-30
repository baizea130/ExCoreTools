using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections;

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
    /// 在Assets/Editor/ExternConfig/SO或Resources/SO下创建一个名称和SO定义脚本类同名的SO，若已存在则获取
    /// </summary>
    public static T GetOrCreateSO<T>(bool isEditor) where T : ScriptableObject
    {
        string dir;
        if (isEditor)
        {
            dir = "Assets/Editor/ExternConfig/SO";
        }
        else
        {
            dir = "Assets/Resources/SO";
        }
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
    public static void AutoMap(object target, object source)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var sourceType = source.GetType();
        var targetType = target.GetType();

        // 收集源对象的所有字段（含继承链）
        var sourceFields = new Dictionary<string, FieldInfo>();
        var t = sourceType;
        while (t != null && t != typeof(object))
        {
            foreach (var f in t.GetFields(flags))
                if (!sourceFields.ContainsKey(f.Name))
                    sourceFields[f.Name] = f;
            t = t.BaseType;
        }

        // 遍历目标对象字段
        t = targetType;
        while (t != null && t != typeof(object))
        {
            foreach (var f in t.GetFields(flags))
            {
                if (!sourceFields.TryGetValue(f.Name, out var srcField))
                    continue;

                var srcValue = srcField.GetValue(source);
                var tgtValue = f.GetValue(target);

                // ① 类型完全一致：直接反射赋值（普通字段、Asset 引用、同类型 List 等）
                if (srcField.FieldType == f.FieldType)
                {
                    f.SetValue(target, srcValue);
                    continue;
                }

                // ② 两边都是列表/数组：尝试元素级转换
                if (srcValue is IList srcList)
                {
                    var tgtList = tgtValue as IList;
                    // 如果目标 List 为 null，尝试创建实例（比如 SO 刚新建时）
                    if (tgtList == null && !f.FieldType.IsAbstract && !f.FieldType.IsInterface)
                    {
                        try
                        {
                            tgtList = Activator.CreateInstance(f.FieldType) as IList;
                            f.SetValue(target, tgtList);
                        }
                        catch { }
                    }

                    if (tgtList != null && TryMapList(srcList, tgtList, srcField.FieldType, f.FieldType))
                        continue;
                }

                // ③ 单个字段：UnityEngine.Object <-> string (GlobalObjectId)
                bool srcIsObj = typeof(UnityEngine.Object).IsAssignableFrom(srcField.FieldType);
                bool tgtIsObj = typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType);
                bool srcIsStr = srcField.FieldType == typeof(string);
                bool tgtIsStr = f.FieldType == typeof(string);

                if (srcIsObj && tgtIsStr)
                {
                    var obj = srcValue as UnityEngine.Object;
                    string id = "";
                    if (obj != null)
                    {
                        var gid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                        id = gid.ToString();
                    }
                    else
                    {
                        // 防御：源为null时，保留目标现有的有效GUID
                        string existing = f.GetValue(target) as string;
                        if (!string.IsNullOrEmpty(existing) && GlobalObjectId.TryParse(existing, out _))
                            id = existing;
                    }
                    f.SetValue(target, id);
                }
                else if (srcIsStr && tgtIsObj)
                {
                    string id = srcValue as string;
                    UnityEngine.Object obj = null;
                    if (!string.IsNullOrEmpty(id) && GlobalObjectId.TryParse(id, out var gid))
                    {
                        obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                        if (obj != null && !f.FieldType.IsAssignableFrom(obj.GetType()))
                            obj = null;
                    }
                    f.SetValue(target, obj);
                }
            }
            t = t.BaseType;
        }
    }

    /// <summary>
    /// 处理 List/Array 的元素级映射
    /// </summary>
    private static bool TryMapList(IList srcList, IList tgtList, Type srcFieldType, Type tgtFieldType)
    {
        Type srcElem = GetListElementType(srcFieldType);
        Type tgtElem = GetListElementType(tgtFieldType);

        if (srcElem == null || tgtElem == null)
            return false;

        // List<Object> -> List<string>  (写入配置：场景对象转 GlobalObjectId)
        if (typeof(UnityEngine.Object).IsAssignableFrom(srcElem) && tgtElem == typeof(string))
        {
            // 先备份目标列表的旧值，防止 null 覆盖有效 GUID
            var oldValues = new List<string>();
            foreach (var item in tgtList)
                oldValues.Add(item as string);

            tgtList.Clear();

            for (int i = 0; i < srcList.Count; i++)
            {
                var obj = srcList[i] as UnityEngine.Object;
                string gidStr = "";
                if (obj != null)
                {
                    var gid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                    gidStr = gid.ToString();
                }
                else if (i < oldValues.Count && !string.IsNullOrEmpty(oldValues[i]))
                {
                    // 防御性保留：源为null，但旧值是有效的GlobalObjectId，不覆盖
                    gidStr = oldValues[i];
                }
                tgtList.Add(gidStr);
            }
            return true;
        }

        // List<string> -> List<Object>  (读取配置：GlobalObjectId 还原场景对象)
        if (srcElem == typeof(string) && typeof(UnityEngine.Object).IsAssignableFrom(tgtElem))
        {
            tgtList.Clear();

            foreach (var item in srcList)
            {
                string gidStr = item as string;
                UnityEngine.Object obj = null;
                if (!string.IsNullOrEmpty(gidStr) && GlobalObjectId.TryParse(gidStr, out var gid))
                {
                    obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                    // 类型安全检查：防止存的是 GameObject，读的目标却是 Collider
                    if (obj != null && !tgtElem.IsAssignableFrom(obj.GetType()))
                        obj = null;
                }
                tgtList.Add(obj);
            }
            return true;
        }

        return false;
    }

    private static Type GetListElementType(Type type)
    {
        if (type.IsArray)
            return type.GetElementType();

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            if (def == typeof(List<>) || def == typeof(IList<>) || def == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];
        }
        return null;
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