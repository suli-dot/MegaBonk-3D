using UnityEngine;
using MegabonkMobile.Core;
using MegabonkMobile.Data;
using MegabonkMobile.AI;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Система дропа лута при смерти врагов
    /// </summary>
    public class LootSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ObjectPool lootPool;
        [SerializeField] private AIDirector aiDirector;

        private void Start()
        {
            if (lootPool == null)
                lootPool = FindObjectOfType<ObjectPool>();

            if (aiDirector == null)
                aiDirector = FindObjectOfType<AIDirector>();

            // Подписаться на события смерти врагов
            EnemyBase.OnDeathStatic += OnEnemyDeath;
        }

        private void OnDestroy()
        {
            EnemyBase.OnDeathStatic -= OnEnemyDeath;
        }

        private void OnEnemyDeath(EnemyBase enemy)
        {
            if (enemy == null) return;

            // Получить таблицу дропа из EnemyDef
            EnemyDef enemyDef = enemy.GetEnemyDef();
            if (enemyDef == null || enemyDef.lootTable == null) return;

            // Получить случайный лут
            GameObject lootPrefab = enemyDef.lootTable.GetRandomLoot();
            if (lootPrefab == null) return;

            // Применить модификатор шанса дропа хила от AIDirector
            if (lootPrefab.GetComponent<HealthPickup>() != null)
            {
                float healDropChance = aiDirector != null ? aiDirector.HealDropChanceMultiplier : 1f;
                if (Random.Range(0f, 1f) > healDropChance)
                {
                    return; // Не дропать хил
                }
            }

            // Спавнить лут
            Vector3 spawnPosition = enemy.transform.position;
            SpawnLoot(lootPrefab, spawnPosition);
        }

        private void SpawnLoot(GameObject lootPrefab, Vector3 position)
        {
            // Добавить случайное смещение
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                0.5f,
                Random.Range(-1f, 1f)
            );

            GameObject loot;
            
            // Попытаться взять из пула
            if (lootPool != null)
            {
                loot = lootPool.Get(lootPrefab.name);
                if (loot != null)
                {
                    loot.transform.position = position + offset;
                    loot.transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                loot = Instantiate(lootPrefab, position + offset, Quaternion.identity);
            }
        }
    }
}

