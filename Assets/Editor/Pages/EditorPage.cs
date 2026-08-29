using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 面板基类
/// </summary>
public abstract class EditorPage
{
    public virtual string Title => GetType().Name;
    public virtual string ParentPage => null;
    public virtual void OnEnter(object data) { }
    public abstract void OnGUI();
    public virtual void OnExit() { }
    /// <summary>
    /// 绘制返回按钮
    /// </summary>
    /// <param name="parent">返回目标的标题</param>
    public virtual void DrawBackBtn(string parent)
    {
        if (!string.IsNullOrEmpty(parent))
        {
            if (GUILayout.Button($"返回 -> {parent}", GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                NavigationCore.Pop();
            }
        }
    }
    /// <summary>
    /// 绘制底部导航栏
    /// </summary>
    /// <param name="title"></param>
    public virtual void DrawBottomItem(string title)
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
        if (NavigationCore.Current != null)
        {
            var breadcrumb = NavigationCore.GetPageNavData();
            GUILayout.Label(breadcrumb);
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndHorizontal();
    }
    protected GUIStyle GetTitleStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 24;
        res.alignment = TextAnchor.MiddleCenter;
        return res;
    }
    /// <summary>
    /// 生成Float输入框
    /// </summary>
    /// <param name="description">文本描述</param>
    /// <param name="param">外部参数</param>
    /// <param name="bound">数值边界</param>
    /// <param name="options">风格</param>
    /// <returns></returns>
    protected float SpawnFloatField(string description, float param, Vector2 bound, GUILayoutOption[] options = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{description} ({bound.x}~{bound.y}) :", GUILayout.Width(NavigationCore.ToolWindow.position.width / 3));
        param = EditorGUILayout.FloatField(param, options);
        param = Mathf.Clamp(param, bound.x, bound.y);
        GUILayout.EndHorizontal();
        return param;
    }
    /// <summary>
    /// 生成Int输入框
    /// </summary>
    /// <param name="description">文本描述</param>
    /// <param name="param">外部参数</param>
    /// <param name="bound">数值边界</param>
    /// <param name="options">风格</param>
    /// <returns></returns>
    protected int SpawnIntField(string description, int param, Vector2 bound, GUILayoutOption[] options = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{description} ({bound.x}~{bound.y}) :", GUILayout.Width(NavigationCore.ToolWindow.position.width / 3));
        param = EditorGUILayout.IntField(param, options);
        param = (int)Mathf.Clamp(param, bound.x, bound.y);
        GUILayout.EndHorizontal();
        return param;
    }
    /// <summary>
    /// 在重大操作前生成的保护性弹窗，Action为空时为单纯的提示弹窗
    /// </summary>
    /// <param name="content">内容文本</param>
    /// <param name="execute">点击确认后触发的功能</param>
    protected void ProtectDialog(string content, Action execute = null)
    {
        bool ok;
        if (execute == null)
        {
            ok = EditorUtility.DisplayDialog(
            "提示",        // 标题
            content,          // 内容
            "OK我收到"          // 确定按钮文字
            );
            return;
        }
        ok = EditorUtility.DisplayDialog(
            "提示",        // 标题
            content,          // 内容
            "OK我收到",           // 确定按钮文字
            "我再想想"            // 取消按钮文字
            );
        if (ok)
        {
            execute();
        }
    }
    /// <summary>
    /// 检查输入标签的输入框是否为空
    /// </summary>
    /// <param name="content">输入框中的字符串</param>
    /// <param name="fullTip">填了字段后显示的提示</param>
    /// <returns>是否为空</returns>
    protected bool CheckFieldEmpty(object content, string fullTip = null)
    {
        bool empty = content == null
            || (content is string s && string.IsNullOrWhiteSpace(s))
            || (content is UnityEngine.Object o && o == null);

        if (empty)
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("*输入的内容为空");
            GUI.contentColor = Color.white;
            return true;
        }

        if (!string.IsNullOrEmpty(fullTip))
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label(fullTip);
            GUI.contentColor = Color.white;
        }
        return false;
    }
    /// <summary>
    /// 判断导入的资源是否在Resources文件夹内并自动填充Res尾部路径
    /// </summary>
    /// <param name="assetObject">资源</param>
    /// <returns></returns>
    protected string GetAssetPath(UnityEngine.Object assetObject)
    {
        if (assetObject == null)
        {
            return null;
        }
        string fullPath = AssetDatabase.GetAssetPath(assetObject);
        if (string.IsNullOrEmpty(fullPath))
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("*资源未保存在项目Assets目录中");
            GUI.contentColor = Color.white;
            return null;
        }
        string resMarker = "/Resources/";
        int resIndex = fullPath.IndexOf(resMarker, System.StringComparison.OrdinalIgnoreCase);
        if (resIndex >= 0)
        {
            string resPath = fullPath.Substring(resIndex + resMarker.Length);
            string ext = System.IO.Path.GetExtension(resPath);
            if (!string.IsNullOrEmpty(ext))
            {
                resPath = resPath.Substring(0, resPath.Length - ext.Length);
            }

            GUI.contentColor = Color.green;
            GUILayout.Label($"Resources路径：{resPath}");
            GUI.contentColor = Color.white;

            return resPath;// 返回可直接用于 Resources.Load<T>() 的路径
        }
        else
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("*资源不在Resources文件夹中");
            GUI.contentColor = Color.white;

            return fullPath;
        }
    }
    /// <summary>
    /// 添加标签
    /// </summary>
    /// <param name="tagName"></param>
    protected void AddTag(string tagName)
    {
        if (string.IsNullOrEmpty(tagName)) return;
        if (UnityEditorInternal.InternalEditorUtility.tags.Contains(tagName))
        {
            return;
        }
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
        );
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
        newTag.stringValue = tagName;

        tagManager.ApplyModifiedProperties();
        tagManager.Update();
    }
    protected void ReadConfig()
    {
        ToolConfig config = MethodExtensions.GetOrCreateSO<ToolConfig>(true);
        MethodExtensions.AutoMap(this, config);
    }
    protected void WriteConfig()
    {
        ToolConfig config = MethodExtensions.GetOrCreateSO<ToolConfig>(true);
        MethodExtensions.AutoMap(config, this);
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
