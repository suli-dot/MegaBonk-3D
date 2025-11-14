using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MegabonkMobile.Core;
using MegabonkMobile.Data;
using MegabonkMobile.AI;

namespace MegabonkMobile.AI
{
    /// <summary>
    /// Спавнер волн врагов
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private List<WaveConfig> waveConfigs = new List<WaveConfig>();
        [SerializeField] private bool loopWaves = true;
        [SerializeField] private float arenaRadius = 20f;

        [Header("References")]
        [SerializeField] private Transform spawnCenter;
        [SerializeField] private AIDirector aiDirector;
        [SerializeField] private ObjectPool enemyPool;

        // Текущее состояние
        private int currentWaveIndex = 0;
        private int enemiesAlive = 0;
        private bool isSpawning = false;

        // События
        public System.Action<int> OnWaveStart; // waveNumber
        public System.Action<int> OnWaveComplete; // waveNumber
        public System.Action<EnemyBase> OnEnemySpawned;
        public System.Action<EnemyBase> OnEnemyKilled;

        private void Start()
        {
            if (spawnCenter == null)
                spawnCenter = transform;

            if (aiDirector == null)
                aiDirector = FindObjectOfType<AIDirector>();

            if (enemyPool == null)
                enemyPool = FindObjectOfType<ObjectPool>();

            // Подписаться на события смерти врагов
            EnemyBase.OnDeathStatic += OnEnemyDeath;
        }

        private void OnDestroy()
        {
            EnemyBase.OnDeathStatic -= OnEnemyDeath;
        }

        /// <summary>
        /// Начать следующую волну
        /// </summary>
        public void StartNextWave()
        {
            if (isSpawning) return;

            if (currentWaveIndex >= waveConfigs.Count)
            {
                if (loopWaves)
                {
                    currentWaveIndex = 0;
                }
                else
                {
                    Debug.Log("Все волны завершены!");
                    return;
                }
            }

            StartCoroutine(SpawnWaveCoroutine(waveConfigs[currentWaveIndex]));
        }

        private IEnumerator SpawnWaveCoroutine(WaveConfig waveConfig)
        {
            isSpawning = true;
            enemiesAlive = 0;

            OnWaveStart?.Invoke(waveConfig.waveNumber);

            yield return new WaitForSeconds(waveConfig.delayBeforeWave);

            // Спавнить группы врагов
            foreach (var group in waveConfig.enemyGroups)
            {
                yield return StartCoroutine(SpawnGroupCoroutine(group));
                yield return new WaitForSeconds(waveConfig.delayBetweenGroups);
            }

            isSpawning = false;
        }

        private IEnumerator SpawnGroupCoroutine(WaveConfig.EnemySpawnGroup group)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyDef, group.spawnZones);
                enemiesAlive++;
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        private void SpawnEnemy(EnemyDef enemyDef, Transform[] spawnZones)
        {
            if (enemyDef == null || enemyDef.prefab == null)
            {
                Debug.LogError("EnemyDef или prefab отсутствует!");
                return;
            }

            // Вычислить позицию спавна
            Vector3 spawnPosition = GetSpawnPosition(spawnZones);

            // Получить врага из пула или создать
            GameObject enemyObj;
            if (enemyPool != null)
            {
                enemyObj = enemyPool.Get(enemyDef.prefab.name);
                if (enemyObj != null)
                {
                    enemyObj.transform.position = spawnPosition;
                    enemyObj.transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                enemyObj = Instantiate(enemyDef.prefab, spawnPosition, Quaternion.identity);
            }

            if (enemyObj != null)
            {
                EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
                if (enemy == null)
                {
                    enemy = enemyObj.AddComponent<EnemyBase>();
                }

                enemy.Initialize(enemyDef);

                // Применить модификаторы от AIDirector
                if (aiDirector != null)
                {
                    float hpMultiplier = aiDirector.EnemyHpMultiplier;
                    float speedMultiplier = aiDirector.EnemySpeedMultiplier;
                    enemy.ApplyPowerUp(Mathf.Max(hpMultiplier, speedMultiplier));
                }

                OnEnemySpawned?.Invoke(enemy);
            }
        }

        private Vector3 GetSpawnPosition(Transform[] spawnZones)
        {
            if (spawnZones != null && spawnZones.Length > 0)
            {
                // Использовать указанные зоны спавна
                Transform zone = spawnZones[Random.Range(0, spawnZones.Length)];
                if (zone != null)
                {
                    return zone.position;
                }
            }

            // Случайная позиция вокруг арены
            Vector2 randomCircle = Random.insideUnitCircle * arenaRadius;
            Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;
            return center + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }

        private void OnEnemyDeath(EnemyBase enemy)
        {
            enemiesAlive--;
            OnEnemyKilled?.Invoke(enemy);

            // Проверить, завершена ли волна
            if (!isSpawning && enemiesAlive <= 0)
            {
                CompleteWave();
            }
        }

        private void CompleteWave()
        {
            if (currentWaveIndex < waveConfigs.Count)
            {
                OnWaveComplete?.Invoke(waveConfigs[currentWaveIndex].waveNumber);
            }

            currentWaveIndex++;
        }

        public int GetCurrentWaveNumber() => currentWaveIndex;
        public int GetEnemiesAlive() => enemiesAlive;
        public bool IsWaveActive() => isSpawning || enemiesAlive > 0;
    }
}

