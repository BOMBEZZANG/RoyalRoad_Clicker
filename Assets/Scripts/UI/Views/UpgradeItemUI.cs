using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.UI.Views
{
    public class UpgradeItemUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI effectText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Header("Visual Settings")]
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = Color.red;
        [SerializeField] private Color lockedColor = Color.gray;

        private UpgradeItem upgradeItem;
        private Func<string, int> getLevelCallback;
        private int currentLevel;
        private bool isUnlocked;
        private bool isAffordable;

        public event Action<UpgradeItem> OnPurchaseClicked;

        public UpgradeItem UpgradeItem => upgradeItem;
        public int CurrentLevel => currentLevel;

        public void Initialize(UpgradeItem item, Func<string, int> levelCallback)
        {
            upgradeItem = item;
            getLevelCallback = levelCallback;

            if (upgradeItem == null)
            {
                Debug.LogError("UpgradeItem is null!");
                return;
            }

            SetupUI();
            SetupButton();
            RefreshDisplay();
        }

        private void SetupUI()
        {
            // Set static information
            if (nameText != null)
                nameText.text = upgradeItem.displayName;

            if (descriptionText != null)
                descriptionText.text = upgradeItem.description;

            if (iconImage != null && upgradeItem.icon != null)
                iconImage.sprite = upgradeItem.icon;

            // Set button text
            if (buttonText != null)
            {
                buttonText.text = upgradeItem.upgradeType == UpgradeType.ClickUpgrade ? "강화하기" : "구매하기";
            }
        }

        private void SetupButton()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveAllListeners();
                purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
            }
        }

        private void OnPurchaseButtonClicked()
        {
            if (upgradeItem != null && isUnlocked && isAffordable)
            {
                OnPurchaseClicked?.Invoke(upgradeItem);
            }
        }

        public void RefreshDisplay()
        {
            if (upgradeItem == null || getLevelCallback == null) return;

            currentLevel = getLevelCallback(upgradeItem.id);
            UpdateLevelDisplay();
            UpdateCostAndEffect();
        }

        public void RefreshAvailability(PlayerClass currentClass, double currentRicePerSecond, double currentRice)
        {
            if (upgradeItem == null) return;

            // Check if upgrade is unlocked
            isUnlocked = upgradeItem.IsUnlocked(currentClass, currentRicePerSecond, getLevelCallback);

            // Check if player can afford it
            double nextCost = upgradeItem.GetCostForLevel(currentLevel + 1);
            isAffordable = currentRice >= nextCost && currentLevel < upgradeItem.maxLevel;

            UpdateVisualState();
        }

        private void UpdateLevelDisplay()
        {
            if (levelText != null)
            {
                if (currentLevel > 0)
                {
                    levelText.text = $"Lv.{currentLevel}";
                    levelText.gameObject.SetActive(true);
                }
                else
                {
                    levelText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateCostAndEffect()
        {
            int nextLevel = currentLevel + 1;
            
            // Update cost display
            if (costText != null)
            {
                if (currentLevel >= upgradeItem.maxLevel)
                {
                    costText.text = "MAX";
                    costText.color = Color.yellow;
                }
                else
                {
                    double cost = upgradeItem.GetCostForLevel(nextLevel);
                    costText.text = FormatNumber(cost) + " 쌀";
                }
            }

            // Update effect display
            if (effectText != null)
            {
                if (currentLevel >= upgradeItem.maxLevel)
                {
                    // Show total effect at max level
                    double totalEffect = upgradeItem.GetTotalEffectForLevel(currentLevel);
                    string effectType = upgradeItem.upgradeType == UpgradeType.ClickUpgrade ? "/탭" : "/초";
                    effectText.text = $"총 효과: +{FormatNumber(totalEffect)}{effectType}";
                }
                else
                {
                    // Show next level effect
                    double nextEffect = upgradeItem.GetEffectForLevel(nextLevel);
                    string effectType = upgradeItem.upgradeType == UpgradeType.ClickUpgrade ? "/탭" : "/초";
                    effectText.text = $"+{FormatNumber(nextEffect)}{effectType}";
                }
            }
        }

        private void UpdateVisualState()
        {
            if (purchaseButton == null) return;

            // Update button interactability
            bool canPurchase = isUnlocked && isAffordable && currentLevel < upgradeItem.maxLevel;
            purchaseButton.interactable = canPurchase;

            // Update visual colors
            Color targetColor;
            if (!isUnlocked)
            {
                targetColor = lockedColor;
            }
            else if (currentLevel >= upgradeItem.maxLevel)
            {
                targetColor = Color.yellow; // Max level
            }
            else if (isAffordable)
            {
                targetColor = affordableColor;
            }
            else
            {
                targetColor = unaffordableColor;
            }

            // Apply color to cost text
            if (costText != null && currentLevel < upgradeItem.maxLevel)
            {
                costText.color = isAffordable ? affordableColor : unaffordableColor;
            }

            // Update button color
            var buttonColors = purchaseButton.colors;
            buttonColors.normalColor = targetColor;
            buttonColors.highlightedColor = Color.Lerp(targetColor, Color.white, 0.1f);
            purchaseButton.colors = buttonColors;
        }

        private string FormatNumber(double number)
        {
            if (number < 1000)
                return number.ToString("F0");
            else if (number < 1000000)
                return (number / 1000).ToString("F1") + "K";
            else if (number < 1000000000)
                return (number / 1000000).ToString("F1") + "M";
            else if (number < 1000000000000)
                return (number / 1000000000).ToString("F1") + "B";
            else if (number < 1000000000000000)
                return (number / 1000000000000).ToString("F1") + "T";
            else
                return (number / 1000000000000000).ToString("F1") + "Q";
        }

        public void SetCustomIcon(Sprite icon)
        {
            if (iconImage != null)
            {
                iconImage.sprite = icon;
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            // Add visual feedback for recently purchased items
            if (highlighted)
            {
                // Could add glow effect, animation, etc.
            }
        }

        private void OnDestroy()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveAllListeners();
            }
        }
    }
}