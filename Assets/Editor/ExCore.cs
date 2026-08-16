using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExCore : EditorWindow
{
    [MenuItem("Tools/生成配置文件", false, 0)]
    public static void InitConfig()
    {
        CSharpWriter.Write("MethodExtensions");
    }
    [MenuItem("Tools/ExCore", false, 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<ExCore>();
        NavigationCore.Init(window, new MenuPage());
    }
    private void OnEnable()
    {
        minSize = new Vector2(600, 300);
        maxSize = new Vector2(2160, 1980);
    }
    private void OnGUI()
    {
        NavigationCore.Current?.OnGUI();
    }
    private void OnDisable()
    {

    }
}
/// <summary>
/// 面板基类
/// </summary>
public abstract class EditorPage
{
    public virtual string Title => GetType().Name;
    public virtual string ParentPage => null;
    public virtual void OnEnter(object data) { }
    public abstract void OnGUI();
    public virtual void OnExit() { }
    /// <summary>
    /// 绘制返回按钮
    /// </summary>
    /// <param name="parent">返回目标的标题</param>
    public virtual void DrawBackBtn(string parent)
    {
        if (!string.IsNullOrEmpty(parent))
        {
            if (GUILayout.Button($"返回 -> {parent}", GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                NavigationCore.Pop();
            }
        }
    }
    /// <summary>
    /// 绘制底部导航栏
    /// </summary>
    /// <param name="title"></param>
    public virtual void DrawBottomItem(string title)
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
        if (NavigationCore.Current != null)
        {
            var breadcrumb = NavigationCore.GetPageNavData();
            GUILayout.Box(breadcrumb);
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndHorizontal();
    }
    protected GUIStyle GetTitleStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 24;
        res.alignment = TextAnchor.MiddleCenter;
        return res;
    }
    /// <summary>
    /// 生成Float输入框
    /// </summary>
    /// <param name="description">文本描述</param>
    /// <param name="param">外部参数</param>
    /// <param name="bound">数值边界</param>
    /// <param name="options">风格</param>
    /// <returns></returns>
    protected float SpawnFloatField(string description, float param, Vector2 bound, GUILayoutOption[] options = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{description} ({bound.x}~{bound.y}) :", GUILayout.Width(NavigationCore.ToolWindow.position.width / 3));
        param = EditorGUILayout.FloatField(param, options);
        param = Mathf.Clamp(param, bound.x, bound.y);
        GUILayout.EndHorizontal();
        return param;
    }
    /// <summary>
    /// 生成Int输入框
    /// </summary>
    /// <param name="description">文本描述</param>
    /// <param name="param">外部参数</param>
    /// <param name="bound">数值边界</param>
    /// <param name="options">风格</param>
    /// <returns></returns>
    protected int SpawnIntField(string description, int param, Vector2 bound, GUILayoutOption[] options = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{description} ({bound.x}~{bound.y}) :", GUILayout.Width(NavigationCore.ToolWindow.position.width / 3));
        param = EditorGUILayout.IntField(param, options);
        param = (int)Mathf.Clamp(param, bound.x, bound.y);
        GUILayout.EndHorizontal();
        return param;
    }
    /// <summary>
    /// 在重大操作前生成的保护性弹窗，Action为空时为单纯的提示弹窗
    /// </summary>
    /// <param name="content">内容文本</param>
    /// <param name="execute">点击确认后触发的功能</param>
    protected void ProtectDialog(string content, Action execute = null)
    {
        bool ok;
        if (execute == null)
        {
            ok = EditorUtility.DisplayDialog(
            "提示",        // 标题
            content,          // 内容
            "OK我收到"          // 确定按钮文字
            );
            return;
        }
        ok = EditorUtility.DisplayDialog(
            "提示",        // 标题
            content,          // 内容
            "OK我收到",           // 确定按钮文字
            "我再想想"            // 取消按钮文字
            );
        if (ok)
        {
            execute();
        }
    }
}
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
#region MenuPage_*
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
            DrawButton<PlayerTempSpawn>("生成玩家预设");
            DrawButton<FirstPersonSpawn>("第一人称控制器");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawButton<UISystemSwawn>("生成UI预设");
            DrawButton<InteractableSpawn>("交互逻辑生成控制器");
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
#endregion
/// <summary>
/// 主要功能界面下所有界面的基类
/// </summary>
public class MainFunction : EditorPage
{
    public override void OnGUI() { }
}
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
/// <summary>
/// UI系统控制器
/// </summary>
public class UISystemSwawn : MainFunction
{
    public override string Title => "生成UI预设";
    public override string ParentPage => "主要功能";
    private int ResolutionX = 1920, ResolutionY = 1080;
    /// <summary>
    /// 详情面板以 纯文字 的形式呈现
    /// </summary>
    private bool ShowDetailByOnlyText = true;
    /// <summary>
    /// 详情面板以 文字+静态图片 的形式呈现
    /// </summary>
    private bool ShowDetailByImg = false;
    /// <summary>
    /// 详情面板以 文字+3D可旋转缩放交互模型 的形式呈现
    /// </summary>
    private bool ShowDetailByModel = false;
    /// <summary>
    /// 开始界面
    /// </summary>
    private bool StartPanel = false;
    /// <summary>
    /// 设置界面
    /// </summary>
    private bool SettingPanel = false;
    /// <summary>
    /// 答题系统
    /// </summary>
    private bool QASystemPanel = false;
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);
        GUILayout.Label("Canvas参考分辨率");

        GUILayout.BeginHorizontal();
        ResolutionX = SpawnIntField("X:", ResolutionX, new Vector2(0, 9999));
        ResolutionY = SpawnIntField("Y:", ResolutionY, new Vector2(0, 9999));
        GUILayout.EndHorizontal();

        GUILayout.Label("点击交互所展现的详情界面形式");
        GUILayout.BeginHorizontal();
        ShowDetailByOnlyText = GUILayout.Toggle(ShowDetailByOnlyText, "纯文字");
        ShowDetailByImg = GUILayout.Toggle(ShowDetailByImg, "文字+图片");
        ShowDetailByModel = GUILayout.Toggle(ShowDetailByModel, "文字+3D可旋转缩放模型");
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("其他常见功能");
        GUILayout.BeginHorizontal();
        StartPanel = GUILayout.Toggle(StartPanel, "开始界面");
        SettingPanel = GUILayout.Toggle(SettingPanel, "设置界面");
        QASystemPanel = GUILayout.Toggle(QASystemPanel, "答题系统");
        GUILayout.EndHorizontal();

        if (GUILayout.Button("生成UI层级"))
        {
            if (!ShowDetailByOnlyText && !ShowDetailByImg && !ShowDetailByModel)
            {
                ProtectDialog("至少选择一种点击详情界面形式");
                return;
            }
            SetCanvas(ResolutionX, ResolutionY);
        }

        base.DrawBottomItem(Title);
    }
    /// <summary>
    /// 生成Canvas
    /// </summary>
    /// <param name="RX">参考分辨率X</param>
    /// <param name="RY">参考分辨率Y</param>
    private void SetCanvas(int RX, int RY)
    {
        GameObject canvasGO = GameObject.Find("MainCanvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("MainCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
            );
        }

        if (!GameObject.Find("EventSystem"))
        {
            GameObject EvtSystem = new GameObject("EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule)
            );
            Undo.RegisterCreatedObjectUndo(EvtSystem, "Create EventSystem");
        }
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        Canvas canvas = canvasGO.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(RX, RY);
    }
}
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
                ProtectDialog("你忘记绑定场景中的玩家了");
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