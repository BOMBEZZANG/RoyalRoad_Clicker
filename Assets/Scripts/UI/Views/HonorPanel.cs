using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Core;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.UI.Views
{
    public class HonorPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform buildingsContainer;
        [SerializeField] private Transform activitiesContainer;
        [SerializeField] private GameObject honorItemPrefab;

        [Header("Section Headers")]
        [SerializeField] private TextMeshProUGUI buildingsHeader;
        [SerializeField] private TextMeshProUGUI activitiesHeader;

        [Header("Current Honor Display")]
        [SerializeField] private TextMeshProUGUI currentHonorText;
        [SerializeField] private TextMeshProUGUI honorPerSecondText;

        private bool isInitialized = false;
        private GameManager gameManager;

        // Temporary honor building data - in real implementation, this would come from ScriptableObject
        private HonorBuildingData[] honorBuildings = new HonorBuildingData[]
        {
            new HonorBuildingData { id = "school", name = "서당 건설", description = "학자들을 가르쳐 명예를 얻습니다", cost = 50, honorPerSecond = 0.5 },
            new HonorBuildingData { id = "shrine", name = "사당 건립", description = "조상을 모셔 명예를 높입니다", cost = 200, honorPerSecond = 2.0 },
            new HonorBuildingData { id = "library", name = "도서관 설립", description = "지식을 보급하여 명예를 쌓습니다", cost = 1000, honorPerSecond = 8.0 },
            new HonorBuildingData { id = "academy", name = "서원 창건", description = "교육기관으로 큰 명예를 얻습니다", cost = 5000, honorPerSecond = 25.0 }
        };

        private HonorActivityData[] honorActivities = new HonorActivityData[]
        {
            new HonorActivityData { id = "banquet", name = "연회 개최", description = "귀족들을 초대하여 명예를 얻습니다", cost = 100, honorReward = 20 },
            new HonorActivityData { id = "charity", name = "자선 활동", description = "가난한 이들을 도와 명예를 쌓습니다", cost = 250, honorReward = 60 },
            new HonorActivityData { id = "festival", name = "축제 후원", description = "마을 축제를 후원하여 큰 명예를 얻습니다", cost = 500, honorReward = 150 }
        };

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                RefreshDisplay();
            }
        }

        public void Initialize()
        {
            if (isInitialized) return;

            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }

            SetupHeaders();
            CreateHonorItems();
            RefreshDisplay();

            isInitialized = true;
        }

        private void SetupHeaders()
        {
            if (buildingsHeader != null)
                buildingsHeader.text = "명예 건물 (Honor Buildings)";

            if (activitiesHeader != null)
                activitiesHeader.text = "명예 활동 (Honor Activities)";
        }

        private void CreateHonorItems()
        {
            if (honorItemPrefab == null)
            {
                Debug.LogError("HonorItemPrefab not assigned!");
                return;
            }

            // Create building items
            CreateBuildingItems();
            
            // Create activity items
            CreateActivityItems();
        }

        private void CreateBuildingItems()
        {
            foreach (var building in honorBuildings)
            {
                var itemGO = Instantiate(honorItemPrefab, buildingsContainer);
                var itemUI = itemGO.GetComponent<HonorItemUI>();
                
                if (itemUI != null)
                {
                    itemUI.InitializeAsBuilding(building, OnBuildingPurchased);
                }
            }
        }

        private void CreateActivityItems()
        {
            foreach (var activity in honorActivities)
            {
                var itemGO = Instantiate(honorItemPrefab, activitiesContainer);
                var itemUI = itemGO.GetComponent<HonorItemUI>();
                
                if (itemUI != null)
                {
                    itemUI.InitializeAsActivity(activity, OnActivityPerformed);
                }
            }
        }

        private void OnBuildingPurchased(HonorBuildingData building)
        {
            if (gameManager?.PlayerModel == null) return;

            double cost = building.cost;
            if (gameManager.PlayerModel.SpendRice(cost))
            {
                // Add to honor per second
                double newHonorPerSecond = gameManager.PlayerModel.HonorPerSecond + building.honorPerSecond;
                gameManager.PlayerModel.UpdateHonorPerSecond(newHonorPerSecond);

                Debug.Log($"Purchased {building.name}! +{building.honorPerSecond} Honor/s");
                
                // Trigger purchase event
                var purchaseEvent = new HonorBuildingPurchasedEvent
                {
                    BuildingId = building.id,
                    Cost = cost,
                    HonorPerSecondAdded = building.honorPerSecond
                };
                
                EventManager.Trigger(purchaseEvent);
                RefreshDisplay();
            }
            else
            {
                Debug.Log($"Cannot afford {building.name}. Cost: {cost}, Current Rice: {gameManager.PlayerModel.Rice}");
            }
        }

        private void OnActivityPerformed(HonorActivityData activity)
        {
            if (gameManager?.PlayerModel == null) return;

            double cost = activity.cost;
            if (gameManager.PlayerModel.SpendRice(cost))
            {
                // Add honor immediately
                gameManager.PlayerModel.AddHonor(activity.honorReward);

                Debug.Log($"Performed {activity.name}! +{activity.honorReward} Honor");
                
                // Trigger activity event
                var activityEvent = new HonorActivityPerformedEvent
                {
                    ActivityId = activity.id,
                    Cost = cost,
                    HonorGained = activity.honorReward
                };
                
                EventManager.Trigger(activityEvent);
                RefreshDisplay();
            }
            else
            {
                Debug.Log($"Cannot afford {activity.name}. Cost: {cost}, Current Rice: {gameManager.PlayerModel.Rice}");
            }
        }

        private void RefreshDisplay()
        {
            if (gameManager?.PlayerModel == null) return;

            // Update current honor display
            if (currentHonorText != null)
            {
                currentHonorText.text = $"현재 명예: {FormatNumber(gameManager.PlayerModel.Honor)}";
            }

            if (honorPerSecondText != null)
            {
                honorPerSecondText.text = $"명예 생산: {FormatNumber(gameManager.PlayerModel.HonorPerSecond)}/초";
            }

            // Refresh all honor items
            RefreshHonorItems();
        }

        private void RefreshHonorItems()
        {
            var honorItems = GetComponentsInChildren<HonorItemUI>();
            double currentRice = gameManager?.PlayerModel?.Rice ?? 0;

            foreach (var item in honorItems)
            {
                item.RefreshAffordability(currentRice);
            }
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

        private void Update()
        {
            // Refresh display periodically
            if (Time.time % 1.0f < Time.deltaTime)
            {
                RefreshDisplay();
            }
        }
    }

    #region Data Classes for Honor System
    [System.Serializable]
    public class HonorBuildingData
    {
        public string id;
        public string name;
        public string description;
        public double cost;
        public double honorPerSecond;
    }

    [System.Serializable]
    public class HonorActivityData
    {
        public string id;
        public string name;
        public string description;
        public double cost;
        public double honorReward;
    }

    public struct HonorBuildingPurchasedEvent
    {
        public string BuildingId;
        public double Cost;
        public double HonorPerSecondAdded;
        public System.DateTime Timestamp;
    }

    public struct HonorActivityPerformedEvent
    {
        public string ActivityId;
        public double Cost;
        public double HonorGained;
        public System.DateTime Timestamp;
    }
    #endregion
}