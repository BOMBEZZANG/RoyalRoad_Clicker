using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Data;
using RoyalRoadClicker.Core;

namespace RoyalRoadClicker.UI.Views
{
    public class UpgradePanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform clickUpgradesContainer;
        [SerializeField] private Transform productionUpgradesContainer;
        [SerializeField] private GameObject upgradeItemPrefab;

        [Header("Section Headers")]
        [SerializeField] private TextMeshProUGUI clickUpgradesHeader;
        [SerializeField] private TextMeshProUGUI productionUpgradesHeader;

        [Header("Data")]
        [SerializeField] private UpgradeData upgradeData;

        private List<UpgradeItemUI> activeUpgradeItems = new List<UpgradeItemUI>();
        private bool isInitialized = false;

        public UpgradeData UpgradeData
        {
            get => upgradeData;
            set
            {
                upgradeData = value;
                if (isInitialized) RefreshUpgradeList();
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                RefreshUpgradeList();
            }
        }

        public void Initialize()
        {
            if (isInitialized) return;

            ValidateReferences();
            SetupHeaders();
            RefreshUpgradeList();
            
            isInitialized = true;
        }

        private void ValidateReferences()
        {
            if (upgradeItemPrefab == null)
            {
                Debug.LogError("UpgradeItemPrefab is not assigned!");
                return;
            }

            if (clickUpgradesContainer == null || productionUpgradesContainer == null)
            {
                Debug.LogError("Upgrade containers are not assigned!");
                return;
            }

            if (upgradeData == null)
            {
                Debug.LogWarning("UpgradeData is not assigned! Create and assign an UpgradeData ScriptableObject.");
            }
        }

        private void SetupHeaders()
        {
            if (clickUpgradesHeader != null)
                clickUpgradesHeader.text = "클릭 강화 (Click Upgrades)";

            if (productionUpgradesHeader != null)
                productionUpgradesHeader.text = "생산 강화 (Production Upgrades)";
        }

        public void RefreshUpgradeList()
        {
            if (upgradeData == null) return;

            ClearExistingItems();
            CreateClickUpgradeItems();
            CreateProductionUpgradeItems();
        }

        private void ClearExistingItems()
        {
            // Clean up existing items
            foreach (var item in activeUpgradeItems)
            {
                if (item != null && item.gameObject != null)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
            activeUpgradeItems.Clear();
        }

        private void CreateClickUpgradeItems()
        {
            var clickUpgrades = upgradeData.ClickUpgrades;
            if (clickUpgrades == null) return;

            foreach (var upgrade in clickUpgrades)
            {
                if (upgrade == null) continue;

                var itemGO = Instantiate(upgradeItemPrefab, clickUpgradesContainer);
                var itemUI = itemGO.GetComponent<UpgradeItemUI>();
                
                if (itemUI != null)
                {
                    itemUI.Initialize(upgrade, GetCurrentUpgradeLevel);
                    itemUI.OnPurchaseClicked += HandleUpgradePurchase;
                    activeUpgradeItems.Add(itemUI);
                }
            }
        }

        private void CreateProductionUpgradeItems()
        {
            var productionUpgrades = upgradeData.ProductionUpgrades;
            if (productionUpgrades == null) return;

            foreach (var upgrade in productionUpgrades)
            {
                if (upgrade == null) continue;

                var itemGO = Instantiate(upgradeItemPrefab, productionUpgradesContainer);
                var itemUI = itemGO.GetComponent<UpgradeItemUI>();
                
                if (itemUI != null)
                {
                    itemUI.Initialize(upgrade, GetCurrentUpgradeLevel);
                    itemUI.OnPurchaseClicked += HandleUpgradePurchase;
                    activeUpgradeItems.Add(itemUI);
                }
            }
        }

        private int GetCurrentUpgradeLevel(string upgradeId)
        {
            var gameManager = GameManager.Instance;
            if (gameManager?.PlayerModel?.GetPlayerData()?.UpgradeProgress != null)
            {
                return gameManager.PlayerModel.GetPlayerData().UpgradeProgress.GetUpgradeLevel(upgradeId);
            }
            return 0;
        }

        private void HandleUpgradePurchase(UpgradeItem upgrade)
        {
            if (upgrade == null) return;

            // Send purchase request through event system
            var purchaseEvent = new UpgradePurchaseRequestEvent
            {
                UpgradeId = upgrade.id,
                UpgradeType = upgrade.upgradeType,
                Cost = upgrade.GetCostForLevel(GetCurrentUpgradeLevel(upgrade.id) + 1),
                Effect = upgrade.GetEffectForLevel(GetCurrentUpgradeLevel(upgrade.id) + 1)
            };

            EventManager.Trigger(purchaseEvent);
            Debug.Log($"Purchase request sent for {upgrade.displayName}");
        }

        public void RefreshUpgradeAvailability()
        {
            var gameManager = GameManager.Instance;
            if (gameManager?.PlayerModel == null) return;

            var currentClass = gameManager.PlayerModel.CurrentClass;
            var currentRicePerSecond = gameManager.PlayerModel.RicePerSecond;
            var currentRice = gameManager.PlayerModel.Rice;

            foreach (var itemUI in activeUpgradeItems)
            {
                if (itemUI != null)
                {
                    itemUI.RefreshAvailability(currentClass, currentRicePerSecond, currentRice);
                }
            }
        }

        private void Update()
        {
            // Refresh availability periodically (every 0.5 seconds)
            if (Time.time % 0.5f < Time.deltaTime)
            {
                RefreshUpgradeAvailability();
            }
        }
    }

    #region Event Definitions for Upgrades
    public struct UpgradePurchaseRequestEvent
    {
        public string UpgradeId;
        public UpgradeType UpgradeType;
        public double Cost;
        public double Effect;
        public System.DateTime Timestamp;

        public UpgradePurchaseRequestEvent(string id, UpgradeType type, double cost, double effect)
        {
            UpgradeId = id;
            UpgradeType = type;
            Cost = cost;
            Effect = effect;
            Timestamp = System.DateTime.Now;
        }
    }

    public struct UpgradePurchaseCompletedEvent
    {
        public string UpgradeId;
        public int NewLevel;
        public double CostPaid;
        public double NewTotalEffect;
        public System.DateTime Timestamp;

        public UpgradePurchaseCompletedEvent(string id, int level, double cost, double effect)
        {
            UpgradeId = id;
            NewLevel = level;
            CostPaid = cost;
            NewTotalEffect = effect;
            Timestamp = System.DateTime.Now;
        }
    }
    #endregion
}