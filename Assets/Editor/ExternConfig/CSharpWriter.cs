using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class CSharpWriter
{
    /// <summary>
    /// 需要挂载到对象上的脚本映射字典
    /// </summary>
    private static Dictionary<string, string> Dict = new Dictionary<string, string>();
    static string key = "Loaded";
    static string value = string.Empty;
    /// <summary>
    /// 将文本文件(包含可替换字段)转化为CS脚本
    /// </summary>
    /// <param name="name">脚本名称</param>
    /// <param name="replacements">需要替换的参数</param>
    /// <param name="target">生成后脚本挂载的物体</param>
    /// <returns></returns>
    public static string Write(string name, Dictionary<string, string> replacements, List<GameObject> targets = null)
    {
        string input = Path.Combine(Application.dataPath, "Resources", "Config", name + ".txt");
        if (!File.Exists(input))
            return $"配置文件 {name} 不存在";
        string content = File.ReadAllText(input);
        if (replacements != null)
        {
            foreach (var kv in replacements)
            {
                string tag = $"<rep>{kv.Key}</rep>";
                content = content.Replace(tag, kv.Value);
            }
        }
        Directory.CreateDirectory($"{Application.dataPath}/Generate");
        string output = $"{Application.dataPath}/Generate/{name}.cs";
        File.WriteAllText(output, content);

        if (targets != null)
        {
            if (targets.Count != 0)
            {
                Dict.Clear();
                value = string.Empty;
                for (int i = 0; i < targets.Count; i++)
                {
                    if (!Dict.ContainsKey(targets[i].name))
                        Dict.Add(targets[i].name, name);
                    value += targets[i].name + ",";
                }
                value += name;
                SessionState.SetString(key, value);
            }
        }
        AssetDatabase.Refresh();
        OnScriptLoadFinished();
        return "生成成功";
    }
    /// <summary>
    /// 将文本文件(不包含可替换字段)转化为CS脚本
    /// </summary>
    /// <param name="name">脚本名称</param>
    /// <param name="replacements">需要替换的参数</param>
    /// <param name="target">生成后脚本挂载的物体</param>
    /// <returns></returns>
    public static string Write(string name)
    {
        string input = Path.Combine(Application.dataPath, "Resources", "Config", name + ".txt");
        if (!File.Exists(input))
            return $"配置文件 {name} 文件不存在";
        string content = File.ReadAllText(input);
        Directory.CreateDirectory($"{Application.dataPath}/Generate");
        string output = $"{Application.dataPath}/Generate/{name}.cs";
        File.WriteAllText(output, content);
        AssetDatabase.Refresh();
        OnScriptLoadFinished();
        return "生成成功";
    }
    /// <summary>
    /// 脚本编译完成后触发，用于自动给场景物体添加脚本
    /// </summary>
    [DidReloadScripts]
    static void OnScriptLoadFinished()
    {
        string[] values = SessionState.GetString(key, string.Empty).Split(',');
        if (values.Length - 1 == 0) return;
        Debug.Log($"成功生成了 {values[values.Length - 1]} 路径{Application.dataPath}/Generate/{values[values.Length - 1]}.cs");
        Debug.Log($"脚本编译完成，待添加脚本数量：{values.Length - 1}");
        for (int i = 0; i < values.Length - 1; i++)
        {
            Type temp = FindMonoBehaviour(values[values.Length - 1]);
            GameObject item = GameObject.Find(values[i]);
            if (temp == null) continue;
            if (item.GetComponent(temp) == null)
            {
                item.AddComponent(temp);
                Debug.Log($"{item}添加{temp.Name}成功");
            }
        }
        SessionState.EraseString(key);
    }
    /// <summary>
    /// 根据类名查找 MonoBehaviour 类型
    /// </summary>
    private static Type FindMonoBehaviour(string className)
    {
        // 常见程序集顺序查找
        string[] assemblies = new[]
        {
            "Assembly-CSharp",           // 普通脚本
            "Assembly-CSharp-firstpass", // Plugins/Standard Assets
            "Assembly-CSharp-Editor",    // Editor 文件夹
        };

        foreach (var asm in assemblies)
        {
            Type t = Type.GetType($"{className}, {asm}");
            if (t != null && t.IsSubclassOf(typeof(MonoBehaviour)))
                return t;
        }
        // 兜底：遍历所有程序集
        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            Type t = asm.GetType(className);
            if (t != null && t.IsSubclassOf(typeof(MonoBehaviour)))
                return t;
        }
        return null;
    }
}