using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// 交互逻辑生成控制器
/// </summary>
public class InteractableSpawn : MainFunction
{
    public override string Title => "交互逻辑生成控制器";
    public override string ParentPage => "主要功能";

    /// <summary>
    /// 展示物体的名称列表，不做类型区分
    /// 顺序：OnlyText -> Img -> Model
    /// </summary>
    public List<string> Titles = new List<string>();
    /// <summary>
    /// 点击后展示纯文字的碰撞体
    /// </summary>
    public List<Collider> CollidersByOnlyText = new List<Collider>();
    public List<string> StringsByOnlyText = new List<string>();

    public List<Collider> CollidersByImg = new List<Collider>();
    public List<string> StringsByImg = new List<string>();
    public List<Sprite> SpritesByImg = new List<Sprite>();
    public List<string> SpritesPathByImg = new List<string>();

    public List<Collider> CollidersByModel = new List<Collider>();
    public List<string> StringsByModel = new List<string>();
    public List<GameObject> ModelsByModel = new List<GameObject>();
    public List<GameObject> PivotsByModel = new List<GameObject>();

    private Vector2 scrollPos;

    private enum InteractableType
    {
        OnlyText, Img, Model
    }

    public override void OnEnter(object data)
    {
        ReadConfig();
    }

    public override void OnGUI()
    {
        GUILayout.Label(Title, GetTitleStyle());
        GUILayout.Label("在生成UI预设界面勾选对应展示模式后此处会自动更新");

        base.DrawBackBtn(ParentPage);

        GUI.contentColor = Color.yellow;
        if (GUILayout.Button("清空配置"))
        {
            ProtectDialog("即将清空该界面下所有列表(无法撤销)",
            () =>
            {
                foreach (var c in CollidersByOnlyText) { if (c != null) { Undo.RecordObject(c.gameObject, "Reset Tag"); c.gameObject.tag = "Untagged"; } }
                foreach (var c in CollidersByImg) { if (c != null) { Undo.RecordObject(c.gameObject, "Reset Tag"); c.gameObject.tag = "Untagged"; } }
                foreach (var c in CollidersByModel) { if (c != null) { Undo.RecordObject(c.gameObject, "Reset Tag"); c.gameObject.tag = "Untagged"; } }
                #region List Clear
                CollidersByOnlyText.Clear();
                StringsByOnlyText.Clear();
                CollidersByImg.Clear();
                StringsByImg.Clear();
                SpritesByImg.Clear();
                CollidersByModel.Clear();
                StringsByModel.Clear();
                ModelsByModel.Clear();
                PivotsByModel.Clear();
                Titles.Clear();
                #endregion
            }
            );
        }
        GUI.contentColor = Color.white;
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        if (MethodExtensions.GetOrCreateSO<ToolConfig>(true).ShowDetailByOnlyText)
            DrawObjList("点击后显示 纯文字 的物体", CollidersByOnlyText, StringsByOnlyText, InteractableType.OnlyText);
        if (MethodExtensions.GetOrCreateSO<ToolConfig>(true).ShowDetailByImg)
            DrawObjList("点击后显示 文字+图片 的物体", CollidersByImg, StringsByImg, InteractableType.Img);
        if (MethodExtensions.GetOrCreateSO<ToolConfig>(true).ShowDetailByModel)
            DrawObjList("点击后显示 文字+模型 的物体", CollidersByModel, StringsByModel, InteractableType.Model);

        GUILayout.EndScrollView();
        GUI.contentColor = Color.yellow;
        if (GUILayout.Button("应用配置", GUILayout.Height(30)))
        {
            ProtectDialog("即将生成ScriptableObject配置文件和相应脚本", () =>
            {
                WriteConfig();
                RefreshSO();
            });
        }
        GUI.contentColor = Color.white;
        base.DrawBottomItem(Title);
    }
    private int GetTitleIndex(int i, InteractableType type)
    {
        switch (type)
        {
            case InteractableType.OnlyText: return i;
            case InteractableType.Img: return i + CollidersByOnlyText.Count;
            case InteractableType.Model: return i + CollidersByOnlyText.Count + CollidersByImg.Count;
            default: return i;
        }
    }

    // ========== RefreshSO 重写 ==========
    private void RefreshSO()
    {
        string soFolder = "Assets/Resources/SO";
        if (!Directory.Exists(soFolder))
        {
            Directory.CreateDirectory(soFolder);
        }
        else
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { soFolder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }
        }

