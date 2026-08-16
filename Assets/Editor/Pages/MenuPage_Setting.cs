using UnityEngine;
/// <summary>
/// 设置界面类
/// </summary>
public class MenuPage_Setting : MenuPage
{
    public override string Title => "通用设置";
    public override string ParentPage => "主菜单";
    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.Label("通用设置", GetTitleStyle());
        base.DrawBottomItem(Title);
    }
}
