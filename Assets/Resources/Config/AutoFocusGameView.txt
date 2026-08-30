using UnityEditor;
[InitializeOnLoad]
public static class AutoFocusGameView
{
    static AutoFocusGameView()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += FocusGameView;
            };
        }
    }

    private static void FocusGameView()
    {
        var gameViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
        if (gameViewType == null) return;

        var gameView = EditorWindow.GetWindow(gameViewType);
        if (gameView != null)
        {
            gameView.Show();
            gameView.Focus();
            gameView.Repaint();
        }
    }
}