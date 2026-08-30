using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class CSharpWriter
{
    private static Dictionary<string, string> Dict = new Dictionary<string, string>();
    static string key = "Loaded";
    static string value = string.Empty;

    private static string ReplacementPath => Path.Combine(Application.dataPath, "Editor", "ExternConfig", "Replacement.txt");

    #region 三个公开重载

    /// <summary>
    /// 字典模式：&lt;rep&gt;Key&lt;/rep&gt; 直接替换为 Dictionary 中的 Value
    /// </summary>
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
        content = Regex.Replace(content, @"<rep>.*?</rep>", string.Empty);
        return WriteCore(content, name, targets);
    }

    /// <summary>
    /// 纯复制模式：无替换，直接输出
    /// </summary>
    public static string Write(string name, List<GameObject> targets = null)
    {
        string input = Path.Combine(Application.dataPath, "Resources", "Config", name + ".txt");
        if (!File.Exists(input))
            return $"配置文件 {name} 文件不存在";

        string content = File.ReadAllText(input);
        content = Regex.Replace(content, @"<rep>.*?</rep>", string.Empty);
        return WriteCore(content, name, targets);
    }

    /// <summary>
    /// 代码块模式：HashSet 中标记的 Key 去 Replacement.txt 查代码块替换；
    /// 未标记的 Key 替换为空字符串（条件删除）。
    /// </summary>
    public static string Write(string name, HashSet<string> replacements, List<GameObject> targets = null)
    {
        string input = Path.Combine(Application.dataPath, "Resources", "Config", name + ".txt");
        if (!File.Exists(input))
            return $"配置文件 {name} 不存在";

        string content = File.ReadAllText(input);

        // 按需解析 Replacement.txt
        Dictionary<string, string> codeBlockDict = null;
        if (replacements != null && replacements.Count > 0)
            codeBlockDict = ParseReplacementFile();

        // 忽略大小写的激活 Key 集合
        var activeKeys = replacements != null
            ? new HashSet<string>(replacements, StringComparer.OrdinalIgnoreCase)
            : null;

        // 扫描模板中所有 <rep>key</rep>，每个匹配都执行替换
        var matches = Regex.Matches(content, @"<rep>(.*?)</rep>");

        foreach (Match match in matches)
        {
            string key = match.Groups[1].Value;
            string tag = $"<rep>{key}</rep>";

            if (activeKeys != null && activeKeys.Contains(key))
            {
                // 条件满足：去 Replacement.txt 查代码块
                if (codeBlockDict != null && codeBlockDict.TryGetValue(key, out string block))
                {
                    content = content.Replace(tag, block);
                }
                else
                {
                    Debug.LogWarning($"[CSharpWriter] Replacement.txt 中未找到代码块 [{key}]，替换为空");
                    content = content.Replace(tag, string.Empty);
                }
            }
            else
            {
                // 条件不满足：替换为空字符串，实现代码块删除
                content = content.Replace(tag, string.Empty);
            }
        }
        content = Regex.Replace(content, @"<rep>.*?</rep>", string.Empty);
        return WriteCore(content, name, targets);
    }
    /// <summary>
    /// 混合模式：HashSet 中标记的 Key 去 Replacement.txt 查代码块替换；
    /// &lt;rep&gt;Key&lt;/rep&gt; 直接替换为 Dictionary 中的 Value
    /// 未标记的 Key 替换为空字符串（条件删除）。
    /// </summary>
    public static string Write(string name, HashSet<string> hashReplacements, Dictionary<string, string> dictReplacements, List<GameObject> targets = null)
    {
        string input = Path.Combine(Application.dataPath, "Resources", "Config", name + ".txt");
        if (!File.Exists(input))
            return $"配置文件 {name} 不存在";

        string content = File.ReadAllText(input);

        // 按需解析 Replacement.txt
        Dictionary<string, string> codeBlockDict = null;
        if (hashReplacements != null && hashReplacements.Count > 0)
            codeBlockDict = ParseReplacementFile();

        // 忽略大小写的激活 Key 集合
        var activeKeys = hashReplacements != null
            ? new HashSet<string>(hashReplacements, StringComparer.OrdinalIgnoreCase)
            : null;

        // 扫描模板中所有 <rep>key</rep>，每个匹配都执行替换
        var matches = Regex.Matches(content, @"<rep>(.*?)</rep>");

        foreach (Match match in matches)
        {
            string key = match.Groups[1].Value;
            string tag = $"<rep>{key}</rep>";

            if (activeKeys != null && activeKeys.Contains(key))
            {
                // 条件满足：去 Replacement.txt 查代码块
                if (codeBlockDict != null && codeBlockDict.TryGetValue(key, out string block))
                {
                    content = content.Replace(tag, block);
                }
                else
                {
                    Debug.LogWarning($"[CSharpWriter] Replacement.txt 中未找到代码块 [{key}]，替换为空");
                    content = content.Replace(tag, string.Empty);
                }
            }
        }
        if (dictReplacements != null)
        {
            foreach (var kv in dictReplacements)
            {
                string tag = $"<rep>{kv.Key}</rep>";
                content = content.Replace(tag, kv.Value);
            }
        }
        content = Regex.Replace(content, @"<rep>.*?</rep>", string.Empty);
        return WriteCore(content, name, targets);
    }
    #endregion

    #region 私有逻辑

    private static string WriteCore(string content, string name, List<GameObject> targets)
    {
        Directory.CreateDirectory($"{Application.dataPath}/Generate");
        string output = $"{Application.dataPath}/Generate/{name}.cs";
        File.WriteAllText(output, content);

        if (targets != null && targets.Count != 0)
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
        else
        {
            SessionState.SetString(key, name);
        }

        AssetDatabase.Refresh();
        OnScriptLoadFinished();
        return "生成成功";
    }

    private static Dictionary<string, string> ParseReplacementFile()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(ReplacementPath))
        {
            Debug.LogWarning($"[CSharpWriter] Replacement.txt 不存在: {ReplacementPath}");
            return result;
        }

        try
        {
            string text = File.ReadAllText(ReplacementPath);
            var matches = Regex.Matches(text, @"\[\[(.*?)\]\]==>\[\[(.*?)\]\]", RegexOptions.Singleline);
            foreach (Match m in matches)
            {
                if (m.Groups.Count >= 3)
                {
                    string k = m.Groups[1].Value.Trim();
                    string v = m.Groups[2].Value;
                    if (!string.IsNullOrEmpty(k))
                        result[k] = v;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CSharpWriter] 解析 Replacement.txt 失败: {ex.Message}");
        }

        return result;
    }

    #endregion
    #region 脚本加载回调

    [DidReloadScripts]
    static void OnScriptLoadFinished()
    {
        string raw = SessionState.GetString(key, string.Empty);
        if (string.IsNullOrEmpty(raw)) return;

        string[] values = raw.Split(',');
        if (values.Length == 0 || string.IsNullOrEmpty(values[values.Length - 1]))
        {
            SessionState.EraseString(key);
            return;
        }

        Debug.Log($"成功生成了 {values[values.Length - 1]} 路径 {Application.dataPath}/Generate/{values[values.Length - 1]}.cs");
        Debug.Log($"脚本编译完成，待添加脚本数量：{values.Length - 1}");

        // 没有指定目标对象时：去 Resources 找同名预制体，直接给预制体资产加脚本
        if (values.Length == 1)
        {
            Type temp = FindMonoBehaviour(values[0]);
            if (temp == null)
            {
                Debug.LogWarning($"[CSharpWriter] 未找到脚本类型: {values[0]}");
                SessionState.EraseString(key);
                return;
            }

            string prefabPath = FindPrefabInResources(values[0]);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning($"[CSharpWriter] Resources 中未找到预制体: {values[0]}，跳过添加脚本");
                SessionState.EraseString(key);
                return;
            }

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot != null)
            {
                if (prefabRoot.GetComponent(temp) == null)
                {
                    prefabRoot.AddComponent(temp);
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                    Debug.Log($"[CSharpWriter] 给预制体 {values[0]} 添加 {temp.Name} 成功");
                }
                else
                {
                    Debug.Log($"[CSharpWriter] 预制体 {values[0]} 已包含 {temp.Name}，跳过");
                }
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
            SessionState.EraseString(key);
            return;
        }
        for (int i = 0; i < values.Length - 1; i++)
        {
            Type temp = FindMonoBehaviour(values[values.Length - 1]);
            if (temp == null) continue;

            GameObject item = GameObject.Find(values[i]);

            // 场景中没有，去 Resources 找同名预制体实例化
            if (item == null)
            {
                string prefabPath = FindPrefabInResources(values[i]);
                if (!string.IsNullOrEmpty(prefabPath))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab != null)
                    {
                        item = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        item.name = values[i];
                        Debug.Log($"[CSharpWriter] 从 Resources 实例化预制体: {values[i]}");
                    }
                }
            }

            if (item == null)
            {
                Debug.LogWarning($"[CSharpWriter] 未找到对象或预制体: {values[i]}，跳过添加脚本");
                continue;
            }

            if (item.GetComponent(temp) == null)
            {
                item.AddComponent(temp);
                Debug.Log($"{item} 添加 {temp.Name} 成功");
            }
        }

        SessionState.EraseString(key);
    }

    private static Type FindMonoBehaviour(string className)
    {
        string[] assemblies = new[]
        {
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp-Editor",
        };

        foreach (var asm in assemblies)
        {
            Type t = Type.GetType($"{className}, {asm}");
            if (t != null && t.IsSubclassOf(typeof(MonoBehaviour)))
                return t;
        }

        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            Type t = asm.GetType(className);
            if (t != null && t.IsSubclassOf(typeof(MonoBehaviour)))
                return t;
        }

        return null;
    }

    /// <summary>
    /// 在 Assets/Resources 及其子文件夹中查找同名预制体
    /// </summary>
    private static string FindPrefabInResources(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"t:Prefab {name}", new[] { "Assets/Resources" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == name)
                return path;
        }
        return null;
    }

    #endregion
}