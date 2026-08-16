using UnityEngine;
/// <summary>
/// 主菜单类
/// </summary>
public class MenuPage : EditorPage
{
    public override string Title => "主菜单";
    public override string ParentPage => null;
    public override void OnGUI()
    {
        GUILayout.BeginHorizontal();
        DrawButtonsOnTop<MenuPage_Main>(this is MenuPage_Main ? "<主要功能>" : "主要功能");
        DrawButtonsOnTop<MenuPage_Setting>(this is MenuPage_Setting ? "<通用设置>" : "通用设置");
        GUI.contentColor = Color.yellow;
        if (GUILayout.Button("说明文档", GUILayout.Height(22)))
        {
            Application.OpenURL("https://ecn1466ik8jj.feishu.cn/wiki/K604wEQpMimrJLkXtAOcthUaneg");
        }
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.black;

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        GUI.backgroundColor = Color.white;
    }
    private void DrawButtonsOnTop<T>(string name) where T : EditorPage, new()
    {
        if (GUILayout.Button(name, GUILayout.Height(22)))
        {
            if (NavigationCore.PageStack.Count <= 1)
            {
                NavigationCore.Push<T>();
            }
            else
            {
                NavigationCore.Replace<T>();
            }
        }
    }
}
