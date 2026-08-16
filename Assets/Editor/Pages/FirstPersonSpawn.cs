using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 第一人称控制器界面
/// </summary>
public class FirstPersonSpawn : MainFunction
{
    public override string Title => "第一人称控制器";
    public override string ParentPage => "主要功能";
    private PlayerData data = new PlayerData();
    private GameObject Player;
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);

        Player = EditorGUILayout.ObjectField(
       "场景中的玩家预制体", Player, typeof(GameObject), true) as GameObject;
        data.Speed = SpawnFloatField("移动速度", data.Speed, new Vector2(0, 999));
        data.Height = SpawnFloatField("模型身高", data.Height, new Vector2(0, 999));
        data.Jump = SpawnFloatField("跳跃高度", data.Jump, new Vector2(0, 999));
        data.Sensitivity = SpawnFloatField("视角灵敏度", data.Sensitivity, new Vector2(1, 9999));
        if (GUILayout.Button("确认生成"))
        {
            if (Player == null)
            {
                ProtectDialog("未绑定场景中的玩家预制体");
            }
            else
            {
                ProtectDialog("即将生成/覆盖第一人称控制器脚本并自动附加", () =>
                {
                    var replacements = new Dictionary<string, string>
                    {
                        { "Speed",       data.Speed.ToString() },
                        { "Jump",        data.Jump.ToString() },
                        { "Sensitivity", data.Sensitivity.ToString() },
                        { "Height",      data.Height.ToString() }
                    };
                    List<GameObject> target = new List<GameObject> { Player };
                    Debug.Log(CSharpWriter.Write("FirstPerson", replacements, target));
                });
            }
        }

        base.DrawBottomItem(Title);
    }
    [Serializable]
    private class PlayerData
    {
        public float Speed;
        public float Height;
        public float Sensitivity;
        public float Jump;
    }
}
