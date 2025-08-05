using UnityEngine;

namespace RoyalRoadClicker.Data
{
    [CreateAssetMenu(fileName = "ClassRequirements", menuName = "RoyalRoad/Class Requirements", order = 1)]
    public class ClassRequirements : ScriptableObject
    {
        [System.Serializable]
        public class ClassRequirement
        {
            public PlayerClass targetClass;
            public double honorRequired;
            public double minimumRicePerSecond;
            public string className;
            public string classDescription;
            
            [Header("Visual Unlocks")]
            public string[] unlockedFeatures;
            
            [Header("Gameplay Multipliers")]
            public float riceMultiplier = 1f;
            public float honorMultiplier = 1f;
            public float tapMultiplier = 1f;
        }

        [SerializeField] private ClassRequirement[] requirements = new ClassRequirement[]
        {
            new ClassRequirement
            {
                targetClass = PlayerClass.TenantFarmer,
                honorRequired = 100,
                minimumRicePerSecond = 0,
                className = "소작농 (Tenant Farmer)",
                classDescription = "You have earned enough honor to escape slavery and work the land.",
                unlockedFeatures = new string[] { "Basic Farm Tools", "Small Rice Field" },
                riceMultiplier = 1.5f,
                honorMultiplier = 1f,
                tapMultiplier = 2f
            },
            new ClassRequirement
            {
                targetClass = PlayerClass.Commoner,
                honorRequired = 1000,
                minimumRicePerSecond = 10,
                className = "평민 (Commoner)",
                classDescription = "You are now a free citizen with your own modest home.",
                unlockedFeatures = new string[] { "Personal House", "Market Access", "Honor Buildings" },
                riceMultiplier = 2f,
                honorMultiplier = 1.5f,
                tapMultiplier = 5f
            },
            new ClassRequirement
            {
                targetClass = PlayerClass.Noble,
                honorRequired = 10000,
                minimumRicePerSecond = 100,
                className = "양반 (Noble)",
                classDescription = "Your honor has elevated you to the noble class.",
                unlockedFeatures = new string[] { "Silk Robes", "Servants", "Scholar's Hall" },
                riceMultiplier = 3f,
                honorMultiplier = 2f,
                tapMultiplier = 10f
            },
            new ClassRequirement
            {
                targetClass = PlayerClass.Lord,
                honorRequired = 100000,
                minimumRicePerSecond = 1000,
                className = "영주 (Lord)",
                classDescription = "You now rule over vast territories measured in Koku.",
                unlockedFeatures = new string[] { "Territory Map", "Koku System", "Army Units" },
                riceMultiplier = 5f,
                honorMultiplier = 3f,
                tapMultiplier = 25f
            },
            new ClassRequirement
            {
                targetClass = PlayerClass.King,
                honorRequired = 1000000,
                minimumRicePerSecond = 10000,
                className = "왕 (King)",
                classDescription = "You have ascended to the throne and rule the entire kingdom.",
                unlockedFeatures = new string[] { "Royal Palace", "Dragon Robe", "Kingdom Management" },
                riceMultiplier = 10f,
                honorMultiplier = 5f,
                tapMultiplier = 100f
            }
        };

        public ClassRequirement GetRequirementForClass(PlayerClass targetClass)
        {
            foreach (var req in requirements)
            {
                if (req.targetClass == targetClass)
                    return req;
            }
            return null;
        }

        public ClassRequirement GetNextClassRequirement(PlayerClass currentClass)
        {
            if (currentClass >= PlayerClass.King)
                return null;

            PlayerClass nextClass = (PlayerClass)((int)currentClass + 1);
            return GetRequirementForClass(nextClass);
        }

        public bool CanAscendToClass(PlayerClass targetClass, double currentHonor, double currentRicePerSecond)
        {
            var requirement = GetRequirementForClass(targetClass);
            if (requirement == null)
                return false;

            return currentHonor >= requirement.honorRequired && 
                   currentRicePerSecond >= requirement.minimumRicePerSecond;
        }

        public float GetTotalMultiplier(PlayerClass currentClass, string multiplierType)
        {
            float totalMultiplier = 1f;

            for (int i = 0; i <= (int)currentClass; i++)
            {
                if (i == 0) continue; // Skip slave class
                
                var req = GetRequirementForClass((PlayerClass)i);
                if (req != null)
                {
                    switch (multiplierType.ToLower())
                    {
                        case "rice":
                            totalMultiplier *= req.riceMultiplier;
                            break;
                        case "honor":
                            totalMultiplier *= req.honorMultiplier;
                            break;
                        case "tap":
                            totalMultiplier *= req.tapMultiplier;
                            break;
                    }
                }
            }

            return totalMultiplier;
        }
    }
}