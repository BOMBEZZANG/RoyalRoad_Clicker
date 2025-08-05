using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.Gameplay.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Resource Displays")]
        [SerializeField] private TextMeshProUGUI riceText;
        [SerializeField] private TextMeshProUGUI honorText;
        [SerializeField] private TextMeshProUGUI ricePerSecondText;
        [SerializeField] private TextMeshProUGUI honorPerSecondText;
        [SerializeField] private TextMeshProUGUI ricePerTapText;
        [SerializeField] private TextMeshProUGUI kokuText;

        [Header("Class Display")]
        [SerializeField] private TextMeshProUGUI classNameText;
        [SerializeField] private Image characterImage;
        [SerializeField] private Image backgroundImage;

        [Header("Character Sprites")]
        [SerializeField] private Sprite slaveSprite;
        [SerializeField] private Sprite tenantFarmerSprite;
        [SerializeField] private Sprite commonerSprite;
        [SerializeField] private Sprite nobleSprite;
        [SerializeField] private Sprite lordSprite;
        [SerializeField] private Sprite kingSprite;

        [Header("Background Sprites")]
        [SerializeField] private Sprite slaveBackground;
        [SerializeField] private Sprite tenantFarmerBackground;
        [SerializeField] private Sprite commonerBackground;
        [SerializeField] private Sprite nobleBackground;
        [SerializeField] private Sprite lordBackground;
        [SerializeField] private Sprite kingBackground;

        [Header("Formatting")]
        [SerializeField] private string riceFormat = "{0:N0} 쌀";
        [SerializeField] private string honorFormat = "{0:N0} 명예";
        [SerializeField] private string perSecondFormat = "{0:N1}/초";
        [SerializeField] private string perTapFormat = "{0:N0}/탭";
        [SerializeField] private string kokuFormat = "{0:N0} 석고";

        private readonly string[] classNames = new string[]
        {
            "노비",
            "소작농",
            "평민",
            "양반",
            "영주",
            "왕"
        };

        public void UpdateRiceDisplay(double rice)
        {
            if (riceText != null)
            {
                riceText.text = string.Format(riceFormat, rice);
            }
        }

        public void UpdateHonorDisplay(double honor)
        {
            if (honorText != null)
            {
                honorText.text = string.Format(honorFormat, honor);
            }
        }

        public void UpdateRicePerSecondDisplay(double ricePerSecond)
        {
            if (ricePerSecondText != null)
            {
                ricePerSecondText.text = string.Format(perSecondFormat, ricePerSecond);
                Debug.Log($"Updated Rice/s display: {ricePerSecond}");
            }
            else
            {
                Debug.LogWarning("ricePerSecondText is null!");
            }
        }

        public void UpdateHonorPerSecondDisplay(double honorPerSecond)
        {
            if (honorPerSecondText != null)
            {
                honorPerSecondText.text = string.Format(perSecondFormat, honorPerSecond);
            }
        }

        public void UpdateRicePerTapDisplay(double ricePerTap)
        {
            if (ricePerTapText != null)
            {
                ricePerTapText.text = string.Format(perTapFormat, ricePerTap);
            }
        }

        public void UpdateKokuDisplay(double koku)
        {
            if (kokuText != null)
            {
                if (koku > 0)
                {
                    kokuText.text = string.Format(kokuFormat, koku);
                    kokuText.gameObject.SetActive(true);
                }
                else
                {
                    kokuText.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateClassDisplay(PlayerClass playerClass)
        {
            if (classNameText != null)
            {
                int classIndex = (int)playerClass;
                if (classIndex >= 0 && classIndex < classNames.Length)
                {
                    classNameText.text = classNames[classIndex];
                }
            }

            UpdateCharacterVisual(playerClass);
            UpdateBackgroundVisual(playerClass);
        }

        private void UpdateCharacterVisual(PlayerClass playerClass)
        {
            if (characterImage == null) return;

            Sprite newSprite = null;
            switch (playerClass)
            {
                case PlayerClass.Slave:
                    newSprite = slaveSprite;
                    break;
                case PlayerClass.TenantFarmer:
                    newSprite = tenantFarmerSprite;
                    break;
                case PlayerClass.Commoner:
                    newSprite = commonerSprite;
                    break;
                case PlayerClass.Noble:
                    newSprite = nobleSprite;
                    break;
                case PlayerClass.Lord:
                    newSprite = lordSprite;
                    break;
                case PlayerClass.King:
                    newSprite = kingSprite;
                    break;
            }

            if (newSprite != null)
            {
                characterImage.sprite = newSprite;
            }
        }

        private void UpdateBackgroundVisual(PlayerClass playerClass)
        {
            if (backgroundImage == null) return;

            Sprite newBackground = null;
            switch (playerClass)
            {
                case PlayerClass.Slave:
                    newBackground = slaveBackground;
                    break;
                case PlayerClass.TenantFarmer:
                    newBackground = tenantFarmerBackground;
                    break;
                case PlayerClass.Commoner:
                    newBackground = commonerBackground;
                    break;
                case PlayerClass.Noble:
                    newBackground = nobleBackground;
                    break;
                case PlayerClass.Lord:
                    newBackground = lordBackground;
                    break;
                case PlayerClass.King:
                    newBackground = kingBackground;
                    break;
            }

            if (newBackground != null)
            {
                backgroundImage.sprite = newBackground;
            }
        }

        public string FormatLargeNumber(double number)
        {
            if (number < 1000)
                return number.ToString("N0");
            else if (number < 1000000)
                return (number / 1000).ToString("N1") + "K";
            else if (number < 1000000000)
                return (number / 1000000).ToString("N1") + "M";
            else if (number < 1000000000000)
                return (number / 1000000000).ToString("N1") + "B";
            else if (number < 1000000000000000)
                return (number / 1000000000000).ToString("N1") + "T";
            else
                return (number / 1000000000000000).ToString("N1") + "Q";
        }

        public void ShowTapFeedback(Vector3 worldPosition)
        {
            // This method can be implemented to show visual feedback when tapping
            // For example, spawning a "+X Rice" text that floats up and fades
        }

        public void PlayClassAscensionAnimation()
        {
            // This method can be implemented to play a special animation
            // when the player ascends to a new class
        }
    }
}