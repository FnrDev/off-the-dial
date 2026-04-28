using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
static class PlayModeStartSceneSetup
{
    const string MainMenuScenePath = "Assets/ThunderWire Studio/UHFPS/Content/Scenes/MainMenu.unity";

    static PlayModeStartSceneSetup()
    {
        var mainMenu = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath);
        if (mainMenu != null)
            EditorSceneManager.playModeStartScene = mainMenu;
    }
}
