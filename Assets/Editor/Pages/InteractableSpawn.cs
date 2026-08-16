using UnityEngine;
/// <summary>
/// 交互逻辑生成控制器
/// </summary>
public class InteractableSpawn : MainFunction
{
    public override string Title => "交互逻辑生成控制器";
    public override string ParentPage => "主要功能";
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);


        base.DrawBottomItem(Title);
    }
}