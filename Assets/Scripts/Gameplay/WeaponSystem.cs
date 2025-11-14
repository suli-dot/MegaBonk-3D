using UnityEngine;
using MegabonkMobile.Core;
using MegabonkMobile.Data;
using System.Collections.Generic;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Система оружия игрока - управляет атаками и модификаторами от перков
    /// </summary>
    public class WeaponSystem : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private WeaponDef currentWeapon;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("Modifiers (от перков)")]
        [SerializeField] private float damageModifier = 1f;
        [SerializeField] private float critChanceModifier = 0f;
        [SerializeField] private float cooldownModifier = 1f;
        [SerializeField] private float rangeModifier = 1f;
        [SerializeField] private float attackSpeedModifier = 1f;

        // Модификаторы от перков
        private List<StatModifier> statModifiers = new List<StatModifier>();

        private float lastAttackTime;
        private ObjectPool projectilePool;

        private void Awake()
        {
            if (attackPoint == null)
            {
                GameObject attackPointObj = new GameObject("AttackPoint");
                attackPointObj.transform.SetParent(transform);
                attackPointObj.transform.localPosition = Vector3.forward * 1f;
                attackPoint = attackPointObj.transform;
            }

            projectilePool = FindObjectOfType<ObjectPool>();
        }

        private void Start()
        {
            if (currentWeapon == null)
            {
                Debug.LogWarning("WeaponSystem: нет оружия!");
            }
        }

        /// <summary>
        /// Попытаться атаковать
        /// </summary>
        public bool TryAttack()
        {
            if (currentWeapon == null) return false;

            float cooldown = currentWeapon.cooldown / cooldownModifier;
            if (Time.time < lastAttackTime + cooldown) return false;

            PerformAttack();
            lastAttackTime = Time.time;
            return true;
        }

        private void PerformAttack()
        {
            if (currentWeapon.type == WeaponType.Melee)
            {
                PerformMeleeAttack();
            }
            else
            {
                PerformRangedAttack();
            }

            // Звук атаки
            if (currentWeapon.attackSound != null)
            {
                Utilities.AudioManager.Instance?.PlaySfx(Utilities.SfxType.Attack);
            }
        }

        private void PerformMeleeAttack()
        {
            Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
            float range = currentWeapon.range * rangeModifier;

            Collider[] hitColliders = Physics.OverlapSphere(attackPos, range, enemyLayerMask);

            foreach (Collider hitCollider in hitColliders)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    float damage = GetFinalDamage();
                    bool isCrit = RollCritical();
                    
                    if (isCrit)
                    {
                        damage *= currentWeapon.critMultiplier;
                    }

                    DamageInfo damageInfo = new DamageInfo(
                        damage,
                        transform.position,
                        isCrit,
                        5f // knockback
                    );

                    damageable.TakeDamage(damageInfo);

                    // Эффект попадания
                    if (currentWeapon.hitEffectPrefab != null)
                    {
                        Instantiate(currentWeapon.hitEffectPrefab, hitCollider.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        private void PerformRangedAttack()
        {
            if (currentWeapon.projectilePrefab == null)
            {
                Debug.LogWarning("Ranged weapon не имеет projectile prefab!");
                return;
            }

            Vector3 spawnPos = attackPoint != null ? attackPoint.position : transform.position;
            Vector3 direction = transform.forward;

            GameObject projectile;
            
            // Попытаться взять из пула
            if (projectilePool != null)
            {
                projectile = projectilePool.Get(currentWeapon.projectilePrefab.name);
                if (projectile != null)
                {
                    projectile.transform.position = spawnPos;
                    projectile.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            else
            {
                projectile = Instantiate(currentWeapon.projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
            }

            if (projectile != null)
            {
                Projectile proj = projectile.GetComponent<Projectile>();
                if (proj != null)
                {
                    float damage = GetFinalDamage();
                    float speed = currentWeapon.projectileSpeed * attackSpeedModifier;
                    proj.Initialize(damage, direction, speed, currentWeapon.range * rangeModifier, enemyLayerMask);
                }
            }
        }

        private float GetFinalDamage()
        {
            float damage = currentWeapon.baseDamage * damageModifier;
            
            // Применить модификаторы от перков
            foreach (var mod in statModifiers)
            {
                if (mod.statType == StatType.Damage)
                {
                    damage += mod.value;
                }
                else if (mod.statType == StatType.DamageMultiplier)
                {
                    damage *= mod.value;
                }
            }

            return damage;
        }

        private bool RollCritical()
        {
            float critChance = currentWeapon.critChance + critChanceModifier;
            
            // Применить модификаторы от перков
            foreach (var mod in statModifiers)
            {
                if (mod.statType == StatType.CritChance)
                {
                    critChance += mod.value;
                }
            }

            return Random.Range(0f, 1f) < critChance;
        }

        /// <summary>
        /// Установить оружие
        /// </summary>
        public void SetWeapon(WeaponDef weapon)
        {
            currentWeapon = weapon;
        }

        /// <summary>
        /// Добавить модификатор от перка
        /// </summary>
        public void AddStatModifier(StatModifier modifier)
        {
            statModifiers.Add(modifier);
            RecalculateModifiers();
        }

        /// <summary>
        /// Удалить модификатор
        /// </summary>
        public void RemoveStatModifier(StatModifier modifier)
        {
            statModifiers.Remove(modifier);
            RecalculateModifiers();
        }

        private void RecalculateModifiers()
        {
            damageModifier = 1f;
            critChanceModifier = 0f;
            cooldownModifier = 1f;
            rangeModifier = 1f;
            attackSpeedModifier = 1f;

            foreach (var mod in statModifiers)
            {
                switch (mod.statType)
                {
                    case StatType.DamageMultiplier:
                        damageModifier *= mod.value;
                        break;
                    case StatType.CritChance:
                        critChanceModifier += mod.value;
                        break;
                    case StatType.AttackSpeed:
                        cooldownModifier /= mod.value; // Уменьшение кулдауна = увеличение скорости
                        break;
                    case StatType.Range:
                        rangeModifier *= mod.value;
                        break;
                }
            }
        }

        // Визуализация радиуса атаки
        private void OnDrawGizmosSelected()
        {
            if (currentWeapon == null) return;

            Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
            float range = currentWeapon.range * rangeModifier;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos, range);
        }
    }

    /// <summary>
    /// Модификатор статистики от перка
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        public float value;
    }

    public enum StatType
    {
        Damage,
        DamageMultiplier,
        CritChance,
        AttackSpeed,
        Range,
        MoveSpeed,
        MaxHealth
    }
}

