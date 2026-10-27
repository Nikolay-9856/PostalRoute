using UnityEngine;
public static class GameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGame()
    {
        if (Object.FindAnyObjectByType<GameManager>() != null)
            return;
        var root = new GameObject("PostalRoute");
        root.AddComponent<GameManager>();
    }
}
