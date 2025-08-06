using UnityEngine;
using RoyalRoadClicker.Data;
using RoyalRoadClicker.UI.Views;
using RoyalRoadClicker.Gameplay.Player;

namespace RoyalRoadClicker.Core
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private UpgradeData upgradeData;

        private PlayerModel playerModel;
        private bool isInitialized = false;

        public UpgradeData UpgradeData => upgradeData;

        private void Awake()
        {
            // Subscribe to upgrade purchase events
            EventManager.Subscribe<UpgradePurchaseRequestEvent>(HandleUpgradePurchaseRequest);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            EventManager.Unsubscribe<UpgradePurchaseRequestEvent>(HandleUpgradePurchaseRequest);
        }

        public void Initialize()
        {
            if (isInitialized) return;

            var gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            playerModel = gameManager.PlayerModel;
            if (playerModel == null)
            {
                Debug.LogError("PlayerModel not found!");
                return;
            }

            if (upgradeData == null)
            {
                Debug.LogError("UpgradeData not assigned!");
                return;
            }

            RecalculatePlayerStats();
            isInitialized = true;

            Debug.Log("UpgradeManager initialized successfully");
        }

        private void HandleUpgradePurchaseRequest(UpgradePurchaseRequestEvent purchaseEvent)
        {
            if (!isInitialized || playerModel == null || upgradeData == null)
            {
                Debug.LogWarning("UpgradeManager not properly initialized");
                return;
            }

            var upgrade = upgradeData.GetUpgradeById(purchaseEvent.UpgradeId);
            if (upgrade == null)
            {
                Debug.LogError($"Upgrade not found: {purchaseEvent.UpgradeId}");
                return;
            }

            TryPurchaseUpgrade(upgrade);
        }

        public bool TryPurchaseUpgrade(UpgradeItem upgrade)
        {
            if (upgrade == null || playerModel == null) return false;

            var playerData = playerModel.GetPlayerData();
            int currentLevel = playerData.UpgradeProgress.GetUpgradeLevel(upgrade.id);

            // Check if already at max level
            if (currentLevel >= upgrade.maxLevel)
            {
                Debug.Log($"Upgrade {upgrade.displayName} is already at max level");
                return false;
            }

            // Calculate cost for next level
            double cost = upgrade.GetCostForLevel(currentLevel + 1);

            // Check if player can afford it
            if (playerModel.Rice < cost)
            {
                Debug.Log($"Cannot afford {upgrade.displayName}. Cost: {cost}, Current Rice: {playerModel.Rice}");
                return false;
            }

            // Check unlock requirements
            if (!upgrade.IsUnlocked(playerModel.CurrentClass, playerModel.RicePerSecond, 
                id => playerData.UpgradeProgress.GetUpgradeLevel(id)))
            {
                Debug.Log($"Upgrade {upgrade.displayName} is not unlocked yet");
                return false;
            }

            // Perform the purchase
            if (playerModel.SpendRice(cost))
            {
                // Increment upgrade level
                playerData.UpgradeProgress.IncrementUpgrade(upgrade.id);
                int newLevel = currentLevel + 1;

                // Apply the upgrade effect
                ApplyUpgradeEffect(upgrade, newLevel);

                // Send completion event
                var completionEvent = new UpgradePurchaseCompletedEvent(
                    upgrade.id, 
                    newLevel, 
                    cost, 
                    upgrade.GetTotalEffectForLevel(newLevel)
                );
                EventManager.Trigger(completionEvent);

                Debug.Log($"Successfully purchased {upgrade.displayName} level {newLevel}!");
                return true;
            }

            return false;
        }

        private void ApplyUpgradeEffect(UpgradeItem upgrade, int newLevel)
        {
            double totalEffect = upgrade.GetTotalEffectForLevel(newLevel);

            switch (upgrade.upgradeType)
            {
                case UpgradeType.ClickUpgrade:
                    // Calculate total tap bonus from all click upgrades
                    double totalTapBonus = CalculateTotalTapBonus();
                    playerModel.UpdateRicePerTap(1 + totalTapBonus); // Base 1 + bonuses
                    Debug.Log($"Updated Rice per tap to: {playerModel.RicePerTap}");
                    break;

                case UpgradeType.ProductionUpgrade:
                    // Calculate total production bonus from all production upgrades
                    double totalProductionBonus = CalculateTotalProductionBonus();
                    playerModel.UpdateRicePerSecond(totalProductionBonus);
                    Debug.Log($"Updated Rice per second to: {playerModel.RicePerSecond}");
                    break;
            }
        }

        private double CalculateTotalTapBonus()
        {
            double totalBonus = 0;
            var clickUpgrades = upgradeData.ClickUpgrades;
            var playerData = playerModel.GetPlayerData();

            foreach (var upgrade in clickUpgrades)
            {
                if (upgrade == null) continue;

                int level = playerData.UpgradeProgress.GetUpgradeLevel(upgrade.id);
                if (level > 0)
                {
                    totalBonus += upgrade.GetTotalEffectForLevel(level);
                }
            }

            return totalBonus;
        }

        private double CalculateTotalProductionBonus()
        {
            double totalBonus = 0;
            var productionUpgrades = upgradeData.ProductionUpgrades;
            var playerData = playerModel.GetPlayerData();

            foreach (var upgrade in productionUpgrades)
            {
                if (upgrade == null) continue;

                int level = playerData.UpgradeProgress.GetUpgradeLevel(upgrade.id);
                if (level > 0)
                {
                    totalBonus += upgrade.GetTotalEffectForLevel(level);
                }
            }

            return totalBonus;
        }

        public void RecalculatePlayerStats()
        {
            if (!isInitialized || playerModel == null) return;

            // Recalculate tap bonus
            double totalTapBonus = CalculateTotalTapBonus();
            playerModel.UpdateRicePerTap(1 + totalTapBonus);

            // Recalculate production bonus
            double totalProductionBonus = CalculateTotalProductionBonus();
            playerModel.UpdateRicePerSecond(totalProductionBonus);

            Debug.Log($"Recalculated stats - Tap: {playerModel.RicePerTap}, Production: {playerModel.RicePerSecond}");
        }

        public int GetUpgradeLevel(string upgradeId)
        {
            if (playerModel?.GetPlayerData()?.UpgradeProgress != null)
            {
                return playerModel.GetPlayerData().UpgradeProgress.GetUpgradeLevel(upgradeId);
            }
            return 0;
        }

        public UpgradeItem GetUpgradeById(string id)
        {
            return upgradeData?.GetUpgradeById(id);
        }

        public UpgradeItem[] GetAvailableUpgrades(UpgradeType? filterType = null)
        {
            if (upgradeData == null || playerModel == null) return new UpgradeItem[0];

            var allUpgrades = filterType.HasValue 
                ? upgradeData.GetUpgradesByType(filterType.Value)
                : upgradeData.GetAllUpgrades();

            var availableUpgrades = new System.Collections.Generic.List<UpgradeItem>();
            var playerData = playerModel.GetPlayerData();

            foreach (var upgrade in allUpgrades)
            {
                if (upgrade == null) continue;

                if (upgrade.IsUnlocked(playerModel.CurrentClass, playerModel.RicePerSecond, 
                    id => playerData.UpgradeProgress.GetUpgradeLevel(id)))
                {
                    availableUpgrades.Add(upgrade);
                }
            }

            return availableUpgrades.ToArray();
        }

        #region Debug Methods
        [ContextMenu("Force Recalculate Stats")]
        public void DebugRecalculateStats()
        {
            RecalculatePlayerStats();
        }

        [ContextMenu("Log All Upgrade Levels")]
        public void DebugLogUpgradeLevels()
        {
            if (playerModel?.GetPlayerData()?.UpgradeProgress == null) return;

            Debug.Log("=== Current Upgrade Levels ===");
            var allUpgrades = upgradeData.GetAllUpgrades();
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade == null) continue;
                int level = GetUpgradeLevel(upgrade.id);
                Debug.Log($"{upgrade.displayName}: Level {level}");
            }
        }
        #endregion
    }
}