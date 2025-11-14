using UnityEngine;
using System.Collections.Generic;

namespace MegabonkMobile.Data
{
    /// <summary>
    /// ScriptableObject с конфигурацией волны врагов
    /// </summary>
    [CreateAssetMenu(fileName = "New Wave Config", menuName = "MegabonkMobile/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [System.Serializable]
        public class EnemySpawnGroup
        {
            [Tooltip("Тип врага для спавна")]
            public EnemyDef enemyDef;
            
            [Tooltip("Количество врагов в группе")]
            public int count = 5;
            
            [Tooltip("Задержка между спавном врагов в группе")]
            public float spawnInterval = 1f;
            
            [Tooltip("Зоны спавна (если пусто, спавн в случайных точках вокруг арены)")]
            public Transform[] spawnZones;
        }

        [Header("Wave Info")]
        public string waveName;
        public int waveNumber;

        [Header("Enemy Groups")]
        public List<EnemySpawnGroup> enemyGroups = new List<EnemySpawnGroup>();

        [Header("Wave Settings")]
        [Tooltip("Задержка перед началом волны")]
        public float delayBeforeWave = 2f;
        
        [Tooltip("Время между группами врагов")]
        public float delayBetweenGroups = 3f;
    }
}

