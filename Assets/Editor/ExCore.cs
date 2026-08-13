using UnityEditor;
using UnityEngine;

public class ExCore : EditorWindow
{
    [MenuItem("Tools/ExCore", false, 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<ExCore>();
        NavigationCore.Init(window, new MenuPage());
    }
    private void OnEnable()
    {
        minSize = new Vector2(600, 300);
        maxSize = new Vector2(2160, 1980);
    }
    private void OnGUI()
    {
        NavigationCore.Current?.OnGuI();
    }
    private void OnDisable()
    {

    }
}
/// <summary>
/// 面板基类
/// </summary>
public abstract class EditorPage
{
    public virtual string Title => GetType().Name;
    public virtual string ParentPage => null;
    public virtual void OnEnter(object data) { }
    public abstract void OnGuI();
    public virtual void OnExit() { }
    public virtual void DrowBackBtn(string parent)
    {
        if (!string.IsNullOrEmpty(parent))
        {
            if (GUILayout.Button($"返回 -> {parent}", GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                NavigationCore.Pop();
            }
        }
    }
    public virtual void DrowBottomItem(string title)
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
        if (NavigationCore.Current != null)
        {
            var breadcrumb = NavigationCore.GetPageNavData();
            GUILayout.Box(breadcrumb);
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
    protected float SpawnFloatField(string description, float param, Vector2 bound, GUILayoutOption[] options = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{description} ({bound.x}~{bound.y}) :", GUILayout.Width(NavigationCore.ToolWindow.position.width / 3));
        param = EditorGUILayout.FloatField(param, options);
        param = Mathf.Clamp(param, bound.x, bound.y);
        GUILayout.EndHorizontal();
        return param;
    }
}
/// <summary>
/// 主菜单类
/// </summary>
public class MenuPage : EditorPage
{
    public override string Title => "主菜单";
    public override string ParentPage => null;
    public override void OnGuI()
    {
        GUILayout.BeginHorizontal();
        DrawButtonsOnTop<MenuPage_Main>(this is MenuPage_Main ? "<主要功能>" : "主要功能");
        DrawButtonsOnTop<MenuPage_Setting>(this is MenuPage_Setting ? "<通用设置>" : "通用设置");
        GUI.contentColor = Color.yellow;
        if (GUILayout.Button("说明文档", GUILayout.Height(22)))
        {
            Application.OpenURL("https://ecn1466ik8jj.feishu.cn/wiki/K604wEQpMimrJLkXtAOcthUaneg");
        }
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();
        var prevColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        GUI.backgroundColor = prevColor;
    }
    private void DrawButtonsOnTop<T>(string name) where T : EditorPage, new()
    {
        if (GUILayout.Button(name, GUILayout.Height(22)))
        {
            if (NavigationCore.PageStack.Count <= 1)
            {
                NavigationCore.Push<T>();
            }
            else
            {
                NavigationCore.Replace<T>();
            }
        }
    }
}
#region MenuPage_*
/// <summary>
/// 设置界面类
/// </summary>
public class MenuPage_Setting : MenuPage
{
    public override string Title => "通用设置";
    public override string ParentPage => "主菜单";
    public override void OnGuI()
    {
        base.OnGuI();
        GUILayout.Label("通用设置", GetTitleStyle());
        base.DrowBottomItem(Title);
    }
}
/// <summary>
/// 主要功能类
/// </summary>
public class MenuPage_Main : MenuPage
{
    public override string Title => "主要功能";
    public override string ParentPage => "主菜单";
    public override void OnGuI()
    {
        base.OnGuI();
        GUILayout.Label("主要功能", GetTitleStyle());
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            DrawButton<FirstPersonSpawn>("第一人称控制器");
            DrawButton<InteractableSpawn>("交互逻辑生成控制器");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        base.DrowBottomItem(Title);
    }
    private void DrawButton<T>(string name) where T : EditorPage, new()
    {
        if (GUILayout.Button(name, GUILayout.Height(30)))
        {
            NavigationCore.Push<T>();
        }
    }
}
#endregion
/// <summary>
/// 主要功能界面下所有界面的基类
/// </summary>
public class MainFunction : EditorPage
{
    public override void OnGuI() { }
}
/// <summary>
/// 第一人称控制器界面
/// </summary>
public class FirstPersonSpawn : MainFunction
{
    public override string Title => "第一人称控制器";
    public override string ParentPage => "主要功能";
    private PlayerData data = new PlayerData();
    public override void OnGuI()
    {
        GUILayout.Label("第一人称控制器", GetTitleStyle());
        base.DrowBackBtn(ParentPage);

        data.mSpeed = SpawnFloatField("移动速度", data.mSpeed, new Vector2(0, 999));
        data.mHeight = SpawnFloatField("模型身高", data.mHeight, new Vector2(0, 999));
        data.mSensitivity = SpawnFloatField("视角灵敏度", data.mSensitivity, new Vector2(1, 9999));
        if (GUILayout.Button("确认生成"))
        {
            Debug.Log($"{data.mSpeed} + {data.mHeight} + {data.mSensitivity}");
        }

        base.DrowBottomItem(Title);
    }
    private class PlayerData
    {
        public float mSpeed;
        public float mHeight;
        public float mSensitivity;
    }
}
/// <summary>
/// 交互逻辑生成控制器
/// </summary>
public class InteractableSpawn : MainFunction
{
    public override string Title => "交互逻辑生成控制器";
    public override string ParentPage => "主要功能";
    public override void OnGuI()
    {
        GUILayout.Label("交互逻辑生成控制器", GetTitleStyle());
        base.DrowBackBtn(ParentPage);


        base.DrowBottomItem(Title);
    }
}