using UnityEditor;
using UnityEngine;
/// <summary>
/// 在场景中生成玩家预设
/// </summary>
public class PlayerTempSpawn : MainFunction
{
    public override string Title => "生成玩家预设";
    public override string ParentPage => "主要功能";
    private int mSelectIndex = 0;
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
                        GameObject byTag = GameObject.FindGameObjectWithTag("Player");
                        GameObject byName = GameObject.Find("Player");
                        GameObject existing = byTag ?? byName;
                        if (existing != null)
                        {
                            ProtectDialog($"场景中已经有了Player标签或名为Player的物体\n名称：{existing.name}\n请使用此物体作为基准或者删除之"
                            );
                            break;
                        }
                        GameObject Player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
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
        int layer = LayerMask.NameToLayer("Player");
        if (layer == -1)
        {
            ProtectDialog($"图层 'Player' 不存在，无法自动设置，请添加图层后手动设置或再次生成预设");
        }
        else
        {
            temp.layer = layer;
        }
        var rb = temp.GetOrAddComponent<Rigidbody>();
        var collider = temp.GetOrAddComponent<Collider>();
        if (select == 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
