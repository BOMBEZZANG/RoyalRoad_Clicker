using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Core;
using RoyalRoadClicker.Data;
using RoyalRoadClicker.Gameplay.Player;

namespace RoyalRoadClicker.UI.Views
{
    public class ClassPanel : MonoBehaviour
    {
        [Header("Current Status")]
        [SerializeField] private TextMeshProUGUI currentClassText;
        [SerializeField] private TextMeshProUGUI currentHonorText;
        [SerializeField] private Image currentClassIcon;

        [Header("Next Class Info")]
        [SerializeField] private TextMeshProUGUI nextClassText;
        [SerializeField] private TextMeshProUGUI requiredHonorText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI timeToAscensionText;

        [Header("Ascension Button")]
        [SerializeField] private Button ascendButton;
        [SerializeField] private TextMeshProUGUI ascendButtonText;

        [Header("Class Tree")]
        [SerializeField] private Transform classTreeContainer;
        [SerializeField] private GameObject classNodePrefab;

        [Header("Ascension Benefits")]
        [SerializeField] private TextMeshProUGUI benefitsText;
        [SerializeField] private ScrollRect benefitsScrollRect;

        [Header("Data")]
        [SerializeField] private ClassRequirements classRequirements;

        private GameManager gameManager;
        private PlayerStatus playerStatus;
        private bool isInitialized = false;

