using UnityEngine;
using System.IO;

namespace MegabonkMobile.Meta
{
    /// <summary>
    /// Сервис сохранения и загрузки данных игры
    /// </summary>
    public static class SaveService
    {
        private static string SettingsPath => Path.Combine(Application.persistentDataPath, "Settings.json");
        private static string RecordsPath => Path.Combine(Application.persistentDataPath, "Records.json");
        private static string UnlocksPath => Path.Combine(Application.persistentDataPath, "Unlocks.json");

        #region Settings

        public static void SaveSettings(SettingsData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SettingsPath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка сохранения настроек: {e.Message}");
            }
        }

        public static SettingsData LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonUtility.FromJson<SettingsData>(json);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки настроек: {e.Message}");
            }

            return new SettingsData(); // Вернуть дефолтные значения
        }

        #endregion

        #region Records

        public static void SaveRecords(RecordsData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(RecordsPath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка сохранения рекордов: {e.Message}");
            }
        }

        public static RecordsData LoadRecords()
        {
            try
            {
                if (File.Exists(RecordsPath))
                {
                    string json = File.ReadAllText(RecordsPath);
                    return JsonUtility.FromJson<RecordsData>(json);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки рекордов: {e.Message}");
            }

            return new RecordsData(); // Вернуть дефолтные значения
        }

        #endregion

        #region Unlocks

        public static void SaveUnlocks(UnlocksData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(UnlocksPath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка сохранения разблокировок: {e.Message}");
            }
        }

        public static UnlocksData LoadUnlocks()
        {
            try
            {
                if (File.Exists(UnlocksPath))
                {
                    string json = File.ReadAllText(UnlocksPath);
                    return JsonUtility.FromJson<UnlocksData>(json);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки разблокировок: {e.Message}");
            }

            return new UnlocksData(); // Вернуть дефолтные значения
        }

        #endregion
    }

    [System.Serializable]
    public class SettingsData
    {
        public float masterVolume = 1f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 1f;
        public float joystickSensitivity = 1f;
        public int graphicsQuality = 2; // 0=Low, 1=Medium, 2=High
    }

    [System.Serializable]
    public class RecordsData
    {
        public int bestWave = 0;
        public float bestSurvivalTime = 0f;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int highestLevel = 1;
    }

    [System.Serializable]
    public class UnlocksData
    {
        public System.Collections.Generic.List<string> unlockedPerks = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> unlockedWeapons = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> unlockedCharacters = new System.Collections.Generic.List<string>();
    }
}

