using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 交互逻辑生成控制器
/// </summary>
public class InteractableSpawn : MainFunction
{
    public override string Title => "交互逻辑生成控制器";
    public override string ParentPage => "主要功能";
    /// <summary>
    /// 点击后展示纯文字的碰撞体
    /// </summary>
    public List<Collider> CollidersByOnlyText = new List<Collider>();
    public List<string> StringsByOnlyText = new List<string>();
    public List<Collider> CollidersByImg = new List<Collider>();
    public List<string> StringsByImg = new List<string>();
    public List<Collider> CollidersByModel = new List<Collider>();
    public List<string> StringsByModel = new List<string>();
    private Vector2 scrollPos;
    public override void OnEnter(object data)
    {
        ReadConfig();
    }
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        GUILayout.Label("在生成UI预设界面勾选对应展示模式后此处会自动更新");
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        base.DrawBackBtn(ParentPage);
        if (MethodExtensions.CreateSO<ToolConfig>().ShowDetailByOnlyText)
            DrawObjList("点击后显示 纯文字 的物体", CollidersByOnlyText, StringsByOnlyText);
        if (MethodExtensions.CreateSO<ToolConfig>().ShowDetailByImg)
            DrawObjList("点击后显示 文字+图片 的物体", CollidersByImg, StringsByImg);
        if (MethodExtensions.CreateSO<ToolConfig>().ShowDetailByModel)
            DrawObjList("点击后显示 文字+模型 的物体", CollidersByModel, StringsByModel);
        GUILayout.EndScrollView();
        if (GUILayout.Button("保存配置  (离开此界面自动保存)"))
        {
            WriteConfig();
        }
        base.DrawBottomItem(Title);
    }
    private void DrawObjList(string content, List<Collider> colliders, List<string> texts)
    {
        GUILayout.Label(content, GetSubheadingStyle());
        for (int i = 0; i < Mathf.Min(colliders.Count, texts.Count); i++)
        {
            colliders[i] = EditorGUILayout.ObjectField("碰撞体", colliders[i], typeof(Collider), true) as Collider;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("描述");
            texts[i] = EditorGUILayout.TextField(texts[i]);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("删除此项"))
            {
                colliders.RemoveAt(i);
                texts.RemoveAt(i);
            }
            GUILayout.Space(15);
        }
        if (GUILayout.Button("+", GetListBtnStyle(), GUILayout.Width(30)))
        {
            colliders.Add(null);
            texts.Add(null);
        }
    }
    private GUIStyle GetSubheadingStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 16;
        return res;
    }
    /// <summary>
    /// 数组加减按钮的风格
    /// </summary>
    /// <returns></returns>
    private GUIStyle GetListBtnStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 30;
        res.alignment = TextAnchor.MiddleCenter;
        return res;
    }
    public override void OnExit()
    {
        WriteConfig();
    }
}