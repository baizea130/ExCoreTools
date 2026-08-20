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
    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.Label("通用设置", GetTitleStyle());

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
        if (GUILayout.Button("测试"))
        {
            MethodExtensions.Execute("123");
        }
        base.DrawBottomItem(Title);
    }
    private static bool HasDOTweenInAssets()
    {
        var guids = AssetDatabase.FindAssets("DOTween", new[] { "Assets" });
        return guids.Length > 0;
    }
}
