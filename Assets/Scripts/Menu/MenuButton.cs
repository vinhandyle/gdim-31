using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The buttons in the menu.
/// </summary>
public class MenuButton : MonoBehaviour
{
    /// <summary>
    /// Start from the beginning of the game.
    /// Reset save file?
    /// </summary>
    public void StartGame()
    {
        SceneController.Instance.LoadScene("Level 1");
    }

    /// <summary>
    /// Continue from the previous save file progress.
    /// </summary>
    public void ContinueGame()
    { 
        // TODO after implementing save system
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
