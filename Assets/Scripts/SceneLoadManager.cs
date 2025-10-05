using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Title,
    Town,
    Dungeon,
}

public static class SceneLoader
{
    public static SceneName CurrentScene
    {
        get
        {
            var scene = SceneManager.GetActiveScene();
            if (System.Enum.TryParse<SceneName>(scene.name, out var sceneName))
            {
                return sceneName;
            }
            return default;
        }
    }
    public static void LoadScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }
    public static void ReLoadScene()
    {
        LoadScene(CurrentScene);
    }
}


