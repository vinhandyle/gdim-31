using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines all scene-related actions.
/// </summary>
public class SceneController : Singleton<SceneController>
{
    public string currentScene { get; private set; }
    private const string menuScene = "Menu";
    private const string pauseMenu = "Pause";
    private const string endMenu = "Ending";

    /// <summary>
    /// Boot up the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene(menuScene);
        GameStateManager.Instance.UpdateState(GameStateManager.GameState.PREGAME);
    }

    /// <summary>
    /// Loads the scene with the given name.
    /// </summary>
    public void LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene, mode);
        StartCoroutine(SceneProgress(ao, scene, 0));
        currentScene = scene;

        if (scene == menuScene)
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.PREGAME);
        else if (scene != pauseMenu)
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.RUNNING);

        // Play music
        switch (scene)
        {
            case menuScene:
                AudioController.Instance.PlayTrack(0);
                break;

            case "Level 1":
            case "Level 1E":
                AudioController.Instance.PlayTrack(1);
                break;

            case "Level 2":
                AudioController.Instance.PlayTrack(2);
                break;

            case "Level 3":
                AudioController.Instance.PlayTrack(3);
                break;

            case endMenu:
                AudioController.Instance.PlayTrack(4);
                break;
        }
    }

    /// <summary>
    /// Unloads the scene with the given name.
    /// </summary>
    public void UnloadScene(string scene)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(scene);
        StartCoroutine(SceneProgress(ao, scene, 1));
        currentScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Use to track the progress of loading a scene.
    /// </summary>
    private IEnumerator SceneProgress(AsyncOperation ao, string scene, int type)
    {
        if (ao == null)
        {
            Debug.LogError(string.Format("Unable to {0} {1}", type == 0 ? "load" : "unload", scene));
            yield break;
        }

        while (!ao.isDone)
        {
            Debug.Log(string.Format("{0} {1} in progress: {2}%", type == 0 ? "Loading" : "Unloading", scene, Mathf.Clamp(ao.progress / 0.9f, 0, 1) * 100));
            yield return null;
        }
    }
}
