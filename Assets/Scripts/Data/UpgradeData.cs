using System;
using UnityEngine;

namespace RoyalRoadClicker.Data
{
    [Serializable]
    public enum UpgradeType
    {
        ClickUpgrade,      // Increases Rice per tap
        ProductionUpgrade  // Increases Rice per second
    }

    [Serializable]
    public class UpgradeItem
    {
        [Header("Basic Info")]
        public string id;
        public string displayName;
        public string description;
        public Sprite icon;
        public UpgradeType upgradeType;

        [Header("Cost Settings")]
        public double baseCost;
        public float costMultiplier = 1.15f; // Cost increases by 15% each level
        public int maxLevel = 100;

        [Header("Effect Settings")]
        public double baseEffect; // Base rice per tap or rice per second
        public float effectMultiplier = 1.1f; // Effect increases by 10% each level

        [Header("Unlock Requirements")]
        public PlayerClass requiredClass = PlayerClass.Slave;
        public string[] requiredUpgrades; // IDs of upgrades that must be purchased first
        public double requiredRicePerSecond = 0;

        public double GetCostForLevel(int level)
        {
            if (level <= 0) return baseCost;
            return baseCost * Math.Pow(costMultiplier, level);
        }

        public double GetEffectForLevel(int level)
        {
            if (level <= 0) return 0;
            return baseEffect * Math.Pow(effectMultiplier, level - 1);
        }

        public double GetTotalEffectForLevel(int level)
        {
            double totalEffect = 0;
            for (int i = 1; i <= level; i++)
            {
                totalEffect += GetEffectForLevel(i);
            }
            return totalEffect;
        }

        public bool IsUnlocked(PlayerClass currentClass, double currentRicePerSecond, System.Func<string, int> getLevelFunc)
        {
            // Check class requirement
            if (currentClass < requiredClass)
                return false;

            // Check rice per second requirement
            if (currentRicePerSecond < requiredRicePerSecond)
                return false;

            // Check required upgrades
            if (requiredUpgrades != null)
            {
                foreach (string requiredId in requiredUpgrades)
                {
                    if (getLevelFunc(requiredId) <= 0)
                        return false;
                }
            }

            return true;
        }
    }

    [CreateAssetMenu(fileName = "UpgradeData", menuName = "RoyalRoad/Upgrade Data", order = 2)]
    public class UpgradeData : ScriptableObject
    {
        [Header("Click Upgrades")]
        [SerializeField] private UpgradeItem[] clickUpgrades;

        [Header("Production Upgrades")]
        [SerializeField] private UpgradeItem[] productionUpgrades;

        public UpgradeItem[] ClickUpgrades => clickUpgrades;
        public UpgradeItem[] ProductionUpgrades => productionUpgrades;

        private void OnValidate()
        {
            // Ensure all upgrades have valid IDs
            ValidateUpgradeArray(clickUpgrades, "Click");
            ValidateUpgradeArray(productionUpgrades, "Production");
        }

        private void ValidateUpgradeArray(UpgradeItem[] upgrades, string prefix)
        {
            if (upgrades == null) return;

            for (int i = 0; i < upgrades.Length; i++)
            {
                if (upgrades[i] != null && string.IsNullOrEmpty(upgrades[i].id))
                {
                    upgrades[i].id = $"{prefix}_{i:D2}";
                }
            }
        }

        public UpgradeItem GetUpgradeById(string id)
        {
            // Search in click upgrades
            if (clickUpgrades != null)
            {
                foreach (var upgrade in clickUpgrades)
                {
                    if (upgrade != null && upgrade.id == id)
                        return upgrade;
                }
            }

            // Search in production upgrades
            if (productionUpgrades != null)
            {
                foreach (var upgrade in productionUpgrades)
                {
                    if (upgrade != null && upgrade.id == id)
                        return upgrade;
                }
            }

            return null;
        }

        public UpgradeItem[] GetUpgradesByType(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.ClickUpgrade:
                    return clickUpgrades ?? new UpgradeItem[0];
                case UpgradeType.ProductionUpgrade:
                    return productionUpgrades ?? new UpgradeItem[0];
                default:
                    return new UpgradeItem[0];
            }
        }

        public UpgradeItem[] GetAllUpgrades()
        {
            var allUpgrades = new System.Collections.Generic.List<UpgradeItem>();
            
            if (clickUpgrades != null)
                allUpgrades.AddRange(clickUpgrades);
            
            if (productionUpgrades != null)
                allUpgrades.AddRange(productionUpgrades);

            return allUpgrades.ToArray();
        }
    }

    [Serializable]
    public class PlayerUpgradeProgress
    {
        [SerializeField] private SerializableDictionary<string, int> upgradeLevels = new SerializableDictionary<string, int>();

        public int GetUpgradeLevel(string upgradeId)
        {
            return upgradeLevels.ContainsKey(upgradeId) ? upgradeLevels[upgradeId] : 0;
        }

        public void SetUpgradeLevel(string upgradeId, int level)
        {
            upgradeLevels[upgradeId] = Math.Max(0, level);
        }

        public void IncrementUpgrade(string upgradeId)
        {
            int currentLevel = GetUpgradeLevel(upgradeId);
            SetUpgradeLevel(upgradeId, currentLevel + 1);
        }

        public bool HasUpgrade(string upgradeId)
        {
            return GetUpgradeLevel(upgradeId) > 0;
        }

        public SerializableDictionary<string, int> GetAllLevels()
        {
            return new SerializableDictionary<string, int>(upgradeLevels);
        }

        public void LoadFromDictionary(SerializableDictionary<string, int> levels)
        {
            upgradeLevels = new SerializableDictionary<string, int>(levels);
        }
    }

    // Simple serializable dictionary for Unity
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] keys;
        [SerializeField] private TValue[] values;

        public SerializableDictionary() : base() { }
        public SerializableDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dict) : base(dict) { }

        public void OnBeforeSerialize()
        {
            keys = new TKey[this.Count];
            values = new TValue[this.Count];
            int i = 0;
            foreach (var kvp in this)
            {
                keys[i] = kvp.Key;
                values[i] = kvp.Value;
                i++;
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            if (keys != null && values != null && keys.Length == values.Length)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    this[keys[i]] = values[i];
                }
            }
        }
    }
}