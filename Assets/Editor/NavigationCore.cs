using System.Collections.Generic;
using System.Linq;
using UnityEditor;
/// <summary>
/// 界面导航类
/// </summary>
public static class NavigationCore
{
    /// <summary>
    /// 导航界面堆栈
    /// </summary>
    public static Stack<EditorPage> PageStack { get; private set; }  = new Stack<EditorPage>();
    public static EditorWindow ToolWindow { get; private set; }
    public static EditorPage Current => PageStack.Count > 0 ? PageStack.Peek() : null;
    public static void Init(EditorWindow window, EditorPage root)
    {
        ToolWindow = window;

        PageStack.Clear();
        PageStack.Push(root);
        root.OnEnter(null);
    }
    /// <summary>
    /// 进入新界面（压栈）
    /// </summary>
    /// <typeparam name="T">界面类，要求继承EditorPage且有无参构造函数</typeparam>
    /// <param name="param">进入界面的信息参数</param>
    public static void Push<T>(object param = null) where T : EditorPage, new()
    {
        Current?.OnExit();
        var page = new T();
        PageStack.Push(page);
        page.OnEnter(param);
        ToolWindow?.Repaint();
    }
    /// <summary>
    /// 返回上个界面（出栈）
    /// </summary>
    public static void Pop()
    {
        if (PageStack.Count <= 1) return;
        Current?.OnExit();
        PageStack.Pop();
        Current?.OnEnter(null);
        ToolWindow?.Repaint();
    }
    /// <summary>
    /// 替换当前界面
    /// </summary>
    /// <typeparam name="T">界面类，要求继承EditorPage且有无参构造函数</typeparam>
    /// <param name="param">进入替换后界面的信息参数</param>
    public static void Replace<T>(object param = null) where T : EditorPage, new()
    {
        Current?.OnExit();
        PageStack.Pop();
        var page = new T();
        PageStack.Push(page);
        page.OnEnter(param);
        ToolWindow?.Repaint();
    }
    /// <summary>
    /// 获取导航栏
    /// </summary>
    /// <returns>导航字符串</returns>
    public static string GetPageNavData()
    {
        return string.Join("->", PageStack.Reverse().Select(m => m.Title));
    }
}