using UnityEngine;

public class ToolConfig : ScriptableObject
{
    [Header("UI预设配置")]
    [ReadOnly] public int ResolutionX;
    [ReadOnly] public int ResolutionY;
    [ReadOnly] public bool ShowDetailByOnlyText, ShowDetailByImg, ShowDetailByModel;
    [ReadOnly] public bool StartPanel, SettingPanel, QASystemPanel;
    [ReadOnly] public string ShowDetailByOnlyTextTag, ShowDetailByImgTag, ShowDetailByModelTag;
    [ReadOnly] public Sprite DetailPanelBG, DetailPanelBackBtn;
    [Header("玩家数值配置")]
    [ReadOnly] public bool HidePlayerLayout;
    [ReadOnly] public float Speed;
    [ReadOnly] public float Height;
    [ReadOnly] public float Sensitivity;
    [ReadOnly] public float Jump;
}
