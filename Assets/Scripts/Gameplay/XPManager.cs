using UnityEngine;
using System;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Управление опытом и уровнями игрока
    /// </summary>
    public class XPManager : MonoBehaviour
    {
        public static XPManager Instance { get; private set; }

        [Header("XP Settings")]
        [SerializeField] private int currentXP = 0;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int baseXPRequired = 100;
        [SerializeField] private float xpMultiplierPerLevel = 1.2f;

        // События
        public event Action<int, int, int> OnXPChanged; // currentXP, requiredXP, level
        public event Action<int> OnLevelUp; // newLevel

        private int RequiredXP => Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpMultiplierPerLevel, currentLevel - 1));

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            OnXPChanged?.Invoke(currentXP, RequiredXP, currentLevel);
        }

        public void AddXP(int amount)
        {
            if (amount <= 0) return;

            currentXP += amount;
            CheckLevelUp();
            OnXPChanged?.Invoke(currentXP, RequiredXP, currentLevel);
        }

        private void CheckLevelUp()
        {
            while (currentXP >= RequiredXP)
            {
                currentXP -= RequiredXP;
                currentLevel++;
                
                OnLevelUp?.Invoke(currentLevel);
                OnXPChanged?.Invoke(currentXP, RequiredXP, currentLevel);
            }
        }

        public int GetCurrentXP() => currentXP;
        public int GetCurrentLevel() => currentLevel;
        public int GetRequiredXP() => RequiredXP;
    }
}

