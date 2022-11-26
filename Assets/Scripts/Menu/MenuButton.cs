using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The buttons in the menu.
/// </summary>
public class MenuButton : MonoBehaviour
{
    [SerializeField] private bool hideNewGame;

    private void Awake()
    {
        if (hideNewGame) gameObject.SetActive(SaveManager.Instance.playerData.level != null);
    }

    /// <summary>
    /// Start from the beginning of the game.
    /// Reset save file?
    /// </summary>
    public void StartGame()
    {
        SaveManager.Instance.LoadRespawn();
    }

    /// <summary>
    /// Continue from the previous save file progress.
    /// </summary>
    public void ContinueGame()
    {
        SaveManager.Instance.LoadGame();
    }

    /// <summary>
    /// [Un]pause the game.
    /// </summary>
    public void TogglePause()
    {
        GameStateManager.Instance.TogglePause();
    }

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void ReturnToMenu()
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            string s = SceneManager.GetSceneAt(i).name;
            if (s.Contains("Level"))
            {
                SaveManager.Instance.SavePlayerInfo(s);
                break;
            }
        }
       
        SceneController.Instance.UnloadScene("Pause");
        SceneController.Instance.LoadScene("Menu");
    }

    /// <summary>
    /// Close the game.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
