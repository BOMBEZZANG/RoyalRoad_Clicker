using System;
using UnityEngine;
using UnityEngine.UI;
using RoyalRoadClicker.Core;

namespace RoyalRoadClicker.UI.Presenters
{
    public enum TabType
    {
        Upgrade = 0,
        Honor = 1,
        Class = 2,
        Shop = 3
    }

    public class BottomNavPresenter : MonoBehaviour
    {
        [Header("Tab Buttons")]
        [SerializeField] private Button upgradeTabButton;
        [SerializeField] private Button honorTabButton;
        [SerializeField] private Button classTabButton;
        [SerializeField] private Button shopTabButton;

        [Header("Content Panels")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private GameObject honorPanel;
        [SerializeField] private GameObject classPanel;
        [SerializeField] private GameObject shopPanel;

        [Header("Tab Visual Settings")]
        [SerializeField] private Color activeTabColor = Color.white;
        [SerializeField] private Color inactiveTabColor = Color.gray;
        [SerializeField] private float tabTransitionSpeed = 0.2f;

        private TabType currentTab = TabType.Upgrade;
        private Button[] tabButtons;
        private GameObject[] contentPanels;

        public event Action<TabType> OnTabChanged;
        public TabType CurrentTab => currentTab;

        private void Awake()
        {
            InitializeArrays();
            SetupButtonListeners();
        }

        private void Start()
        {
            // Set default tab
            SwitchToTab(TabType.Upgrade);
        }

        private void InitializeArrays()
        {
            tabButtons = new Button[]
            {
                upgradeTabButton,
                honorTabButton,
                classTabButton,
                shopTabButton
            };

            contentPanels = new GameObject[]
            {
                upgradePanel,
                honorPanel,
                classPanel,
                shopPanel
            };

            // Validate all references
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (tabButtons[i] == null)
                {
                    Debug.LogError($"Tab button {(TabType)i} is not assigned!");
                }
            }

            for (int i = 0; i < contentPanels.Length; i++)
            {
                if (contentPanels[i] == null)
                {
                    Debug.LogError($"Content panel {(TabType)i} is not assigned!");
                }
            }
        }

        private void SetupButtonListeners()
        {
            if (upgradeTabButton != null)
                upgradeTabButton.onClick.AddListener(() => SwitchToTab(TabType.Upgrade));

            if (honorTabButton != null)
                honorTabButton.onClick.AddListener(() => SwitchToTab(TabType.Honor));

            if (classTabButton != null)
                classTabButton.onClick.AddListener(() => SwitchToTab(TabType.Class));

            if (shopTabButton != null)
                shopTabButton.onClick.AddListener(() => SwitchToTab(TabType.Shop));
        }

        public void SwitchToTab(TabType tabType)
        {
            if (currentTab == tabType) return;

            TabType previousTab = currentTab;
            currentTab = tabType;

            UpdateTabVisuals();
            UpdateContentPanels();

            // Trigger events
            OnTabChanged?.Invoke(tabType);
            
            // Send global event
            EventManager.Trigger(new TabSwitchedEvent(previousTab, currentTab));

            Debug.Log($"Switched from {previousTab} to {currentTab} tab");
        }

        private void UpdateTabVisuals()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (tabButtons[i] == null) continue;

                bool isActive = i == (int)currentTab;
                Color targetColor = isActive ? activeTabColor : inactiveTabColor;
                
                // Update button color
                ColorBlock colors = tabButtons[i].colors;
                colors.normalColor = targetColor;
                colors.selectedColor = targetColor;
                tabButtons[i].colors = colors;

                // Update interactable state (optional - you might want to keep all tabs clickable)
                // tabButtons[i].interactable = !isActive;
            }
        }

        private void UpdateContentPanels()
        {
            for (int i = 0; i < contentPanels.Length; i++)
            {
                if (contentPanels[i] == null) continue;

                bool shouldBeActive = i == (int)currentTab;
                
                if (contentPanels[i].activeSelf != shouldBeActive)
                {
                    contentPanels[i].SetActive(shouldBeActive);
                }
            }
        }

        public bool IsTabUnlocked(TabType tabType)
        {
            // Add logic here to check if tabs should be unlocked based on player progression
            switch (tabType)
            {
                case TabType.Upgrade:
                    return true; // Always available
                
                case TabType.Honor:
                    // Unlock when player reaches Tenant Farmer or has some rice production
                    return GameManager.Instance?.PlayerModel?.CurrentClass >= RoyalRoadClicker.Data.PlayerClass.TenantFarmer 
                           || GameManager.Instance?.PlayerModel?.RicePerSecond > 0;
                
                case TabType.Class:
                    return true; // Always available to see progression
                
                case TabType.Shop:
                    return true; // Always available for monetization
                
                default:
                    return true;
            }
        }

        public void RefreshTabAvailability()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (tabButtons[i] == null) continue;

                TabType tabType = (TabType)i;
                bool isUnlocked = IsTabUnlocked(tabType);
                
                tabButtons[i].interactable = isUnlocked;
                
                // Visual feedback for locked tabs
                if (!isUnlocked)
                {
                    ColorBlock colors = tabButtons[i].colors;
                    colors.normalColor = Color.black;
                    colors.disabledColor = Color.black;
                    tabButtons[i].colors = colors;
                }
            }
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (upgradeTabButton != null)
                upgradeTabButton.onClick.RemoveAllListeners();

            if (honorTabButton != null)
                honorTabButton.onClick.RemoveAllListeners();

            if (classTabButton != null)
                classTabButton.onClick.RemoveAllListeners();

            if (shopTabButton != null)
                shopTabButton.onClick.RemoveAllListeners();
        }

        #region Public API for external access
        public void ForceRefreshCurrentTab()
        {
            UpdateTabVisuals();
            UpdateContentPanels();
            OnTabChanged?.Invoke(currentTab);
        }

        public void EnableTab(TabType tabType, bool enabled)
        {
            int index = (int)tabType;
            if (index >= 0 && index < tabButtons.Length && tabButtons[index] != null)
            {
                tabButtons[index].interactable = enabled;
            }
        }
        #endregion
    }

    #region Event Definitions
    public struct TabSwitchedEvent
    {
        public TabType PreviousTab;
        public TabType CurrentTab;
        public System.DateTime Timestamp;

        public TabSwitchedEvent(TabType previous, TabType current)
        {
            PreviousTab = previous;
            CurrentTab = current;
            Timestamp = System.DateTime.Now;
        }
    }
    #endregion
}