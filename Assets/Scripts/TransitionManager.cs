using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    enum Scene
    {
        Title,
        MainScene,
        Credits,
    }
    private static void ToScene(Scene scene)
    {
        SceneManager.LoadScene((int)scene, LoadSceneMode.Single);
    }
    public static void ToTitle()
    {
        ToScene(Scene.Title);
    }

    public static void ToMainScene()
    {
        ToScene(Scene.MainScene);
    }
    public static void ToCredits()
    {
        ToScene(Scene.Credits);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
