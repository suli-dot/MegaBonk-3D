using UnityEngine;
using System.Collections.Generic;
using MegabonkMobile.Gameplay;

namespace MegabonkMobile.AI
{
    /// <summary>
    /// AI Director - динамическая сложность на основе метрик игрока
    /// </summary>
    public class AIDirector : MonoBehaviour
    {
        [Header("Difficulty Settings")]
        [SerializeField] private float baseDifficulty = 0.5f;
        [SerializeField] private float minDifficulty = 0.1f;
        [SerializeField] private float maxDifficulty = 1.0f;
        [SerializeField] private float adjustmentSpeed = 0.1f;

        [Header("Metrics Window")]
        [SerializeField] private float metricsWindowSize = 30f; // секунды

        [Header("Multipliers")]
        [SerializeField] private AnimationCurve enemyCountCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 2f);
        [SerializeField] private AnimationCurve enemyHpCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 2f);
        [SerializeField] private AnimationCurve enemySpeedCurve = AnimationCurve.Linear(0f, 0.8f, 1f, 1.5f);
        [SerializeField] private AnimationCurve spawnIntervalCurve = AnimationCurve.Linear(0f, 1.5f, 1f, 0.5f);
        [SerializeField] private AnimationCurve healDropChanceCurve = AnimationCurve.Linear(0f, 0.3f, 1f, 0.1f);

        // Метрики
        private Queue<PlayerMetrics> metricsHistory = new Queue<PlayerMetrics>();
        private float currentDifficulty;

        // Геттеры для множителей
        public float Difficulty => currentDifficulty;
        public float EnemyCountMultiplier => enemyCountCurve.Evaluate(currentDifficulty);
        public float EnemyHpMultiplier => enemyHpCurve.Evaluate(currentDifficulty);
        public float EnemySpeedMultiplier => enemySpeedCurve.Evaluate(currentDifficulty);
        public float SpawnIntervalMultiplier => spawnIntervalCurve.Evaluate(currentDifficulty);
        public float HealDropChanceMultiplier => healDropChanceCurve.Evaluate(currentDifficulty);

        private void Start()
        {
            currentDifficulty = baseDifficulty;
        }

        private void Update()
        {
            UpdateMetrics();
            AdjustDifficulty();
        }

        private void UpdateMetrics()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null) return;

            // Собрать текущие метрики
            PlayerMetrics metrics = new PlayerMetrics
            {
                timestamp = Time.time,
                healthPercent = player.CurrentHealth / player.MaxHealth,
                isAlive = player.IsAlive
            };

            metricsHistory.Enqueue(metrics);

            // Удалить старые метрики
            float cutoffTime = Time.time - metricsWindowSize;
            while (metricsHistory.Count > 0 && metricsHistory.Peek().timestamp < cutoffTime)
            {
                metricsHistory.Dequeue();
            }
        }

        private void AdjustDifficulty()
        {
            if (metricsHistory.Count == 0) return;

            // Вычислить средние метрики
            float avgHealthPercent = 0f;
            int aliveCount = 0;

            foreach (var metric in metricsHistory)
            {
                avgHealthPercent += metric.healthPercent;
                if (metric.isAlive)
                    aliveCount++;
            }

            avgHealthPercent /= metricsHistory.Count;
            float survivalRate = (float)aliveCount / metricsHistory.Count;

            // Вычислить целевую сложность на основе метрик
            // Если игроку легко (высокий HP, высокая выживаемость) → увеличить сложность
            // Если игроку тяжело (низкий HP, низкая выживаемость) → уменьшить сложность
            float targetDifficulty = baseDifficulty;

            // Корректировка на основе здоровья
            if (avgHealthPercent > 0.7f)
            {
                targetDifficulty += 0.2f; // Игроку легко
            }
            else if (avgHealthPercent < 0.3f)
            {
                targetDifficulty -= 0.2f; // Игроку тяжело
            }

            // Корректировка на основе выживаемости
            if (survivalRate > 0.8f)
            {
                targetDifficulty += 0.1f;
            }
            else if (survivalRate < 0.5f)
            {
                targetDifficulty -= 0.1f;
            }

            // Плавно изменить сложность
            targetDifficulty = Mathf.Clamp(targetDifficulty, minDifficulty, maxDifficulty);
            currentDifficulty = Mathf.Lerp(
                currentDifficulty,
                targetDifficulty,
                Time.deltaTime * adjustmentSpeed
            );
        }

        /// <summary>
        /// Принудительно установить сложность (например, при начале новой волны)
        /// </summary>
        public void SetDifficulty(float difficulty)
        {
            currentDifficulty = Mathf.Clamp(difficulty, minDifficulty, maxDifficulty);
        }

        /// <summary>
        /// Сбросить метрики
        /// </summary>
        public void ResetMetrics()
        {
            metricsHistory.Clear();
            currentDifficulty = baseDifficulty;
        }
    }

    /// <summary>
    /// Метрики игрока для анализа
    /// </summary>
    public struct PlayerMetrics
    {
        public float timestamp;
        public float healthPercent;
        public bool isAlive;
    }
}

