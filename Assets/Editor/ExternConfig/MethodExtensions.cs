using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class MethodExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                comp = Undo.AddComponent<T>(go);
            else
                comp = go.AddComponent<T>();
#else
            comp = go.AddComponent<T>();
#endif
        }
        return comp;
    }

    public static T CreateSO<T>() where T : ScriptableObject
    {
        string dir = "Assets/Resources/Config";
        string path = $"{dir}/{typeof(T).Name}.asset";
        T existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null)
            return existing;

        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }
}
