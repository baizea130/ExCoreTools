using System;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 设置界面类
/// </summary>
public class MenuPage_Setting : MenuPage
{
    public override string Title => "通用设置";
    public override string ParentPage => "主菜单";
    private bool useDOtween = false;
    private Vector2 scrollPosition;
    public override void OnEnter(object data)
    {
        ReadConfig();
    }
    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.Label("通用设置", GetTitleStyle());

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        GUILayout.Label("普通", GetSubheadingStyle());
        GUILayout.Space(60);
        GUILayout.Label("脚本相关（调整后需要到对应脚本生成界面重新生成，建议一开始就调好）", GetSubheadingStyle());
        GUIStyle bigToggle = new GUIStyle(EditorStyles.toggle);
        bigToggle.fontSize = 15;
        bigToggle.fixedHeight = 35;  // 行高

        useDOtween = GUILayout.Toggle(useDOtween, "使用DG.Tween优化UI动画", bigToggle);
        if (HasDOTweenInAssets() == false && useDOtween)
        {
            GUI.contentColor = Color.red;
            GUILayout.Label("*未检测到DG.Tween相关文件，请自行安装或取消勾选");
            GUI.contentColor = Color.white;
        }
        GUILayout.Space(60);
        if (GUILayout.Button("测试"))
        {
            MethodExtensions.Execute("123");
        }
        GUILayout.EndScrollView();
        GUI.contentColor = Color.yellow;
        GUILayout.Label("离开此界面会自动保存配置");
        GUI.contentColor = Color.white;
        base.DrawBottomItem(Title);
    }
    private GUIStyle GetSubheadingStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 18;
        return res;
    }
    private static bool HasDOTweenInAssets()
    {
        var guids = AssetDatabase.FindAssets("DOTween", new[] { "Assets" });
        return guids.Length > 0;
    }
    public override void OnExit()
    {
        WriteConfig();
    }

}
