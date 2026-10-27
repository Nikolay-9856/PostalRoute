using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
// Единый источник сведений о выпуске: название, версия и ориентация в Player Settings,
// сцена Main в профиле сборки. Выполняется при открытии проекта и из меню.
[InitializeOnLoad]
public static class ReleaseSetup
{
    public const string ProductName = "Почтовый маршрут";
    public const string ReleaseVersion = "1.0.0";
    private const string ScenePath = "Assets/Scenes/Main.unity";
    static ReleaseSetup()
    {
        EditorApplication.delayCall += Apply;
    }
    [MenuItem("Почтовый маршрут/Применить настройки выпуска")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (PlayerSettings.productName != ProductName) PlayerSettings.productName = ProductName;
        if (PlayerSettings.bundleVersion != ReleaseVersion) PlayerSettings.bundleVersion = ReleaseVersion;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        if (!File.Exists(ScenePath))
        {
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
        }
        if (!EditorBuildSettings.scenes.Any(s => s.path == ScenePath))
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }
}
