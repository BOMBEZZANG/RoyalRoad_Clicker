using UnityEngine;
using RoyalRoadClicker.Gameplay.Player;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        [Header("Core References")]
        [SerializeField] private PlayerPresenter playerPresenter;
        
        [Header("Game Settings")]
        [SerializeField] private bool loadSaveOnStart = true;
        [SerializeField] private bool debugMode = false;
        
        private PlayerModel playerModel;
        private float sessionStartTime;
        private long totalPlayTime;
        
        public PlayerModel PlayerModel => playerModel;
        public bool IsGameInitialized { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            sessionStartTime = Time.time;
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (IsGameInitialized) return;
            
            if (playerPresenter == null)
            {
                Debug.LogError("PlayerPresenter not assigned to GameManager!");
                return;
            }
            
            LoadOrCreatePlayer();
            
            playerPresenter.Initialize(playerModel);
            playerPresenter.OnPlayerDataChanged += HandlePlayerDataChanged;
            playerPresenter.OnPlayerClassChanged += HandlePlayerClassChanged;
            
            IsGameInitialized = true;
            
            if (debugMode)
            {
                Debug.Log("Game initialized successfully!");
            }
        }

        private void LoadOrCreatePlayer()
        {
            if (loadSaveOnStart)
            {
                SaveData saveData = PlayerDataSerializer.LoadFromFile();
                
                if (saveData != null && saveData.IsValidSave())
                {
                    playerModel = new PlayerModel(saveData.PlayerData);
                    totalPlayTime = saveData.PlayTime;
                    
                    if (debugMode)
                    {
                        Debug.Log($"Loaded save from {saveData.SaveTimestamp}");
                        Debug.Log($"Current resources - Rice: {saveData.PlayerData.Rice}, Honor: {saveData.PlayerData.Honor}");
                    }
                }
                else
                {
                    CreateNewPlayer();
                }
            }
            else
            {
                CreateNewPlayer();
            }
        }

        private void CreateNewPlayer()
        {
            playerModel = new PlayerModel();
            totalPlayTime = 0;
            
            if (debugMode)
            {
                Debug.Log("Created new player data");
            }
        }

        private void HandlePlayerDataChanged(PlayerData data)
        {
            long currentPlayTime = totalPlayTime + (long)(Time.time - sessionStartTime);
            SaveData saveData = new SaveData(data, currentPlayTime);
            bool success = PlayerDataSerializer.SaveToFile(saveData);
            
            if (debugMode && success)
            {
                Debug.Log($"Game saved at {saveData.SaveTimestamp}");
            }
        }

        private void HandlePlayerClassChanged(PlayerClass newClass)
        {
            if (debugMode)
            {
                Debug.Log($"Player ascended to {newClass}!");
            }
            
            // You can add special effects or notifications here
        }

        public void ResetGame()
        {
            if (playerPresenter != null)
            {
                playerPresenter.ResetProgress();
            }
            
            PlayerDataSerializer.DeleteSaveFile();
            totalPlayTime = 0;
            sessionStartTime = Time.time;
            
            if (debugMode)
            {
                Debug.Log("Game reset completed");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && IsGameInitialized)
            {
                SaveGame();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && IsGameInitialized)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            if (IsGameInitialized)
            {
                SaveGame();
            }
        }

        private void SaveGame()
        {
            if (playerPresenter != null)
            {
                playerPresenter.SavePlayerData();
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }

        #region Debug Methods
        [ContextMenu("Add Debug Rice")]
        public void DebugAddRice()
        {
            if (playerModel != null)
            {
                playerModel.AddRice(1000);
                Debug.Log("Added 1000 Rice");
            }
        }

        [ContextMenu("Add Debug Honor")]
        public void DebugAddHonor()
        {
            if (playerModel != null)
            {
                playerModel.AddHonor(100);
                Debug.Log("Added 100 Honor");
            }
        }

        [ContextMenu("Set Production Rates")]
        public void DebugSetProduction()
        {
            if (playerModel != null)
            {
                playerModel.UpdateRicePerSecond(100);
                playerModel.UpdateHonorPerSecond(10);
                Debug.Log("Set Rice/s: 100, Honor/s: 10");
            }
        }

        [ContextMenu("Set Small Production")]
        public void DebugSetSmallProduction()
        {
            if (playerModel != null)
            {
                playerModel.UpdateRicePerSecond(5);
                playerModel.UpdateHonorPerSecond(1);
                Debug.Log("Set Rice/s: 5, Honor/s: 1");
            }
        }

        [ContextMenu("Test Production Update")]
        public void DebugTestProductionUpdate()
        {
            if (playerModel != null)
            {
                Debug.Log($"Before: Rice={playerModel.Rice}, RicePerSecond={playerModel.RicePerSecond}");
                playerModel.UpdateProduction(1.0f); // Simulate 1 second
                Debug.Log($"After: Rice={playerModel.Rice}");
            }
        }

        [ContextMenu("Force Class Ascension")]
        public void DebugForceAscension()
        {
            if (playerModel != null && playerPresenter != null)
            {
                var playerStatus = playerPresenter.GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    var nextReq = playerStatus.GetNextClassRequirement();
                    if (nextReq != null)
                    {
                        playerModel.AddHonor(nextReq.honorRequired);
                        playerStatus.TryAscendToNextClass();
                    }
                }
            }
        }
        #endregion
    }
}