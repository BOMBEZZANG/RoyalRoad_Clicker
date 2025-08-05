using System;
using System.IO;
using UnityEngine;

namespace RoyalRoadClicker.Data
{
    public static class PlayerDataSerializer
    {
        private const string SAVE_FILE_NAME = "playerdata.sav";
        private const string BACKUP_FILE_NAME = "playerdata.bak";
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        private static readonly string BackupPath = Path.Combine(Application.persistentDataPath, BACKUP_FILE_NAME);

        public static bool SaveToFile(SaveData saveData)
        {
            if (saveData == null || !saveData.IsValidSave())
            {
                Debug.LogError("Invalid save data!");
                return false;
            }

            try
            {
                // Create backup of existing save
                if (File.Exists(SavePath))
                {
                    File.Copy(SavePath, BackupPath, true);
                }

                // Convert to JSON with pretty print for debugging
                string json = JsonUtility.ToJson(saveData, true);
                
                // Encrypt or encode the data (simple base64 for now)
                string encodedData = EncodeData(json);
                
                // Write to file
                File.WriteAllText(SavePath, encodedData);
                
                Debug.Log($"Game saved successfully at {saveData.SaveTimestamp}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                
                // Restore backup if save failed
                if (File.Exists(BackupPath))
                {
                    try
                    {
                        File.Copy(BackupPath, SavePath, true);
                    }
                    catch
                    {
                        // Backup restore failed
                    }
                }
                
                return false;
            }
        }

        public static SaveData LoadFromFile()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("No save file found. Starting new game.");
                return null;
            }

            try
            {
                // Read from file
                string encodedData = File.ReadAllText(SavePath);
                
                // Decode the data
                string json = DecodeData(encodedData);
                
                // Convert from JSON
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData != null && saveData.IsValidSave())
                {
                    Debug.Log($"Game loaded successfully. Last saved: {saveData.SaveTimestamp}");
                    return saveData;
                }
                else
                {
                    Debug.LogWarning("Save file is corrupted or invalid.");
                    return LoadBackup();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return LoadBackup();
            }
        }

        private static SaveData LoadBackup()
        {
            if (!File.Exists(BackupPath))
            {
                Debug.Log("No backup file found.");
                return null;
            }

            try
            {
                string encodedData = File.ReadAllText(BackupPath);
                string json = DecodeData(encodedData);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData != null && saveData.IsValidSave())
                {
                    Debug.Log("Loaded from backup file.");
                    return saveData;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load backup: {e.Message}");
            }

            return null;
        }

        public static bool DeleteSaveFile()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }
                
                if (File.Exists(BackupPath))
                {
                    File.Delete(BackupPath);
                }
                
                Debug.Log("Save files deleted successfully.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save files: {e.Message}");
                return false;
            }
        }

        public static bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }

        public static DateTime GetLastSaveTime()
        {
            if (File.Exists(SavePath))
            {
                return File.GetLastWriteTime(SavePath);
            }
            return DateTime.MinValue;
        }

        private static string EncodeData(string data)
        {
            try
            {
                byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(bytesToEncode);
            }
            catch
            {
                return data;
            }
        }

        private static string DecodeData(string encodedData)
        {
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(encodedData);
                return System.Text.Encoding.UTF8.GetString(decodedBytes);
            }
            catch
            {
                // If decoding fails, assume it's plain text (for backwards compatibility)
                return encodedData;
            }
        }

        public static void SaveToPlayerPrefs(SaveData saveData)
        {
            if (saveData == null || !saveData.IsValidSave())
                return;

            try
            {
                string json = JsonUtility.ToJson(saveData);
                PlayerPrefs.SetString("RoyalRoadSaveData", json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save to PlayerPrefs: {e.Message}");
            }
        }

        public static SaveData LoadFromPlayerPrefs()
        {
            try
            {
                if (PlayerPrefs.HasKey("RoyalRoadSaveData"))
                {
                    string json = PlayerPrefs.GetString("RoyalRoadSaveData");
                    return JsonUtility.FromJson<SaveData>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load from PlayerPrefs: {e.Message}");
            }

            return null;
        }
    }
}