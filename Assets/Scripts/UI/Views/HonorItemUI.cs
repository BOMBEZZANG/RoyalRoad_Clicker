using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoyalRoadClicker.UI.Views
{
    public class HonorItemUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI effectText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Header("Visual Settings")]
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = Color.red;

        private HonorBuildingData buildingData;
        private HonorActivityData activityData;
        private bool isBuilding = false;
        private bool isAffordable = false;

        private Action<HonorBuildingData> buildingCallback;
        private Action<HonorActivityData> activityCallback;

        public void InitializeAsBuilding(HonorBuildingData building, Action<HonorBuildingData> callback)
        {
            buildingData = building;
            buildingCallback = callback;
            isBuilding = true;

            SetupUI();
            SetupButton();
        }

        public void InitializeAsActivity(HonorActivityData activity, Action<HonorActivityData> callback)
        {
            activityData = activity;
            activityCallback = callback;
            isBuilding = false;

            SetupUI();
            SetupButton();
        }

        private void SetupUI()
        {
            if (isBuilding && buildingData != null)
            {
                if (nameText != null)
                    nameText.text = buildingData.name;

                if (descriptionText != null)
                    descriptionText.text = buildingData.description;

                if (costText != null)
                    costText.text = FormatNumber(buildingData.cost) + " 쌀";

                if (effectText != null)
                    effectText.text = $"+{FormatNumber(buildingData.honorPerSecond)} 명예/초";

                if (buttonText != null)
                    buttonText.text = "건설하기";
            }
            else if (!isBuilding && activityData != null)
            {
                if (nameText != null)
                    nameText.text = activityData.name;

                if (descriptionText != null)
                    descriptionText.text = activityData.description;

                if (costText != null)
                    costText.text = FormatNumber(activityData.cost) + " 쌀";

                if (effectText != null)
                    effectText.text = $"+{FormatNumber(activityData.honorReward)} 명예";

                if (buttonText != null)
                    buttonText.text = "실행하기";
            }
        }

        private void SetupButton()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnButtonClicked()
        {
            if (!isAffordable) return;

            if (isBuilding && buildingData != null)
            {
                buildingCallback?.Invoke(buildingData);
            }
            else if (!isBuilding && activityData != null)
            {
                activityCallback?.Invoke(activityData);
            }
        }

        public void RefreshAffordability(double currentRice)
        {
            double requiredCost = isBuilding ? buildingData?.cost ?? 0 : activityData?.cost ?? 0;
            isAffordable = currentRice >= requiredCost;

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (actionButton == null) return;

            // Update button interactability
            actionButton.interactable = isAffordable;

            // Update cost text color
            if (costText != null)
            {
                costText.color = isAffordable ? affordableColor : unaffordableColor;
            }

            // Update button color
            var buttonColors = actionButton.colors;
            buttonColors.normalColor = isAffordable ? affordableColor : unaffordableColor;
            buttonColors.highlightedColor = Color.Lerp(buttonColors.normalColor, Color.white, 0.1f);
            actionButton.colors = buttonColors;
        }

        private string FormatNumber(double number)
        {
            if (number < 1000)
                return number.ToString("F0");
            else if (number < 1000000)
                return (number / 1000).ToString("F1") + "K";
            else if (number < 1000000000)
                return (number / 1000000).ToString("F1") + "M";
            else
                return (number / 1000000000).ToString("F1") + "B";
        }

        private void OnDestroy()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
            }
        }
    }
}