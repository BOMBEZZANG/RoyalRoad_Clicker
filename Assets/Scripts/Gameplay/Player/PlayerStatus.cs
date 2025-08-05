using System;
using UnityEngine;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.Gameplay.Player
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private ClassRequirements classRequirements;
        private PlayerPresenter playerPresenter;
        
        [Header("Validation Settings")]
        [SerializeField] private double maxResourceCap = 1e15; // Quadrillion cap
        [SerializeField] private float maxProductionRate = 1e12f; // Trillion per second cap
        
        public event Action<string> OnValidationError;
        public event Action<PlayerClass, ClassRequirements.ClassRequirement> OnClassAscensionAvailable;

        private void Awake()
        {
            playerPresenter = GetComponent<PlayerPresenter>();
            if (playerPresenter == null)
            {
                Debug.LogError("PlayerPresenter not found on the same GameObject!");
            }

            if (classRequirements == null)
            {
                Debug.LogError("ClassRequirements ScriptableObject not assigned!");
            }
        }

        public bool ValidateResourceAmount(double amount, string resourceName)
        {
            if (amount < 0)
            {
                OnValidationError?.Invoke($"{resourceName} cannot be negative!");
                return false;
            }

            if (amount > maxResourceCap)
            {
                OnValidationError?.Invoke($"{resourceName} has reached the maximum cap!");
                return false;
            }

            if (double.IsNaN(amount) || double.IsInfinity(amount))
            {
                OnValidationError?.Invoke($"Invalid {resourceName} value detected!");
                return false;
            }

            return true;
        }

        public bool ValidateProductionRate(double rate, string productionType)
        {
            if (rate < 0)
            {
                OnValidationError?.Invoke($"{productionType} cannot be negative!");
                return false;
            }

            if (rate > maxProductionRate)
            {
                OnValidationError?.Invoke($"{productionType} exceeds maximum allowed rate!");
                return false;
            }

            return true;
        }

        public bool CanAscendToNextClass()
        {
            if (playerPresenter == null || classRequirements == null)
                return false;

            var currentClass = playerPresenter.CurrentClass;
            if (currentClass >= PlayerClass.King)
                return false;

            var nextRequirement = classRequirements.GetNextClassRequirement(currentClass);
            if (nextRequirement == null)
                return false;

            return classRequirements.CanAscendToClass(
                nextRequirement.targetClass,
                playerPresenter.Honor,
                playerPresenter.RicePerSecond
            );
        }

        public ClassRequirements.ClassRequirement GetNextClassRequirement()
        {
            if (playerPresenter == null || classRequirements == null)
                return null;

            return classRequirements.GetNextClassRequirement(playerPresenter.CurrentClass);
        }

        public float GetCurrentRiceMultiplier()
        {
            if (playerPresenter == null || classRequirements == null)
                return 1f;

            return classRequirements.GetTotalMultiplier(playerPresenter.CurrentClass, "rice");
        }

        public float GetCurrentHonorMultiplier()
        {
            if (playerPresenter == null || classRequirements == null)
                return 1f;

            return classRequirements.GetTotalMultiplier(playerPresenter.CurrentClass, "honor");
        }

        public float GetCurrentTapMultiplier()
        {
            if (playerPresenter == null || classRequirements == null)
                return 1f;

            return classRequirements.GetTotalMultiplier(playerPresenter.CurrentClass, "tap");
        }

        public double GetProgressToNextClass()
        {
            var nextReq = GetNextClassRequirement();
            if (nextReq == null)
                return 1.0; // Already at max class

            if (playerPresenter == null)
                return 0.0;

            return Math.Min(1.0, playerPresenter.Honor / nextReq.honorRequired);
        }

        public string GetFormattedTimeToNextClass()
        {
            var nextReq = GetNextClassRequirement();
            if (nextReq == null || playerPresenter == null)
                return "---";

            if (playerPresenter.HonorPerSecond <= 0)
                return "∞";

            double honorNeeded = Math.Max(0, nextReq.honorRequired - playerPresenter.Honor);
            double secondsNeeded = honorNeeded / playerPresenter.HonorPerSecond;

            if (secondsNeeded <= 0)
                return "Ready!";

            return FormatTime(secondsNeeded);
        }

        private string FormatTime(double seconds)
        {
            if (seconds < 60)
                return $"{seconds:F0}s";
            else if (seconds < 3600)
                return $"{seconds / 60:F0}m {seconds % 60:F0}s";
            else if (seconds < 86400)
                return $"{seconds / 3600:F0}h {(seconds % 3600) / 60:F0}m";
            else
                return $"{seconds / 86400:F0}d {(seconds % 86400) / 3600:F0}h";
        }

        private void Update()
        {
            CheckClassAscensionAvailability();
        }

        private float checkTimer = 0f;
        private void CheckClassAscensionAvailability()
        {
            checkTimer += Time.deltaTime;
            if (checkTimer < 1f) // Check once per second
                return;

            checkTimer = 0f;

            if (CanAscendToNextClass())
            {
                var nextReq = GetNextClassRequirement();
                if (nextReq != null)
                {
                    OnClassAscensionAvailable?.Invoke(nextReq.targetClass, nextReq);
                }
            }
        }

        public bool TryAscendToNextClass()
        {
            var nextReq = GetNextClassRequirement();
            if (nextReq == null || playerPresenter == null)
                return false;

            if (!CanAscendToNextClass())
            {
                OnValidationError?.Invoke("Requirements not met for class ascension!");
                return false;
            }

            return playerPresenter.TryAscendClass(nextReq.honorRequired);
        }
    }
}