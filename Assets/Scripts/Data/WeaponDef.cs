using UnityEngine;

namespace MegabonkMobile.Data
{
    /// <summary>
    /// ScriptableObject с данными оружия
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "MegabonkMobile/Weapon")]
    public class WeaponDef : ScriptableObject
    {
        [Header("Weapon Info")]
        public string weaponName;
        public WeaponType type;
        public Sprite icon;

        [Header("Stats")]
        [Tooltip("Базовый урон")]
        public float baseDamage = 10f;
        
        [Tooltip("Шанс критического удара (0-1)")]
        [Range(0f, 1f)]
        public float critChance = 0.1f;
        
        [Tooltip("Множитель критического урона")]
        public float critMultiplier = 2f;
        
        [Tooltip("Время между атаками")]
        public float cooldown = 1f;
        
        [Tooltip("Дальность атаки")]
        public float range = 2f;
        
        [Tooltip("Скорость снаряда (для дальнего оружия)")]
        public float projectileSpeed = 10f;

        [Header("Projectile (для дальнего оружия)")]
        public GameObject projectilePrefab;
        
        [Header("Visual")]
        public GameObject hitEffectPrefab;
        public AudioClip attackSound;
    }

    public enum WeaponType
    {
        Melee,   // Ближний бой
        Ranged   // Дальний бой
    }
}

