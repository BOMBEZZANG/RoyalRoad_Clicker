using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RoyalRoadClicker.UI.Views
{
    public class ShopPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform iapContainer;
        [SerializeField] private Transform adsContainer;
        [SerializeField] private GameObject shopItemPrefab;

        [Header("Section Headers")]
        [SerializeField] private TextMeshProUGUI iapHeader;
        [SerializeField] private TextMeshProUGUI adsHeader;

        [Header("Coming Soon")]
        [SerializeField] private GameObject comingSoonPanel;
        [SerializeField] private TextMeshProUGUI comingSoonText;

        private bool isInitialized = false;

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

            SetupHeaders();
            ShowComingSoon();

            isInitialized = true;
        }

        private void SetupHeaders()
        {
            if (iapHeader != null)
                iapHeader.text = "프리미엄 패키지 (Premium Packages)";

            if (adsHeader != null)
                adsHeader.text = "광고 보상 (Ad Rewards)";
        }

        private void ShowComingSoon()
        {
            if (comingSoonPanel != null)
            {
                comingSoonPanel.SetActive(true);
            }

            if (comingSoonText != null)
            {
                comingSoonText.text = "상점 기능은 곧 추가될 예정입니다!\n\n" +
                                     "Shop features coming soon!\n\n" +
                                     "• 프리미엄 패키지\n" +
                                     "• 광고 보상\n" +
                                     "• 특별 할인\n" +
                                     "• 시즌 패스";
            }

            // Hide actual shop content for now
            if (scrollRect != null)
                scrollRect.gameObject.SetActive(false);
        }

        public void RefreshDisplay()
        {
            // Placeholder for when shop is implemented
            Debug.Log("Shop display refreshed");
        }

        #region Placeholder Methods for Future Implementation
        
        public void PurchaseRemoveAds()
        {
            Debug.Log("Remove Ads purchase requested");
            // Implement IAP for removing ads
        }

        public void PurchaseStarterPack()
        {
            Debug.Log("Starter Pack purchase requested");
            // Implement starter pack IAP
        }

        public void WatchAdForBonus()
        {
            Debug.Log("Watch ad for bonus requested");
            // Implement rewarded ad
        }

        public void WatchAdForResources()
        {
            Debug.Log("Watch ad for resources requested");
            // Implement rewarded ad for rice/honor
        }

        #endregion

        #region Test Methods (Remove in production)
        
        [ContextMenu("Test IAP")]
        public void TestIAP()
        {
            Debug.Log("Test IAP triggered");
        }

        [ContextMenu("Test Rewarded Ad")]
        public void TestRewardedAd()
        {
            Debug.Log("Test Rewarded Ad triggered");
        }

        #endregion
    }
}