        AddTag("Interactable");
        // 2. 重建 TextOnlySO
        for (int i = 0; i < CollidersByOnlyText.Count; i++)
        {
            if (CollidersByOnlyText[i] == null) continue;

            CollidersByOnlyText[i].gameObject.tag = "Interactable";
            var so = ScriptableObject.CreateInstance<TextOnlySO>();
            so.name = CollidersByOnlyText[i].gameObject.name;
            so.objName = Titles[i];
            so.detail = StringsByOnlyText[i];

            string fileName = SanitizeFileName(CollidersByOnlyText[i].gameObject.name);
            string path = AssetDatabase.GenerateUniqueAssetPath($"{soFolder}/{fileName}.asset");
            AssetDatabase.CreateAsset(so, path);
        }

        // 3. 重建 ImgSO
        int imgTitleOffset = CollidersByOnlyText.Count;
        for (int i = 0; i < CollidersByImg.Count; i++)
        {
            if (CollidersByImg[i] == null) continue;

            CollidersByImg[i].gameObject.tag = "Interactable";
            var so = ScriptableObject.CreateInstance<ImgSO>();
            so.name = CollidersByImg[i].gameObject.name;
            so.objName = Titles[i + imgTitleOffset];
            so.objSprite = SpritesByImg[i];
            so.spritePath = SpritesPathByImg[i];
            so.detail = StringsByImg[i];

            string fileName = SanitizeFileName(CollidersByImg[i].gameObject.name);
            string path = AssetDatabase.GenerateUniqueAssetPath($"{soFolder}/{fileName}.asset");
            AssetDatabase.CreateAsset(so, path);
        }

