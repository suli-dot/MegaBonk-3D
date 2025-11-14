using UnityEngine;
using System.Collections.Generic;

namespace MegabonkMobile.Data
{
    /// <summary>
    /// ScriptableObject с таблицей дропа лута
    /// </summary>
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "MegabonkMobile/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public class LootDrop
        {
            public LootType lootType;
            public GameObject prefab;
            [Range(0f, 100f)]
            public float dropChance = 10f; // Процент шанса
        }

        [Header("Loot Drops")]
        public List<LootDrop> lootDrops = new List<LootDrop>();

        /// <summary>
        /// Получить случайный лут на основе шансов
        /// </summary>
        public GameObject GetRandomLoot()
        {
            if (lootDrops.Count == 0) return null;

            float random = Random.Range(0f, 100f);
            float cumulativeChance = 0f;

            foreach (var drop in lootDrops)
            {
                cumulativeChance += drop.dropChance;
                if (random <= cumulativeChance)
                {
                    return drop.prefab;
                }
            }

            // Если ничего не выпало, вернуть первый лут (fallback)
            return lootDrops[0].prefab;
        }
    }

    public enum LootType
    {
        XPOrb,
        HealthPickup,
        Coin,
        Artifact
    }
}

