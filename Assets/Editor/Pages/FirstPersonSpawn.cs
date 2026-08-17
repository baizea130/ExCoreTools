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
    public float Speed;
    public float Height;
    public float Sensitivity;
    public float Jump;
    private GameObject Player;
    public bool HidePlayerLayout;
    public override void OnEnter(object data)
    {
        ReadConfig();
    }
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);

        Player = EditorGUILayout.ObjectField(
       "场景中的玩家预制体", Player, typeof(GameObject), true) as GameObject;
        CheckFieldEmpty(Player);
        HidePlayerLayout = GUILayout.Toggle(HidePlayerLayout, "主摄像机隐藏 Player 图层");
        Speed = SpawnFloatField("移动速度", Speed, new Vector2(0, 999));
        Height = SpawnFloatField("模型身高", Height, new Vector2(0, 999));
        Jump = SpawnFloatField("跳跃高度", Jump, new Vector2(0, 999));
        Sensitivity = SpawnFloatField("视角灵敏度", Sensitivity, new Vector2(1, 9999));
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
                    if (HidePlayerLayout)
                    {
                        HideLayer();
                    }
                    else
                    {
                        ShowLayer();
                    }
                    var replacements = new Dictionary<string, string>
                    {
                        { "Speed",       Speed.ToString() },
                        { "Jump",        Jump.ToString() },
                        { "Sensitivity", Sensitivity.ToString() },
                        { "Height",      Height.ToString() }
                    };
                    List<GameObject> target = new List<GameObject> { Player };
                    Debug.Log(CSharpWriter.Write("FirstPerson", replacements, target));
                    WriteConfig();
                });
            }
        }

        base.DrawBottomItem(Title);
    }
    /// <summary>屏蔽图层</summary>
    public void HideLayer()
    {
        int layer = LayerMask.NameToLayer("Player");
        if (layer == -1)
        {
            ProtectDialog($"图层 '{Player}' 不存在！");
            return;
        }
        Camera.main.cullingMask &= ~(1 << layer);
    }

    /// <summary>解除屏蔽图层</summary>
    public void ShowLayer()
    {
        int layer = LayerMask.NameToLayer("Player");
        if (layer == -1)
        {
            ProtectDialog($"图层 '{Player}' 不存在！");
            return;
        }
        Camera.main.cullingMask |= 1 << layer;
    }

}
