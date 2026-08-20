using UnityEngine;
/// <summary>
/// 主要功能类
/// </summary>
public class MenuPage_Main : MenuPage
{
    public override string Title => "主要功能";
    public override string ParentPage => "主菜单";
    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.Label(Title, GetTitleStyle());
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            DrawButton<PlayerTempSpawn>("生成玩家预设   (场景物体)");
            DrawButton<FirstPersonSpawn>("第一人称控制器    (脚本)");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawButton<UISystemSpawn>("生成UI预设   (场景物体+脚本)");
            DrawButton<InteractableSpawn>("交互逻辑生成控制器   (脚本)");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        base.DrawBottomItem(Title);
    }
    private void DrawButton<T>(string name) where T : EditorPage, new()
    {
        if (GUILayout.Button(name, GUILayout.Height(30), GUILayout.Width(NavigationCore.ToolWindow.position.width / 2 - 5)))
        {
            NavigationCore.Push<T>();
        }
    }
}
