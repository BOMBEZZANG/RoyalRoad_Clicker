using System;
using UnityEngine;

namespace RoyalRoadClicker.Data
{
    [Serializable]
    public enum PlayerClass
    {
        Slave = 0,          // 노비
        TenantFarmer = 1,   // 소작농
        Commoner = 2,       // 평민
        Noble = 3,          // 양반
        Lord = 4,           // 영주
        King = 5            // 왕
    }

    [Serializable]
    public class PlayerData
    {
        [SerializeField] private double rice;
        [SerializeField] private double honor;
        [SerializeField] private PlayerClass currentClass;
        [SerializeField] private double totalRiceEarned;
        [SerializeField] private double totalHonorEarned;
        [SerializeField] private double koku;
        [SerializeField] private DateTime lastSaveTime;
        [SerializeField] private double ricePerSecond;
        [SerializeField] private double honorPerSecond;
        [SerializeField] private int tapCount;
        [SerializeField] private double ricePerTap;
        [SerializeField] private PlayerUpgradeProgress upgradeProgress;

        public double Rice
        {
            get => rice;
            set
            {
                if (value < 0) value = 0;
                var delta = value - rice;
                if (delta > 0) totalRiceEarned += delta;
                rice = value;
            }
        }

        public double Honor
        {
            get => honor;
            set
            {
                if (value < 0) value = 0;
                var delta = value - honor;
                if (delta > 0) totalHonorEarned += delta;
                honor = value;
            }
        }

        public PlayerClass CurrentClass
        {
            get => currentClass;
            set => currentClass = value;
        }

        public double TotalRiceEarned => totalRiceEarned;
        public double TotalHonorEarned => totalHonorEarned;
        public double Koku => koku;
        public DateTime LastSaveTime => lastSaveTime;
        public double RicePerSecond => ricePerSecond;
        public double HonorPerSecond => honorPerSecond;
        public int TapCount => tapCount;
        public double RicePerTap => ricePerTap;
        public PlayerUpgradeProgress UpgradeProgress => upgradeProgress;

        public PlayerData()
        {
            rice = 0;
            honor = 0;
            currentClass = PlayerClass.Slave;
            totalRiceEarned = 0;
            totalHonorEarned = 0;
            koku = 0;
            lastSaveTime = DateTime.Now;
            ricePerSecond = 0;
            honorPerSecond = 0;
            tapCount = 0;
            ricePerTap = 1;
            upgradeProgress = new PlayerUpgradeProgress();
        }

        public void UpdateLastSaveTime()
        {
            lastSaveTime = DateTime.Now;
        }

        public void IncrementTapCount()
        {
            tapCount++;
        }

        public void SetRicePerSecond(double value)
        {
            ricePerSecond = Math.Max(0, value);
        }

        public void SetHonorPerSecond(double value)
        {
            honorPerSecond = Math.Max(0, value);
        }

        public void SetRicePerTap(double value)
        {
            ricePerTap = Math.Max(0, value);
        }

        public void SetKoku(double value)
        {
            koku = Math.Max(0, value);
        }

        public bool CanAscendToNextClass(double requiredHonor)
        {
            return honor >= requiredHonor && currentClass < PlayerClass.King;
        }

        public bool TryAscendClass(double requiredHonor)
        {
            if (!CanAscendToNextClass(requiredHonor)) return false;

            honor -= requiredHonor;
            currentClass = (PlayerClass)((int)currentClass + 1);
            return true;
        }

        public void CalculateOfflineEarnings(DateTime currentTime)
        {
            var timeDifference = currentTime - lastSaveTime;
            var secondsOffline = Math.Max(0, timeDifference.TotalSeconds);

            if (secondsOffline > 0)
            {
                var offlineRice = ricePerSecond * secondsOffline;
                var offlineHonor = honorPerSecond * secondsOffline;

                Rice += offlineRice;
                Honor += offlineHonor;
            }
        }

        public PlayerData Clone()
        {
            return new PlayerData
            {
                rice = this.rice,
                honor = this.honor,
                currentClass = this.currentClass,
                totalRiceEarned = this.totalRiceEarned,
                totalHonorEarned = this.totalHonorEarned,
                koku = this.koku,
                lastSaveTime = this.lastSaveTime,
                ricePerSecond = this.ricePerSecond,
                honorPerSecond = this.honorPerSecond,
                tapCount = this.tapCount,
                ricePerTap = this.ricePerTap
            };
        }
    }
}