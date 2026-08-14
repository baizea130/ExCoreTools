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
}