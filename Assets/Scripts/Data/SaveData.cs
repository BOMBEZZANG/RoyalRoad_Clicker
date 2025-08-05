using System;
using UnityEngine;

namespace RoyalRoadClicker.Data
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int saveVersion = 1;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private string saveTimestamp;
        [SerializeField] private long playTime;

        public int SaveVersion => saveVersion;
        public PlayerData PlayerData => playerData;
        public string SaveTimestamp => saveTimestamp;
        public long PlayTime => playTime;

        public SaveData()
        {
            saveVersion = 1;
            playerData = new PlayerData();
            saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            playTime = 0;
        }

        public SaveData(PlayerData player, long totalPlayTime)
        {
            saveVersion = 1;
            playerData = player?.Clone() ?? new PlayerData();
            saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            playTime = totalPlayTime;
        }

        public bool IsValidSave()
        {
            return playerData != null && saveVersion > 0;
        }

        public void UpdateTimestamp()
        {
            saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}