        // 4. 重建 ModelSO
        int modelTitleOffset = CollidersByOnlyText.Count + CollidersByImg.Count;
        for (int i = 0; i < CollidersByModel.Count; i++)
        {
            if (CollidersByModel[i] == null) continue;

            CollidersByModel[i].gameObject.tag = "Interactable";
            var so = ScriptableObject.CreateInstance<ModelSO>();
            so.name = CollidersByModel[i].gameObject.name;
            so.objName = Titles[i + modelTitleOffset];
            so.objModel = ModelsByModel[i];
            so.pivot = PivotsByModel[i];
            so.detail = StringsByModel[i];

            string fileName = SanitizeFileName(CollidersByModel[i].gameObject.name);
            string path = AssetDatabase.GenerateUniqueAssetPath($"{soFolder}/{fileName}.asset");
            AssetDatabase.CreateAsset(so, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private string SanitizeFileName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "Unnamed";
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }

    /// <summary>
    /// 绘制填充项
    /// </summary>
    private void DrawObjList(string content, List<Collider> colliders, List<string> texts, InteractableType type)
    {
        GUILayout.Label(content, GetSubheadingStyle());
        for (int i = 0; i < Mathf.Min(colliders.Count, texts.Count); i++)
        {
            int titleIndex = GetTitleIndex(i, type);
            if (titleIndex >= Titles.Count) break;

            GUI.contentColor = Color.yellow;
            GUILayout.Label($"[{i}]");
            GUI.contentColor = Color.white;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("详情界面的名称/标题");
            Titles[titleIndex] = EditorGUILayout.TextField(Titles[titleIndex], GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            colliders[i] = EditorGUILayout.ObjectField("碰撞体", colliders[i], typeof(Collider), true) as Collider;
            CheckFieldEmpty(colliders[i]);

            if (type == InteractableType.Img)
            {
                if (i < SpritesByImg.Count)
                {
                    SpritesByImg[i] = EditorGUILayout.ObjectField($"展示的图片", SpritesByImg[i], typeof(Sprite), true) as Sprite;
                    CheckFieldEmpty(SpritesByImg[i]);
                    SpritesPathByImg[i] = GetAssetPath(SpritesByImg[i]);
                }

            }

            if (type == InteractableType.Model)
            {
                if (i < PivotsByModel.Count)
                {
                    ModelsByModel[i] = EditorGUILayout.ObjectField($"展示的模型", ModelsByModel[i], typeof(GameObject), true) as GameObject;
                    GUILayout.BeginHorizontal();
                    PivotsByModel[i] = EditorGUILayout.ObjectField($"旋转焦点", PivotsByModel[i], typeof(GameObject), true) as GameObject;
                    if (GUILayout.Button("模型自身应用旋转焦点"))
                    {
                        PivotsByModel[i] = ModelsByModel[i];
                    }
                    GUILayout.EndHorizontal();
                }

            }
            EditorGUILayout.LabelField("描述");
            float descWidth = EditorGUIUtility.currentViewWidth - 30f;
            texts[i] = DrawAutoHeightDescription(texts[i], descWidth, $"{type}_{i}");

            if (GUILayout.Button("删除此项"))
            {
                int removeTitleIndex = GetTitleIndex(i, type);
                if (removeTitleIndex < Titles.Count)
                    Titles.RemoveAt(removeTitleIndex);

                colliders[i].gameObject.tag = "Untagged";
                colliders.RemoveAt(i);
                texts.RemoveAt(i);
                if (type == InteractableType.Img)
                {
                    if (i < SpritesByImg.Count) SpritesByImg.RemoveAt(i);
                    if (i < SpritesPathByImg.Count) SpritesPathByImg.RemoveAt(i);
                }
                if (type == InteractableType.Model)
                {
                    if (i < ModelsByModel.Count) ModelsByModel.RemoveAt(i);
                    if (i < PivotsByModel.Count) PivotsByModel.RemoveAt(i);
                }
                i--;
            }
            GUILayout.Space(15);
        }

        if (GUILayout.Button("+", GetListBtnStyle(), GUILayout.Width(30)))
        {
            colliders.Add(null);
            texts.Add(null);

            // 计算 Titles 插入位置：插到对应 type 段的末尾
            int insertIndex = -1;
            switch (type)
            {
                case InteractableType.OnlyText:
                    insertIndex = CollidersByOnlyText.Count - 1;
                    break;
                case InteractableType.Img:
                    insertIndex = CollidersByOnlyText.Count + CollidersByImg.Count - 1;
                    break;
                case InteractableType.Model:
                    insertIndex = CollidersByOnlyText.Count + CollidersByImg.Count + CollidersByModel.Count - 1;
                    break;
            }
            if (insertIndex >= 0 && insertIndex <= Titles.Count)
                Titles.Insert(insertIndex, null);
            else
                Titles.Add(null);

            if (type == InteractableType.Img)
            {
                SpritesByImg.Add(null);
                SpritesPathByImg.Add(null);
            }
            if (type == InteractableType.Model)
            {
                ModelsByModel.Add(null);
                PivotsByModel.Add(null);
            }
        }
    }

    private GUIStyle GetSubheadingStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 16;
        return res;
    }

    private GUIStyle GetListBtnStyle()
    {
        var res = new GUIStyle(EditorStyles.label);
        res.fontSize = 30;
        res.alignment = TextAnchor.MiddleCenter;
        return res;
    }

    public override void OnExit()
    {
        WriteConfig();
    }

    // ===================== 自适应高度描述文本区域 =====================
    private Dictionary<string, Vector2> _descScrollCache = new Dictionary<string, Vector2>();

    private float CalcTextAreaHeight(string text, float width)
    {
        var style = new GUIStyle(EditorStyles.textArea);
        style.wordWrap = true;
        return style.CalcHeight(new GUIContent(text), width);
    }

    private string DrawAutoHeightDescription(string text, float viewWidth, string uniqueKey)
    {
        var textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;

        float contentHeight = CalcTextAreaHeight(text, viewWidth);
        float threeLineHeight = CalcTextAreaHeight("1\n2\n3\n4", viewWidth);
        float minHeight = EditorGUIUtility.singleLineHeight;
        float targetHeight = Mathf.Clamp(contentHeight, minHeight, threeLineHeight);

        if (contentHeight > threeLineHeight)
        {
            if (!_descScrollCache.ContainsKey(uniqueKey))
                _descScrollCache[uniqueKey] = Vector2.zero;
            GUIStyle hiddenVScroll = new GUIStyle(GUI.skin.verticalScrollbar);
            hiddenVScroll.fixedWidth = 0;
            _descScrollCache[uniqueKey] = GUILayout.BeginScrollView(
                _descScrollCache[uniqueKey],
                GUIStyle.none,
                hiddenVScroll,
                GUILayout.Height(targetHeight),
                GUILayout.ExpandWidth(true)
            );

            text = EditorGUILayout.TextArea(text, textAreaStyle, GUILayout.Height(contentHeight));

            GUILayout.EndScrollView();
        }
        else
        {
            text = EditorGUILayout.TextArea(text, textAreaStyle, GUILayout.Height(targetHeight));
        }

        return text;
    }
}