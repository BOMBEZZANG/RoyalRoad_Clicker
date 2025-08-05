using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalRoadClicker.Core
{
    public static class EventManager
    {
        private static Dictionary<Type, List<Delegate>> eventListeners = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            Type eventType = typeof(T);
            
            if (!eventListeners.ContainsKey(eventType))
            {
                eventListeners[eventType] = new List<Delegate>();
            }
            
            eventListeners[eventType].Add(listener);
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            Type eventType = typeof(T);
            
            if (eventListeners.ContainsKey(eventType))
            {
                eventListeners[eventType].Remove(listener);
                
                if (eventListeners[eventType].Count == 0)
                {
                    eventListeners.Remove(eventType);
                }
            }
        }

        public static void Trigger<T>(T eventData) where T : struct
        {
            Type eventType = typeof(T);
            
            if (eventListeners.ContainsKey(eventType))
            {
                foreach (var listener in eventListeners[eventType].ToArray())
                {
                    try
                    {
                        ((Action<T>)listener)?.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error triggering event {eventType.Name}: {e.Message}");
                    }
                }
            }
        }

        public static void Clear()
        {
            eventListeners.Clear();
        }
    }

    #region Event Definitions
    public struct ResourceChangedEvent
    {
        public string ResourceType;
        public double OldValue;
        public double NewValue;
        
        public ResourceChangedEvent(string type, double oldVal, double newVal)
        {
            ResourceType = type;
            OldValue = oldVal;
            NewValue = newVal;
        }
    }

    public struct ClassAscensionEvent
    {
        public RoyalRoadClicker.Data.PlayerClass OldClass;
        public RoyalRoadClicker.Data.PlayerClass NewClass;
        public DateTime Timestamp;
        
        public ClassAscensionEvent(RoyalRoadClicker.Data.PlayerClass oldClass, RoyalRoadClicker.Data.PlayerClass newClass)
        {
            OldClass = oldClass;
            NewClass = newClass;
            Timestamp = DateTime.Now;
        }
    }

    public struct UpgradePurchasedEvent
    {
        public string UpgradeId;
        public double Cost;
        public string CostType;
        
        public UpgradePurchasedEvent(string id, double cost, string costType)
        {
            UpgradeId = id;
            Cost = cost;
            CostType = costType;
        }
    }

    public struct GameStateChangedEvent
    {
        public string OldState;
        public string NewState;
        
        public GameStateChangedEvent(string oldState, string newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    public struct TapPerformedEvent
    {
        public Vector2 ScreenPosition;
        public double RiceEarned;
        public int TotalTaps;
        
        public TapPerformedEvent(Vector2 pos, double rice, int taps)
        {
            ScreenPosition = pos;
            RiceEarned = rice;
            TotalTaps = taps;
        }
    }

    public struct SaveGameEvent
    {
        public bool Success;
        public string Message;
        
        public SaveGameEvent(bool success, string msg = "")
        {
            Success = success;
            Message = msg;
        }
    }

    public struct OfflineEarningsEvent
    {
        public double RiceEarned;
        public double HonorEarned;
        public TimeSpan OfflineTime;
        
        public OfflineEarningsEvent(double rice, double honor, TimeSpan time)
        {
            RiceEarned = rice;
            HonorEarned = honor;
            OfflineTime = time;
        }
    }
    #endregion
}