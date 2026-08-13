using UnityEditor;
using UnityEngine;

public class ExCore : EditorWindow
{
    [MenuItem("Tools/ExCore", false, 0)]
    public static void ShowWindow()
    {
        GetWindow<ExCore>();
    }
    private void OnEnable()
    {
        minSize = new Vector2(600, 300);
    }
    private void OnGUI()
    {
        if (GUILayout.Button("测试"))
        {
            Debug.Log("测试");
        }
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
    public virtual void DrowTopItem(string title, string parent)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
        if (parent != string.Empty)
        {
            if (GUILayout.Button($"返回 {parent}"))
            {

            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.Label($"{title}");
        EditorGUILayout.EndHorizontal();
    }
}
public class MenuPage : EditorPage
{
    public override string Title => "主界面";
    public override string ParentPage => null;

    public override void OnGuI()
    {
        base.DrowTopItem(Title, ParentPage);
    }

}