        private readonly string[] classNames = new string[]
        {
            "노비 (Slave)",
            "소작농 (Tenant Farmer)", 
            "평민 (Commoner)",
            "양반 (Noble)",
            "영주 (Lord)",
            "왕 (King)"
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

            // Find PlayerStatus component
            playerStatus = FindObjectOfType<PlayerStatus>();
            if (playerStatus == null)
            {
                Debug.LogError("PlayerStatus not found!");
                return;
            }

            // Get class requirements if not assigned
            if (classRequirements == null)
            {
                // Try to find it from PlayerStatus
                classRequirements = playerStatus.GetComponent<PlayerStatus>()?.GetType()
                    .GetField("classRequirements", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(playerStatus) as ClassRequirements;
            }

            SetupAscendButton();
            CreateClassTree();
            RefreshDisplay();

            isInitialized = true;
        }

        private void SetupAscendButton()
        {
            if (ascendButton != null)
            {
                ascendButton.onClick.RemoveAllListeners();
                ascendButton.onClick.AddListener(OnAscendButtonClicked);
            }

            if (ascendButtonText != null)
            {
                ascendButtonText.text = "승급하기";
            }
        }

        private void CreateClassTree()
        {
            if (classTreeContainer == null || classNodePrefab == null) return;

            // Clear existing nodes
            foreach (Transform child in classTreeContainer)
            {
                DestroyImmediate(child.gameObject);
            }

            // Create nodes for each class
            for (int i = 0; i < classNames.Length; i++)
            {
                var nodeGO = Instantiate(classNodePrefab, classTreeContainer);
                var nodeUI = nodeGO.GetComponent<ClassNodeUI>();
                
                if (nodeUI != null)
                {
                    PlayerClass playerClass = (PlayerClass)i;
                    nodeUI.Initialize(playerClass, classNames[i], classRequirements);
                }
            }
        }

        private void OnAscendButtonClicked()
        {
            if (playerStatus == null) return;

            bool success = playerStatus.TryAscendToNextClass();
            if (success)
            {
                // Play ascension effects, sounds, etc.
                PlayAscensionEffect();
                RefreshDisplay();
                Debug.Log("Class ascension successful!");
            }
            else
            {
                Debug.Log("Cannot ascend - requirements not met");
            }
        }

        private void PlayAscensionEffect()
        {
            // Add visual/audio feedback for successful ascension
            Debug.Log("🎉 Class Ascension Complete! 🎉");
        }

        private void RefreshDisplay()
        {
            if (gameManager?.PlayerModel == null) return;

            var playerModel = gameManager.PlayerModel;
            var currentClass = playerModel.CurrentClass;
            var currentHonor = playerModel.Honor;

            // Update current status
            UpdateCurrentStatus(currentClass, currentHonor);
            
            // Update next class information
            UpdateNextClassInfo(currentClass, currentHonor);
            
            // Update ascend button
            UpdateAscendButton();
            
            // Update class tree
            UpdateClassTree();
            
            // Update benefits text
            UpdateBenefitsText();
        }

        private void UpdateCurrentStatus(PlayerClass currentClass, double currentHonor)
        {
            if (currentClassText != null)
            {
                currentClassText.text = $"현재 신분: {classNames[(int)currentClass]}";
            }

            if (currentHonorText != null)
            {
                currentHonorText.text = $"보유 명예: {FormatNumber(currentHonor)}";
            }
        }

        private void UpdateNextClassInfo(PlayerClass currentClass, double currentHonor)
        {
            if (currentClass >= PlayerClass.King)
            {
                // Already at max class
                if (nextClassText != null)
                    nextClassText.text = "최고 신분 달성!";

                if (requiredHonorText != null)
                    requiredHonorText.text = "---";

                if (progressSlider != null)
                    progressSlider.value = 1f;

                if (progressText != null)
                    progressText.text = "100%";

                if (timeToAscensionText != null)
                    timeToAscensionText.text = "---";

                return;
            }

            var nextRequirement = classRequirements?.GetNextClassRequirement(currentClass);
            if (nextRequirement == null) return;

            // Update next class info
            if (nextClassText != null)
            {
                nextClassText.text = $"다음 신분: {nextRequirement.className}";
            }

            if (requiredHonorText != null)
            {
                requiredHonorText.text = $"필요 명예: {FormatNumber(nextRequirement.honorRequired)}";
            }

            // Update progress
            double progress = playerStatus?.GetProgressToNextClass() ?? 0;
            if (progressSlider != null)
            {
                progressSlider.value = (float)progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{(progress * 100):F1}%";
            }

            // Update time to ascension
            if (timeToAscensionText != null)
            {
                string timeText = playerStatus?.GetFormattedTimeToNextClass() ?? "---";
                timeToAscensionText.text = $"예상 시간: {timeText}";
            }
        }

        private void UpdateAscendButton()
        {
            if (ascendButton == null) return;

            bool canAscend = playerStatus?.CanAscendToNextClass() ?? false;
            bool isMaxClass = gameManager?.PlayerModel?.CurrentClass >= PlayerClass.King;

            ascendButton.interactable = canAscend && !isMaxClass;

            // Update button color based on state
            var buttonColors = ascendButton.colors;
            if (isMaxClass)
            {
                buttonColors.normalColor = Color.yellow;
                if (ascendButtonText != null)
                    ascendButtonText.text = "최고 신분";
            }
            else if (canAscend)
            {
                buttonColors.normalColor = Color.green;
                if (ascendButtonText != null)
                    ascendButtonText.text = "승급하기";
            }
            else
            {
                buttonColors.normalColor = Color.gray;
                if (ascendButtonText != null)
                    ascendButtonText.text = "요건 부족";
            }

            ascendButton.colors = buttonColors;
        }

        private void UpdateClassTree()
        {
            var classNodes = GetComponentsInChildren<ClassNodeUI>();
            var currentClass = gameManager?.PlayerModel?.CurrentClass ?? PlayerClass.Slave;

            foreach (var node in classNodes)
            {
                node.RefreshState(currentClass);
            }
        }

        private void UpdateBenefitsText()
        {
            if (benefitsText == null) return;

            var currentClass = gameManager?.PlayerModel?.CurrentClass ?? PlayerClass.Slave;
            var nextRequirement = classRequirements?.GetNextClassRequirement(currentClass);

            if (nextRequirement == null)
            {
                benefitsText.text = "최고 신분에 도달했습니다!";
                return;
            }

            string benefits = $"<b>{nextRequirement.className} 승급 혜택:</b>\n\n";
            benefits += $"• 쌀 생산량 {nextRequirement.riceMultiplier:F1}배 증가\n";
            benefits += $"• 명예 생산량 {nextRequirement.honorMultiplier:F1}배 증가\n";
            benefits += $"• 탭 효율 {nextRequirement.tapMultiplier:F1}배 증가\n\n";

            if (nextRequirement.unlockedFeatures != null && nextRequirement.unlockedFeatures.Length > 0)
            {
                benefits += "<b>해금 기능:</b>\n";
                foreach (var feature in nextRequirement.unlockedFeatures)
                {
                    benefits += $"• {feature}\n";
                }
            }

            benefitsText.text = benefits;
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
            if (Time.time % 0.5f < Time.deltaTime)
            {
                RefreshDisplay();
            }
        }
    }
}