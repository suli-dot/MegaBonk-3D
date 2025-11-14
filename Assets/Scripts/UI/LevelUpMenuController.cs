using UnityEngine;
using System.Collections.Generic;
using MegabonkMobile.Data;
using MegabonkMobile.Gameplay;

namespace MegabonkMobile.UI
{
    /// <summary>
    /// Контроллер меню выбора перков (логика, без UI)
    /// </summary>
    public class LevelUpMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LevelUpSystem levelUpSystem;

        // События для UI
        public event System.Action<List<Perk>> OnShowPerkSelection; // список перков для отображения
        public event System.Action OnHidePerkSelection;

        private List<Perk> currentPerks = new List<Perk>();

        private void Start()
        {
            if (levelUpSystem == null)
                levelUpSystem = FindObjectOfType<LevelUpSystem>();

            // Подписаться на события LevelUpSystem
            if (levelUpSystem != null)
            {
                levelUpSystem.OnLevelUpMenuRequested += OnLevelUpRequested;
            }

            // Подписаться на события XPManager
            if (XPManager.Instance != null)
            {
                XPManager.Instance.OnLevelUp += OnPlayerLevelUp;
            }
        }

        private void OnDestroy()
        {
            if (levelUpSystem != null)
            {
                levelUpSystem.OnLevelUpMenuRequested -= OnLevelUpRequested;
            }

            if (XPManager.Instance != null)
            {
                XPManager.Instance.OnLevelUp -= OnPlayerLevelUp;
            }
        }

        private void OnPlayerLevelUp(int newLevel)
        {
            // Показать меню выбора перков
            if (levelUpSystem != null)
            {
                levelUpSystem.ShowLevelUpMenu();
            }
        }

        private void OnLevelUpRequested(List<Perk> perks)
        {
            currentPerks = perks;
            OnShowPerkSelection?.Invoke(perks);
            
            // Остановить время
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Выбрать перк (вызывается из UI)
        /// </summary>
        public void SelectPerk(Perk perk)
        {
            if (perk == null || levelUpSystem == null) return;

            // Применить перк
            levelUpSystem.ApplyPerk(perk);

            // Скрыть меню
            HideMenu();
        }

        private void HideMenu()
        {
            OnHidePerkSelection?.Invoke();
            
            // Возобновить время
            Time.timeScale = 1f;
        }
    }
}

