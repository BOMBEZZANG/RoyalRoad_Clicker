using System;
using UnityEngine;
using RoyalRoadClicker.Data;

namespace RoyalRoadClicker.Gameplay.Player
{
    public class PlayerModel
    {
        private PlayerData playerData;
        
        public event Action<double> OnRiceChanged;
        public event Action<double> OnHonorChanged;
        public event Action<PlayerClass> OnClassChanged;
        public event Action<double> OnRicePerSecondChanged;
        public event Action<double> OnHonorPerSecondChanged;
        public event Action<double> OnRicePerTapChanged;
        public event Action<double> OnKokuChanged;

        public double Rice => playerData.Rice;
        public double Honor => playerData.Honor;
        public PlayerClass CurrentClass => playerData.CurrentClass;
        public double TotalRiceEarned => playerData.TotalRiceEarned;
        public double TotalHonorEarned => playerData.TotalHonorEarned;
        public double Koku => playerData.Koku;
        public double RicePerSecond => playerData.RicePerSecond;
        public double HonorPerSecond => playerData.HonorPerSecond;
        public double RicePerTap => playerData.RicePerTap;
        public int TapCount => playerData.TapCount;

        public PlayerModel()
        {
            playerData = new PlayerData();
        }

        public PlayerModel(PlayerData data)
        {
            playerData = data ?? new PlayerData();
        }

        public void AddRice(double amount)
        {
            if (amount <= 0) return;
            
            var previousRice = playerData.Rice;
            playerData.Rice += amount;
            
            if (Math.Abs(playerData.Rice - previousRice) > 0.001)
            {
                OnRiceChanged?.Invoke(playerData.Rice);
            }
        }

        public bool SpendRice(double amount)
        {
            if (amount <= 0 || playerData.Rice < amount) return false;
            
            playerData.Rice -= amount;
            OnRiceChanged?.Invoke(playerData.Rice);
            return true;
        }

        public void AddHonor(double amount)
        {
            if (amount <= 0) return;
            
            var previousHonor = playerData.Honor;
            playerData.Honor += amount;
            
            if (Math.Abs(playerData.Honor - previousHonor) > 0.001)
            {
                OnHonorChanged?.Invoke(playerData.Honor);
            }
        }

        public bool SpendHonor(double amount)
        {
            if (amount <= 0 || playerData.Honor < amount) return false;
            
            playerData.Honor -= amount;
            OnHonorChanged?.Invoke(playerData.Honor);
            return true;
        }

        public void PerformTap()
        {
            playerData.IncrementTapCount();
            AddRice(playerData.RicePerTap);
        }

        public void UpdateRicePerSecond(double newValue)
        {
            if (Math.Abs(playerData.RicePerSecond - newValue) < 0.001) return;
            
            playerData.SetRicePerSecond(newValue);
            OnRicePerSecondChanged?.Invoke(newValue);
        }

        public void UpdateHonorPerSecond(double newValue)
        {
            if (Math.Abs(playerData.HonorPerSecond - newValue) < 0.001) return;
            
            playerData.SetHonorPerSecond(newValue);
            OnHonorPerSecondChanged?.Invoke(newValue);
        }

        public void UpdateRicePerTap(double newValue)
        {
            if (Math.Abs(playerData.RicePerTap - newValue) < 0.001) return;
            
            playerData.SetRicePerTap(newValue);
            OnRicePerTapChanged?.Invoke(newValue);
        }

        public void UpdateKoku(double newValue)
        {
            if (Math.Abs(playerData.Koku - newValue) < 0.001) return;
            
            playerData.SetKoku(newValue);
            OnKokuChanged?.Invoke(newValue);
        }

        public bool CanAscendToNextClass(double requiredHonor)
        {
            return playerData.CanAscendToNextClass(requiredHonor);
        }

        public bool TryAscendClass(double requiredHonor)
        {
            var previousClass = playerData.CurrentClass;
            var success = playerData.TryAscendClass(requiredHonor);
            
            if (success)
            {
                OnClassChanged?.Invoke(playerData.CurrentClass);
                OnHonorChanged?.Invoke(playerData.Honor);
            }
            
            return success;
        }

        public void ProcessOfflineEarnings()
        {
            var currentTime = DateTime.Now;
            var previousRice = playerData.Rice;
            var previousHonor = playerData.Honor;
            
            playerData.CalculateOfflineEarnings(currentTime);
            
            if (Math.Abs(playerData.Rice - previousRice) > 0.001)
            {
                OnRiceChanged?.Invoke(playerData.Rice);
            }
            
            if (Math.Abs(playerData.Honor - previousHonor) > 0.001)
            {
                OnHonorChanged?.Invoke(playerData.Honor);
            }
            
            playerData.UpdateLastSaveTime();
        }

        public void UpdateProduction(float deltaTime)
        {
            if (playerData.RicePerSecond > 0)
            {
                AddRice(playerData.RicePerSecond * deltaTime);
            }
            
            if (playerData.HonorPerSecond > 0)
            {
                AddHonor(playerData.HonorPerSecond * deltaTime);
            }
        }

        public PlayerData GetPlayerData()
        {
            playerData.UpdateLastSaveTime();
            return playerData.Clone();
        }

        public void LoadPlayerData(PlayerData data)
        {
            if (data == null) return;
            
            playerData = data.Clone();
            
            OnRiceChanged?.Invoke(playerData.Rice);
            OnHonorChanged?.Invoke(playerData.Honor);
            OnClassChanged?.Invoke(playerData.CurrentClass);
            OnRicePerSecondChanged?.Invoke(playerData.RicePerSecond);
            OnHonorPerSecondChanged?.Invoke(playerData.HonorPerSecond);
            OnRicePerTapChanged?.Invoke(playerData.RicePerTap);
            OnKokuChanged?.Invoke(playerData.Koku);
        }

        public void ResetProgress()
        {
            playerData = new PlayerData();
            
            OnRiceChanged?.Invoke(playerData.Rice);
            OnHonorChanged?.Invoke(playerData.Honor);
            OnClassChanged?.Invoke(playerData.CurrentClass);
            OnRicePerSecondChanged?.Invoke(playerData.RicePerSecond);
            OnHonorPerSecondChanged?.Invoke(playerData.HonorPerSecond);
            OnRicePerTapChanged?.Invoke(playerData.RicePerTap);
            OnKokuChanged?.Invoke(playerData.Koku);
        }
    }
}