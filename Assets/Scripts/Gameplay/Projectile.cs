using UnityEngine;
using MegabonkMobile.Core;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Снаряд для дальнего оружия
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private GameObject hitEffectPrefab;

        private float damage;
        private float speed;
        private float maxDistance;
        private LayerMask targetLayerMask;
        private Vector3 startPosition;
        private Rigidbody rb;
        private bool hasHit = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Инициализировать снаряд
        /// </summary>
        public void Initialize(float damage, Vector3 direction, float speed, float maxDistance, LayerMask targetLayerMask)
        {
            this.damage = damage;
            this.speed = speed;
            this.maxDistance = maxDistance;
            this.targetLayerMask = targetLayerMask;
            this.startPosition = transform.position;
            this.hasHit = false;

            rb.velocity = direction.normalized * speed;

            // Уничтожить через время
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // Проверить максимальную дистанцию
            float distanceTraveled = Vector3.Distance(transform.position, startPosition);
            if (distanceTraveled >= maxDistance)
            {
                DestroyProjectile();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;

            // Проверить, что это враг
            if ((targetLayerMask.value & (1 << other.gameObject.layer)) == 0)
                return;

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                DamageInfo damageInfo = new DamageInfo(
                    damage,
                    transform.position,
                    false,
                    2f
                );

                damageable.TakeDamage(damageInfo);
                hasHit = true;

                // Эффект попадания
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                }

                DestroyProjectile();
            }
        }

        private void DestroyProjectile()
        {
            // Попытаться вернуть в пул
            ObjectPool pool = FindObjectOfType<ObjectPool>();
            if (pool != null)
            {
                pool.Return(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

