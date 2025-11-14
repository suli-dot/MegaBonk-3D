using UnityEngine;

namespace MegabonkMobile.Core
{
    /// <summary>
    /// Интерфейс для объектов, которые могут получать урон
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(DamageInfo damageInfo);
        bool IsAlive { get; }
        float CurrentHealth { get; }
        float MaxHealth { get; }
    }

    /// <summary>
    /// Информация об уроне
    /// </summary>
    public struct DamageInfo
    {
        public float damage;
        public Vector3 sourcePosition;
        public bool isCritical;
        public float knockbackForce;
        
        public DamageInfo(float damage, Vector3 sourcePosition, bool isCritical = false, float knockbackForce = 0f)
        {
            this.damage = damage;
            this.sourcePosition = sourcePosition;
            this.isCritical = isCritical;
            this.knockbackForce = knockbackForce;
        }
    }
}

