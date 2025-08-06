using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.UI.Views
{
    public class ClassNodeUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI classNameText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject currentIndicator;
        [SerializeField] private GameObject completedIndicator;
        [SerializeField] private GameObject lockedIndicator;

        [Header("Visual Settings")]
        [SerializeField] private Color currentClassColor = Color.yellow;
        [SerializeField] private Color completedClassColor = Color.green;
        [SerializeField] private Color availableClassColor = Color.white;
        [SerializeField] private Color lockedClassColor = Color.gray;

        private PlayerClass nodeClass;
        private ClassRequirements classRequirements;
        private bool isInitialized = false;

        public PlayerClass NodeClass => nodeClass;

        public void Initialize(PlayerClass playerClass, string className, ClassRequirements requirements)
        {
            nodeClass = playerClass;
            classRequirements = requirements;

            if (classNameText != null)
            {
                classNameText.text = className;
            }

            isInitialized = true;
            RefreshState(PlayerClass.Slave); // Default state
        }

        public void RefreshState(PlayerClass currentPlayerClass)
        {
            if (!isInitialized) return;

            ClassNodeState state = DetermineNodeState(currentPlayerClass);
            ApplyVisualState(state);
        }

        private ClassNodeState DetermineNodeState(PlayerClass currentPlayerClass)
        {
            if (nodeClass == currentPlayerClass)
            {
                return ClassNodeState.Current;
            }
            else if (nodeClass < currentPlayerClass)
            {
                return ClassNodeState.Completed;
            }
            else if (nodeClass == currentPlayerClass + 1)
            {
                // This is the next available class
                return ClassNodeState.Available;
            }
            else
            {
                // Future class, not yet accessible
                return ClassNodeState.Locked;
            }
        }

        private void ApplyVisualState(ClassNodeState state)
        {
            // Reset all indicators
            if (currentIndicator != null) currentIndicator.SetActive(false);
            if (completedIndicator != null) completedIndicator.SetActive(false);
            if (lockedIndicator != null) lockedIndicator.SetActive(false);

            Color backgroundColor = lockedClassColor;
            float alpha = 0.5f;

            switch (state)
            {
                case ClassNodeState.Current:
                    backgroundColor = currentClassColor;
                    alpha = 1f;
                    if (currentIndicator != null) currentIndicator.SetActive(true);
                    break;

                case ClassNodeState.Completed:
                    backgroundColor = completedClassColor;
                    alpha = 1f;
                    if (completedIndicator != null) completedIndicator.SetActive(true);
                    break;

                case ClassNodeState.Available:
                    backgroundColor = availableClassColor;
                    alpha = 0.8f;
                    break;

                case ClassNodeState.Locked:
                    backgroundColor = lockedClassColor;
                    alpha = 0.3f;
                    if (lockedIndicator != null) lockedIndicator.SetActive(true);
                    break;
            }

            // Apply background color
            if (backgroundImage != null)
            {
                Color bgColor = backgroundColor;
                bgColor.a = alpha;
                backgroundImage.color = bgColor;
            }

            // Apply text color
            if (classNameText != null)
            {
                Color textColor = state == ClassNodeState.Locked ? Color.gray : Color.white;
                classNameText.color = textColor;
            }

            // Apply icon color/alpha
            if (iconImage != null)
            {
                Color iconColor = iconImage.color;
                iconColor.a = alpha;
                iconImage.color = iconColor;
            }
        }

        public void ShowRequirementTooltip()
        {
            if (classRequirements == null) return;

            var requirement = classRequirements.GetRequirementForClass(nodeClass);
            if (requirement == null) return;

            // In a full implementation, this would show a tooltip
            Debug.Log($"{requirement.className} Requirements:\n" +
                     $"Honor Required: {requirement.honorRequired}\n" +
                     $"Min Rice/s: {requirement.minimumRicePerSecond}");
        }

        private void OnMouseEnter()
        {
            // Show tooltip on hover
            ShowRequirementTooltip();
        }

        public void SetCustomIcon(Sprite icon)
        {
            if (iconImage != null)
            {
                iconImage.sprite = icon;
            }
        }

        public void PlayUnlockAnimation()
        {
            // Add animation when a class becomes available
            if (backgroundImage != null)
            {
                // Simple pulse animation using Unity's built-in system
                StartCoroutine(PulseAnimation());
            }
        }

        private System.Collections.IEnumerator PulseAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.1f;
            float duration = 0.2f;
            
            // Scale up
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Scale down
            elapsed = 0f;
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
    }

    public enum ClassNodeState
    {
        Current,    // Player's current class
        Completed,  // Classes player has already passed
        Available,  // Next class that can be unlocked
        Locked      // Future classes not yet accessible
    }
}