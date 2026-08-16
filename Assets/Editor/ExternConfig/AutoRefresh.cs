using UnityEditor;

[InitializeOnLoad]
public class AutoRefresh : EditorWindow
{
    static AutoRefresh()
    {
        AssemblyReloadEvents.afterAssemblyReload += () =>
        {
            EditorApplication.delayCall += () =>
            {
                if (HasOpenInstances<ExCore>())
                {
                    var window = GetWindow<ExCore>();
                    NavigationCore.Init(window, new MenuPage());
                }
            };
        };
    }
}