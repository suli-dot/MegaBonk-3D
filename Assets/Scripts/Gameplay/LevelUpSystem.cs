using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MegabonkMobile.Data;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Система повышения уровня и выбора перков
    /// </summary>
    public class LevelUpSystem : MonoBehaviour
    {
        [Header("Perk Settings")]
        [SerializeField] private List<Perk> allPerks = new List<Perk>();
        [SerializeField] private int perksPerLevel = 3;

        [Header("Rarity Weights")]
        [SerializeField] private float commonWeight = 1f;
        [SerializeField] private float rareWeight = 0.3f;
        [SerializeField] private float epicWeight = 0.1f;
        [SerializeField] private float legendaryWeight = 0.03f;

        private List<Perk> selectedPerks = new List<Perk>();

        // События
        public System.Action<List<Perk>> OnLevelUpMenuRequested; // список перков для выбора

        /// <summary>
        /// Показать меню выбора перков
        /// </summary>
        public void ShowLevelUpMenu()
        {
            List<Perk> availablePerks = GetRandomPerks(perksPerLevel);
            OnLevelUpMenuRequested?.Invoke(availablePerks);
        }

        /// <summary>
        /// Получить случайные перки с учетом редкости и синергий
        /// </summary>
        private List<Perk> GetRandomPerks(int count)
        {
            List<Perk> availablePerks = new List<Perk>(allPerks);
            
            // Исключить уже выбранные перки (или можно разрешить несколько раз)
            // availablePerks.RemoveAll(p => selectedPerks.Contains(p));

            // Вычислить веса с учетом синергий
            Dictionary<Perk, float> weights = new Dictionary<Perk, float>();
            
            foreach (var perk in availablePerks)
            {
                float weight = GetRarityWeight(perk.rarity);
                
                // Увеличить вес, если есть синергия с выбранными перками
                foreach (var selectedPerk in selectedPerks)
                {
                    if (perk.synergyPerkIds.Contains(selectedPerk.id))
                    {
                        weight *= 1.5f; // Увеличить шанс
                    }
                }
                
                weights[perk] = weight;
            }

            // Выбрать перки на основе весов
            List<Perk> selected = new List<Perk>();
            List<Perk> tempList = new List<Perk>(availablePerks);

            for (int i = 0; i < count && tempList.Count > 0; i++)
            {
                // Вычислить общий вес
                float totalWeight = 0f;
                foreach (var perk in tempList)
                {
                    totalWeight += weights[perk];
                }

                // Случайный выбор
                float random = Random.Range(0f, totalWeight);
                float cumulativeWeight = 0f;

                Perk selectedPerk = null;
                foreach (var perk in tempList)
                {
                    cumulativeWeight += weights[perk];
                    if (random <= cumulativeWeight)
                    {
                        selectedPerk = perk;
                        break;
                    }
                }

                if (selectedPerk != null)
                {
                    selected.Add(selectedPerk);
                    tempList.Remove(selectedPerk);
                }
            }

            return selected;
        }

        private float GetRarityWeight(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    return commonWeight;
                case Rarity.Rare:
                    return rareWeight;
                case Rarity.Epic:
                    return epicWeight;
                case Rarity.Legendary:
                    return legendaryWeight;
                default:
                    return 1f;
            }
        }

        /// <summary>
        /// Применить выбранный перк
        /// </summary>
        public void ApplyPerk(Perk perk)
        {
            if (perk == null) return;

            selectedPerks.Add(perk);

            // Применить эффекты перка
            WeaponSystem weaponSystem = FindObjectOfType<WeaponSystem>();
            PlayerController playerController = FindObjectOfType<PlayerController>();

            foreach (var effect in perk.effects)
            {
                if (weaponSystem != null)
                {
                    weaponSystem.AddStatModifier(effect);
                }

                // Применить к игроку (здоровье, скорость)
                if (playerController != null)
                {
                    switch (effect.statType)
                    {
                        case StatType.MaxHealth:
                            playerController.Heal(effect.value);
                            break;
                        case StatType.MoveSpeed:
                            // Можно добавить модификатор скорости в PlayerController
                            break;
                    }
                }
            }
        }
    }
}

