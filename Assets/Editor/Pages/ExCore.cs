using UnityEditor;
using UnityEngine;

public class ExCore : EditorWindow
{
    [MenuItem("Tools/生成配置文件", false, 0)]
    public static void InitConfig()
    {
        CSharpWriter.Write("MethodExtensions");
    }
    [MenuItem("Tools/ExCore", false, 1)]
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
        NavigationCore.Current?.OnGUI();
    }
}
