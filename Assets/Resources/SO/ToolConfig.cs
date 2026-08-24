using System.Collections.Generic;
using UnityEngine;

public class ToolConfig : ScriptableObject
{
    [Header("UI预设配置")]
    public int ResolutionX;
    public int ResolutionY;
    public bool ShowDetailByOnlyText, ShowDetailByImg, ShowDetailByModel;
    public bool StartPanel, SettingPanel, QASystemPanel;
    public string ShowDetailByOnlyTextTag, ShowDetailByImgTag, ShowDetailByModelTag;
    public Sprite DetailPanelBG, DetailPanelBackBtn;
    public Sprite StartPanelBG, StartPanelBtn;
    public string StartPanelBGPath, StartPanelBtnPath, DetailPanelBGPath, DetailPanelBackBtnPath;
    [Header("玩家数值配置")]
    public bool HidePlayerLayout;
    public float Speed;
    public float Height;
    public float Sensitivity;
    public float Jump;
    [Header("交互系统控制")]
    public List<Collider> CollidersByOnlyText = new List<Collider>();
    public List<string> StringsByOnlyText = new List<string>();
    public List<Collider> CollidersByImg = new List<Collider>();
    public List<string> StringsByImg = new List<string>();
    public List<Collider> CollidersByModel = new List<Collider>();
    public List<string> StringsByModel = new List<string>();
    [Header("通用设置")]
    public bool useDOtween;
}
