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
            if (GUILayout.Button($"返回 {parent}", GUILayout.Height(35), GUILayout.ExpandWidth(false)))
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
        GUILayout.EndHorizontal();
    }
    private void DrawButtonsOnTop<T>(string name) where T : EditorPage, new()
    {
        if (GUILayout.Button(name, GUILayout.Height(30)))
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
        GUILayout.Label("通用设置");
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
        GUILayout.Label("主要功能");
        base.DrowBottomItem(Title);
    }
}
