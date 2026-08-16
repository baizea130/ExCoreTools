using UnityEditor;
using UnityEngine;
/// <summary>
/// 在场景中生成玩家预设
/// </summary>
public class PlayerTempSpawn : MainFunction
{
    public override string Title => "生成玩家预设";
    public override string ParentPage => "主要功能";
    private int mSelectIndex = -1;
    private string[] mOptions = { "第一人称预设" };
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);

        mSelectIndex = EditorGUILayout.Popup("选择玩家预设", mSelectIndex, mOptions);
        if (GUILayout.Button("生成"))
        {
            switch (mSelectIndex)
            {
                case -1:
                    {
                        ProtectDialog($"未选择合法预设");
                        return;
                    }
                case 0:
                    {
                        GameObject Player = GameObject.FindGameObjectWithTag("Player");
                        if (Player != null || GameObject.Find("Player") != null)
                        {
                            ProtectDialog($"场景中已经有了Player标签或名为Player的物体\n名称：{Player.name}\n请使用此物体作为基准或者删除之"
                            );
                            break;
                        }
                        Player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        InitComponent(Player, mSelectIndex);
                        break;
                    }
                default: break;
            }
        }
        base.DrawBottomItem(Title);
    }
    private void InitComponent(GameObject temp, int select)
    {
        temp.name = "Player";
        temp.tag = "Player";
        var rb = temp.GetOrAddComponent<Rigidbody>();
        var collider = temp.GetOrAddComponent<Collider>();
        if (select == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
