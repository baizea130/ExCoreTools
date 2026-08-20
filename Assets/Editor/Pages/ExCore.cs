using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExCore : EditorWindow
{

    [MenuItem("Tools/生成局内框架脚本", false, 0)]
    public static void InitBaseObj()
    {
        CSharpWriter.Write("EventCenter");
        CSharpWriter.Write("Singleton");
        CSharpWriter.Write("GameEvents");
    }
    [MenuItem("Tools/应用场景框架配置", false, 20)]
    public static void InitSceneObj()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            gameManager = new GameObject("GameManager");
        }
        List<GameObject> target = new List<GameObject> { gameManager };

        CSharpWriter.Write("GameManager", GetReplaceDict(), target);
    }
    /// <summary>
    /// 获取在InitSceneObj中替换的字符串字典
    /// </summary>
    /// <returns></returns>
    private static HashSet<string> GetReplaceDict()
    {
        HashSet<string> replacement = new HashSet<string>();
        if (MethodExtensions.CreateSO<ToolConfig>().StartPanel == true)
        {
            replacement.Add("GameManager_StartPanel");
        }
        return replacement;
    }
    [MenuItem("Tools/ExCore", false, 99)]
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
