using UnityEngine;
using UnityEngine.UI;
using RoyalRoadClicker.Core;
using RoyalRoadClicker.Gameplay.Player;

namespace RoyalRoadClicker.UI
{
    public class TestUIController : MonoBehaviour
    {
        [Header("Test Buttons")]
        [SerializeField] private Button addRiceButton;
        [SerializeField] private Button addHonorButton;
        [SerializeField] private Button buyUpgradeButton;
        [SerializeField] private Button setProductionButton;
        [SerializeField] private Button resetButton;

        private PlayerPresenter playerPresenter;
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;
            playerPresenter = FindObjectOfType<PlayerPresenter>();

            if (playerPresenter == null)
            {
                Debug.LogError("PlayerPresenter not found!");
            }

            SetupButtons();
        }

        private void SetupButtons()
        {
            if (addRiceButton != null)
                addRiceButton.onClick.AddListener(AddTestRice);

            if (addHonorButton != null)
                addHonorButton.onClick.AddListener(AddTestHonor);

            if (buyUpgradeButton != null)
                buyUpgradeButton.onClick.AddListener(BuyTestUpgrade);

            if (setProductionButton != null)
                setProductionButton.onClick.AddListener(SetProductionRates);

            if (resetButton != null)
                resetButton.onClick.AddListener(ResetGame);
        }

        public void AddTestRice()
        {
            if (gameManager != null && gameManager.PlayerModel != null)
            {
                gameManager.PlayerModel.AddRice(100);
                Debug.Log("Added 100 Rice via button");
            }
        }

        public void AddTestHonor()
        {
            if (gameManager != null && gameManager.PlayerModel != null)
            {
                gameManager.PlayerModel.AddHonor(50);
                Debug.Log("Added 50 Honor via button");
            }
        }

        public void BuyTestUpgrade()
        {
            if (playerPresenter != null)
            {
                // First give some rice if needed
                if (playerPresenter.Rice < 10)
                {
                    if (gameManager != null && gameManager.PlayerModel != null)
                    {
                        gameManager.PlayerModel.AddRice(50);
                    }
                }

                // Buy upgrade: Cost 10 rice, +1 rice/s, +0.5 rice/tap
                bool success = playerPresenter.PurchaseRiceUpgrade(10, 1, 0.5);
                Debug.Log($"Upgrade purchase: {success}");
            }
        }

        public void SetProductionRates()
        {
            if (gameManager != null && gameManager.PlayerModel != null)
            {
                gameManager.PlayerModel.UpdateRicePerSecond(5);
                gameManager.PlayerModel.UpdateHonorPerSecond(1);
                Debug.Log("Set production rates: Rice/s: 5, Honor/s: 1");
            }
        }

        public void ResetGame()
        {
            if (gameManager != null)
            {
                gameManager.ResetGame();
                Debug.Log("Game reset");
            }
        }

        private void OnDestroy()
        {
            if (addRiceButton != null)
                addRiceButton.onClick.RemoveListener(AddTestRice);

            if (addHonorButton != null)
                addHonorButton.onClick.RemoveListener(AddTestHonor);

            if (buyUpgradeButton != null)
                buyUpgradeButton.onClick.RemoveListener(BuyTestUpgrade);

            if (setProductionButton != null)
                setProductionButton.onClick.RemoveListener(SetProductionRates);

            if (resetButton != null)
                resetButton.onClick.RemoveListener(ResetGame);
        }
    }
}