using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Save system for cross-scene and play session data persistence.
/// </summary>
public class SaveManager : Singleton<SaveManager>
{    
    public PlayerData playerData { get; private set; }
    public PlayerController player;

    private Dictionary<int, string> activeCheckpoints = new Dictionary<int, string>();
    [SerializeField] private string firstLevelToLoad;
    [SerializeField] private int checkpointStart;


    private void Start()
    {
        LoadGameFromDisk();
        SceneController.Instance.LoadMainMenu();
    }

    #region Save

    /// <summary>
    /// Save the progress of the current play session.
    /// </summary>
    public void SavePlayerInfo(string level = "")
    {
        HealthManager health = player.GetComponent<HealthManager>();

        if (level != "") playerData.level = level;

        playerData.posX = player.transform.position.x;
        playerData.posY = player.transform.position.y;

        playerData.timeLeft = health.timeLeft;
        playerData.dmgTaken = health.dmgTaken;

        playerData.activeCheckpoints = activeCheckpoints;
        playerData.lastCheckpoint = health.respawnPoint;
    }

    /// <summary>
    /// Add the specified checkpoint to the list of activated ones for this playthrough.
    /// </summary>
    public void SaveCheckpoint(int id)
    {
        if (!activeCheckpoints.ContainsKey(id))
            activeCheckpoints.Add(id, SceneController.Instance.currentScene);
    }

    #endregion

    #region Load

    /// <summary>
    /// Load the progress of the most recent play session.
    /// </summary>
    private IEnumerator LoadPlayerInfo(string level, int checkpointID = -1)
    {       
        SceneController.Instance.LoadScene(level);

        // Wait for player object in next scene to link with this
        while (SceneManager.GetActiveScene().name != level || player == null)
        {
            yield return null;
        }

        HealthManager health = player.GetComponent<HealthManager>();            
        activeCheckpoints = playerData.activeCheckpoints;            

        // Load at previous or at checkpoint
        if (checkpointID == -1)
        {            
            health.SetCheckpoint(playerData.lastCheckpoint);
            health.SetHealthBar(playerData.timeLeft, playerData.dmgTaken);
        }
        else
        {            
            health.SetCheckpoint(checkpointID);
            health.Respawn();
        }
        
        Checkpoint.ReactivateAll(new List<int>(activeCheckpoints.Keys));
    }

    /// <summary>
    /// Load the scene containing the specified checkpoint.
    /// Returns whether a scene transition is needed.
    /// </summary>
    public bool LoadRespawn(int checkpointID = -1)
    {
        if (checkpointID == -1)
        {
            // New game
            playerData = new PlayerData();
            StartCoroutine(LoadPlayerInfo(firstLevelToLoad, checkpointStart));
            return true;
        }
        else
        {
            // Mid-game
            string level = activeCheckpoints[checkpointID];

            if (level != SceneController.Instance.currentScene)
            {
                StartCoroutine(LoadPlayerInfo(level, checkpointID));
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Load the specified level with the player at the specified position.
    /// </summary>
    public void LoadLevel(string level, Vector2 pos)
    {
        playerData.posX = pos.x;
        playerData.posY = pos.y;
        StartCoroutine(LoadPlayerInfo(level));
    }

    /// <summary>
    /// Load the progress of the most recent play session.
    /// </summary>
    public void LoadGame()
    {
        LoadLevel(playerData.level, new Vector2(playerData.posX, playerData.posY));
    }

    #endregion

    #region Disk

    /// <summary>
    /// Save game progress stored in the application onto the local disk.
    /// Location: \AppData\LocalLow\DefaultCompany\gdim-31
    /// </summary>
    private void SaveGameToDisk()
    {
        // Do this in case the player force closes the application
        if (GameStateManager.Instance.currentState == GameStateManager.GameState.RUNNING)
            SavePlayerInfo(SceneController.Instance.currentScene);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        bf.Serialize(file, playerData);
        file.Close();
    }

    /// <summary>
    /// Reads save data from local disk and loads it into the application.
    /// Location: \AppData\LocalLow\DefaultCompany\gdim-31
    /// </summary>
    private void LoadGameFromDisk()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            try
            {
                playerData = (PlayerData)bf.Deserialize(file);
            }
            catch (Exception ex)
            {
                playerData = new PlayerData();
                Debug.Log(ex);
            }

            file.Close();
        }
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveGameToDisk();
    }
}

[Serializable]
public class PlayerData
{
    public string level;

    public float posX;
    public float posY;

    public float timeLeft;
    public int dmgTaken;

    public Dictionary<int, string> activeCheckpoints = new Dictionary<int, string>();
    public int lastCheckpoint;
}
