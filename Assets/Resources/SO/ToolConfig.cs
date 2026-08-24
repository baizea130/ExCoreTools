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
    public string StartPanelBGPath, StartPanelBtnPath,DetailPanelBGPath,DetailPanelBackBtnPath;
    [Header("玩家数值配置")]
    public bool HidePlayerLayout;
    public float Speed;
    public float Height;
    public float Sensitivity;
    public float Jump;
    [Header("通用设置")]
    public bool useDOtween;
}
