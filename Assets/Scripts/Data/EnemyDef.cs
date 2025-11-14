using UnityEngine;

namespace MegabonkMobile.Data
{
    /// <summary>
    /// ScriptableObject с данными врага
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "MegabonkMobile/Enemy")]
    public class EnemyDef : ScriptableObject
    {
        [Header("Enemy Info")]
        public string enemyName;
        public EnemyType enemyType;
        public GameObject prefab;

        [Header("Stats")]
        public float maxHealth = 50f;
        public float moveSpeed = 3f;
        public float attackDamage = 5f;
        public float attackRange = 2f;
        public float detectionRange = 10f;
        public float attackCooldown = 2f;

        [Header("AI Pattern")]
        public AIPattern aiPattern = AIPattern.Rusher;

        [Header("Loot")]
        public LootTable lootTable;

        [Header("Wave Weight")]
        [Tooltip("Вес для спавна в волнах (чем больше, тем чаще спавнится)")]
        public int weight = 1;
    }

    public enum EnemyType
    {
        Rusher,
        ZigZag,
        Shooter,
        Suicide
    }

    public enum AIPattern
    {
        Rusher,    // Прямо на игрока
        ZigZag,    // Зигзагообразное движение
        Shooter,   // Держит дистанцию и стреляет
        Suicide    // Подбегает и взрывается
    }
}

