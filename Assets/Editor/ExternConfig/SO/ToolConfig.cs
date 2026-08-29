using System.Collections.Generic;
using UnityEngine;

public class ToolConfig : ScriptableObject
{
    [Header("UI预设配置")]
    [ReadOnly] public int ResolutionX;
    [ReadOnly] public int ResolutionY;
    [ReadOnly] public bool ShowDetailByOnlyText, ShowDetailByImg, ShowDetailByModel;
    [ReadOnly] public bool StartPanel, SettingPanel, QASystemPanel;
    [ReadOnly] public Sprite DetailPanelBG, DetailPanelBackBtn;
    [ReadOnly] public Sprite StartPanelBG, StartPanelBtn;
    [ReadOnly] public string StartPanelBGPath, StartPanelBtnPath, DetailPanelBGPath, DetailPanelBackBtnPath;
    [Header("玩家数值配置")]
    [ReadOnly] public bool HidePlayerLayout;
    [ReadOnly] public float Speed;
    [ReadOnly] public float Height;
    [ReadOnly] public float Sensitivity;
    [ReadOnly] public float Jump;
    [Header("交互系统控制")]
    [ReadOnly] public List<string> Titles = new List<string>();
    [ReadOnly] public List<Collider> CollidersByOnlyText = new List<Collider>();
    [ReadOnly] public List<string> StringsByOnlyText = new List<string>();

    [ReadOnly] public List<Collider> CollidersByImg = new List<Collider>();
    [ReadOnly] public List<string> StringsByImg = new List<string>();
    [ReadOnly] public List<Sprite> SpritesByImg = new List<Sprite>();
    [ReadOnly] public List<string> SpritesPathByImg = new List<string>();

    [ReadOnly] public List<Collider> CollidersByModel = new List<Collider>();
    [ReadOnly] public List<string> StringsByModel = new List<string>();
    [ReadOnly] public List<GameObject> ModelsByModel = new List<GameObject>();
    [ReadOnly] public List<GameObject> PivotsByModel = new List<GameObject>();
    [Header("通用设置")]
    [ReadOnly] public bool useDOtween;
}
