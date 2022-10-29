using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines all game state behavior.
/// </summary>
public class GameStateManager : Singleton<GameStateManager>
{
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
    }

    [SerializeField] private GameState _currentState = GameState.PREGAME;
    public GameState currentState
    {
        get { return _currentState; }
        private set { _currentState = value; }
    }

    /// <summary>
    /// Change the state of the game to PREGAME, RUNNING, or PAUSED.
    /// </summary>
    public void UpdateState(GameState state)
    {
        _currentState = state;

        switch (currentState)
        {
            case GameState.PAUSED:
                Time.timeScale = 0;
                break;
            default:
                Time.timeScale = 1;
                break;
        }
    }

    /// <summary>
    /// Pause/resume the game. Use for menus.
    /// </summary>
    public void TogglePause()
    {
        UpdateState(_currentState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }
}
