using System;
using UnityEngine;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.Gameplay.Player
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerModel playerModel;
        private PlayerView playerView;
        
        [Header("Production Update")]
        [SerializeField] private float productionUpdateInterval = 0.1f;
        private float productionTimer = 0f;

        [Header("Save Settings")]
        [SerializeField] private float autoSaveInterval = 30f;
        private float autoSaveTimer = 0f;

        public event Action<PlayerData> OnPlayerDataChanged;
        public event Action<PlayerClass> OnPlayerClassChanged;

        public double Rice => playerModel?.Rice ?? 0;
        public double Honor => playerModel?.Honor ?? 0;
        public PlayerClass CurrentClass => playerModel?.CurrentClass ?? PlayerClass.Slave;
        public double RicePerSecond => playerModel?.RicePerSecond ?? 0;
        public double HonorPerSecond => playerModel?.HonorPerSecond ?? 0;
        public double RicePerTap => playerModel?.RicePerTap ?? 1;

        private void Awake()
        {
            playerView = GetComponent<PlayerView>();
            if (playerView == null)
            {
                Debug.LogError("PlayerView component not found on the same GameObject!");
            }
        }

        public void Initialize(PlayerModel model)
        {
            if (model == null)
            {
                Debug.LogError("PlayerModel cannot be null!");
                return;
            }

            playerModel = model;
            Debug.Log($"PlayerPresenter initialized! Rice: {playerModel.Rice}, Honor: {playerModel.Honor}, RicePerTap: {playerModel.RicePerTap}");
            
            SubscribeToModelEvents();
            
            if (playerView != null)
            {
                UpdateView();
                Debug.Log("PlayerView updated");
            }
            else
            {
                Debug.LogWarning("PlayerView is null during initialization");
            }
            
            playerModel.ProcessOfflineEarnings();
        }

        private void OnDestroy()
        {
            UnsubscribeFromModelEvents();
        }

        private void Update()
        {
            if (playerModel == null) return;

            productionTimer += Time.deltaTime;
            if (productionTimer >= productionUpdateInterval)
            {
                playerModel.UpdateProduction(productionTimer);
                productionTimer = 0f;
            }

            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SavePlayerData();
                autoSaveTimer = 0f;
            }
        }

        public void OnTap()
        {
            Debug.Log("OnTap() called!");
            
            if (playerModel == null) 
            {
                Debug.LogError("PlayerModel is null in OnTap()!");
                return;
            }
            
            Debug.Log($"Current Class: {CurrentClass}");
            
            if (CurrentClass == PlayerClass.Slave)
            {
                bool hasOpportunity = IsWindowOfOpportunity();
                Debug.Log($"Slave class - Window of Opportunity: {hasOpportunity}");
                
                if (!hasOpportunity)
                {
                    Debug.Log("Tap ignored - no window of opportunity");
                    return;
                }
            }
            
            Debug.Log($"Performing tap - Rice per tap: {RicePerTap}");
            playerModel.PerformTap();
            Debug.Log($"After tap - Current Rice: {Rice}");
        }

        private bool IsWindowOfOpportunity()
        {
            // Temporary: High chance for testing - change back to 0.3f later
            return UnityEngine.Random.Range(0f, 1f) < 0.9f;
        }

        public bool PurchaseRiceUpgrade(double cost, double ricePerSecondIncrease, double ricePerTapIncrease = 0)
        {
            if (playerModel == null || !playerModel.SpendRice(cost)) 
            {
                Debug.Log($"Cannot purchase upgrade. Cost: {cost}, Current Rice: {Rice}");
                return false;
            }

            Debug.Log($"Purchased upgrade! Cost: {cost}, Rice/s increase: {ricePerSecondIncrease}, Tap increase: {ricePerTapIncrease}");

            if (ricePerSecondIncrease > 0)
            {
                double newRicePerSecond = playerModel.RicePerSecond + ricePerSecondIncrease;
                playerModel.UpdateRicePerSecond(newRicePerSecond);
                Debug.Log($"New Rice per second: {newRicePerSecond}");
            }

            if (ricePerTapIncrease > 0)
            {
                double newRicePerTap = playerModel.RicePerTap + ricePerTapIncrease;
                playerModel.UpdateRicePerTap(newRicePerTap);
                Debug.Log($"New Rice per tap: {newRicePerTap}");
            }

            return true;
        }

        [ContextMenu("Test Basic Upgrade")]
        public void TestBasicUpgrade()
        {
            // Give player some rice first
            if (playerModel != null)
            {
                playerModel.AddRice(100);
                // Buy a simple upgrade
                PurchaseRiceUpgrade(50, 2, 1); // Cost 50 rice, +2 rice/s, +1 rice/tap
            }
        }

        public bool PurchaseHonorBuilding(double riceCost, double honorPerSecondIncrease)
        {
            if (playerModel == null || !playerModel.SpendRice(riceCost)) return false;

            playerModel.UpdateHonorPerSecond(playerModel.HonorPerSecond + honorPerSecondIncrease);
            return true;
        }

        public bool TryAscendClass(double honorCost)
        {
            if (playerModel == null) return false;
            return playerModel.TryAscendClass(honorCost);
        }

        public void SavePlayerData()
        {
            if (playerModel == null) return;
            
            var data = playerModel.GetPlayerData();
            OnPlayerDataChanged?.Invoke(data);
        }

        public void LoadPlayerData(PlayerData data)
        {
            if (playerModel == null) return;
            
            playerModel.LoadPlayerData(data);
        }

        public void ResetProgress()
        {
            if (playerModel == null) return;
            
            playerModel.ResetProgress();
        }

        private void SubscribeToModelEvents()
        {
            if (playerModel == null) return;

            playerModel.OnRiceChanged += HandleRiceChanged;
            playerModel.OnHonorChanged += HandleHonorChanged;
            playerModel.OnClassChanged += HandleClassChanged;
            playerModel.OnRicePerSecondChanged += HandleRicePerSecondChanged;
            playerModel.OnHonorPerSecondChanged += HandleHonorPerSecondChanged;
            playerModel.OnRicePerTapChanged += HandleRicePerTapChanged;
            playerModel.OnKokuChanged += HandleKokuChanged;
        }

        private void UnsubscribeFromModelEvents()
        {
            if (playerModel == null) return;

            playerModel.OnRiceChanged -= HandleRiceChanged;
            playerModel.OnHonorChanged -= HandleHonorChanged;
            playerModel.OnClassChanged -= HandleClassChanged;
            playerModel.OnRicePerSecondChanged -= HandleRicePerSecondChanged;
            playerModel.OnHonorPerSecondChanged -= HandleHonorPerSecondChanged;
            playerModel.OnRicePerTapChanged -= HandleRicePerTapChanged;
            playerModel.OnKokuChanged -= HandleKokuChanged;
        }

        private void HandleRiceChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateRiceDisplay(newValue);
            }
        }

        private void HandleHonorChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateHonorDisplay(newValue);
            }
        }

        private void HandleClassChanged(PlayerClass newClass)
        {
            if (playerView != null)
            {
                playerView.UpdateClassDisplay(newClass);
            }
            OnPlayerClassChanged?.Invoke(newClass);
        }

        private void HandleRicePerSecondChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateRicePerSecondDisplay(newValue);
            }
        }

        private void HandleHonorPerSecondChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateHonorPerSecondDisplay(newValue);
            }
        }

        private void HandleRicePerTapChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateRicePerTapDisplay(newValue);
            }
        }

        private void HandleKokuChanged(double newValue)
        {
            if (playerView != null)
            {
                playerView.UpdateKokuDisplay(newValue);
            }
        }

        private void UpdateView()
        {
            if (playerView == null || playerModel == null) return;

            playerView.UpdateRiceDisplay(playerModel.Rice);
            playerView.UpdateHonorDisplay(playerModel.Honor);
            playerView.UpdateClassDisplay(playerModel.CurrentClass);
            playerView.UpdateRicePerSecondDisplay(playerModel.RicePerSecond);
            playerView.UpdateHonorPerSecondDisplay(playerModel.HonorPerSecond);
            playerView.UpdateRicePerTapDisplay(playerModel.RicePerTap);
            playerView.UpdateKokuDisplay(playerModel.Koku);
        }
    }
}