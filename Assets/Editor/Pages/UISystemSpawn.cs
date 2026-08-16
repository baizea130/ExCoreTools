using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// UI系统控制器
/// </summary>
public class UISystemSpawn : MainFunction
{
    #region 基础属性
    public override string Title => "生成UI预设";
    public override string ParentPage => "主要功能";
    private int ResolutionX = 1920, ResolutionY = 1080;
    /// <summary>
    /// 详情面板以 纯文字 的形式呈现
    /// </summary>
    private bool ShowDetailByOnlyText = false;
    /// <summary>
    /// 点击后展现 纯文字 的物体标签
    /// </summary>
    private string ShowDetailByOnlyTextTag;
    /// <summary>
    /// 详情面板以 文字+静态图片 的形式呈现
    /// </summary>
    private bool ShowDetailByImg = false;
    /// <summary>
    /// 点击后展现 文字+静态图片 的物体标签
    /// </summary>
    private string ShowDetailByImgTag;
    /// <summary>
    /// 详情面板以 文字+3D可旋转缩放交互模型 的形式呈现
    /// </summary>
    private bool ShowDetailByModel = false;
    /// <summary>
    /// 点击后展现 文字+3D可旋转缩放交互模型 的物体标签
    /// </summary>
    private string ShowDetailByModelTag;
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
    private Vector2 scrollPos;
    #endregion
    /// <summary>
    /// 展示物品详情界面的背景图
    /// </summary>
    public Sprite DetailPanelBG;
    /// <summary>
    /// 展示物品详情界面的返回按钮图形
    /// </summary>
    public Sprite DetailPanelBackBtn;
    public override void OnEnter(object data)
    {
        ReadConfig();
    }
    private void ReadConfig()
    {
        StartPanel = MethodExtensions.CreateSO<ToolConfig>().StartPanel;
    }
    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        base.DrawBackBtn(ParentPage);

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
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
        DetailPanelExtension();

        GUILayout.Space(20);
        GUILayout.Label("其他常见功能");
        GUILayout.BeginHorizontal();
        StartPanel = GUILayout.Toggle(StartPanel, "开始界面");
        SettingPanel = GUILayout.Toggle(SettingPanel, "设置界面");
        QASystemPanel = GUILayout.Toggle(QASystemPanel, "答题系统");
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUI.contentColor = Color.yellow;
        if (GUILayout.Button("生成UI层级", GUILayout.Height(30)))
        {

            if (!ShowDetailByOnlyText && !ShowDetailByImg && !ShowDetailByModel)
            {
                ProtectDialog("至少选择一种点击详情界面形式");
                return;
            }
            ProtectDialog("即将按上述配置生成UI框架", () =>
            {
                SetUILayouts(SetCanvas(ResolutionX, ResolutionY));//创建Canvas和设置UI图层
                SetUiManager();
            }
            );
        }
        GUI.contentColor = Color.white;

        base.DrawBottomItem(Title);
    }
    /// <summary>
    /// 生成Canvas
    /// </summary>
    /// <param name="RX">参考分辨率X</param>
    /// <param name="RY">参考分辨率Y</param>
    private GameObject SetCanvas(int RX, int RY)
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
        return canvasGO;
    }
    /// <summary>
    /// 生成UI图层
    /// </summary>
    /// <param name="canvas"></param>
    private void SetUILayouts(GameObject canvas)
    {
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(canvas.transform.GetChild(i).gameObject);
        }
        List<GameObject> Layouts = new List<GameObject>();
        GameObject BottomLayout = new GameObject("BottomLayout", typeof(RectTransform));
        GameObject MidLayout = new GameObject("MidLayout", typeof(RectTransform));
        GameObject TopLayout = new GameObject("TopLayout", typeof(RectTransform));
        Layouts.Add(BottomLayout); Layouts.Add(MidLayout); Layouts.Add(TopLayout);
        foreach (var item in Layouts)
        {
            item.transform.SetParent(canvas.transform);
            item.GetOrAddComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.GetOrAddComponent<RectTransform>().sizeDelta = new Vector2(ResolutionX, ResolutionY);
        }
    }
    private void SetUiManager()
    {
        GameObject uiManager = GameObject.Find("uiManager");
        if (uiManager == null)
        {
            uiManager = new GameObject("uiManager");
        }
        List<GameObject> target = new List<GameObject>() { uiManager };
        CSharpWriter.Write("uiManager", null, target);
        MethodExtensions.CreateSO<ToolConfig>().StartPanel = StartPanel;
    }
    /// <summary>
    /// 根据详情勾选框的内容进行拓展
    /// </summary>
    private void DetailPanelExtension()
    {
        if (ShowDetailByOnlyText || ShowDetailByImg || ShowDetailByModel)
        {
            GUILayout.Space(15);
            DetailPanelBG = EditorGUILayout.ObjectField("详情界面背景Sprite", DetailPanelBG, typeof(Sprite), true) as Sprite;
            CheckFieldEmpty(DetailPanelBG);
            GUILayout.Space(15);
            DetailPanelBackBtn = EditorGUILayout.ObjectField("详情界面关闭按钮Sprite", DetailPanelBackBtn, typeof(Sprite), true) as Sprite;
            CheckFieldEmpty(DetailPanelBackBtn);
            GUILayout.Space(25);
            if (ShowDetailByOnlyText)
            {
                GUILayout.Label("点击后展示纯文本的物体标签：");
                ShowDetailByOnlyTextTag = EditorGUILayout.TextField("", ShowDetailByOnlyTextTag, GUILayout.ExpandWidth(true));
                CheckFieldEmpty(ShowDetailByOnlyTextTag, "*请确保编辑器中有同名标签");
            }
            if (ShowDetailByImg)
            {
                GUILayout.Space(15);
                GUILayout.Label("点击后展示 文本+图片 的物体标签：");
                ShowDetailByImgTag = EditorGUILayout.TextField("", ShowDetailByImgTag, GUILayout.ExpandWidth(true));
                CheckFieldEmpty(ShowDetailByImgTag, "*请确保编辑器中有同名标签");
            }
            if (ShowDetailByModel)
            {
                GUILayout.Space(15);
                GUILayout.Label("点击后展示 文本+3D模型 的物体标签：");
                ShowDetailByModelTag = EditorGUILayout.TextField("", ShowDetailByModelTag, GUILayout.ExpandWidth(true));
                CheckFieldEmpty(ShowDetailByModelTag, "*请确保编辑器中有同名标签");
            }
        }
    }
}
