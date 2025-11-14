using UnityEngine;
using UnityEngine.AI;
using MegabonkMobile.Core;
using MegabonkMobile.Data;
using System;

namespace MegabonkMobile.AI
{
    /// <summary>
    /// Базовый класс врага с FSM
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Enemy Data")]
        [SerializeField] private EnemyDef enemyDef;

        [Header("References")]
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField] private Transform playerTarget;

        // FSM
        private EnemyState currentState = EnemyState.Idle;
        private float stateTimer = 0f;

        // Health
        private float currentHealth;
        public bool IsAlive => currentHealth > 0f;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => enemyDef != null ? enemyDef.maxHealth : 100f;

        // Attack
        private float lastAttackTime = 0f;

        // События
        public event Action<EnemyBase> OnDeath;
        public event Action<EnemyBase> OnTakeDamage;
        public static event Action<EnemyBase> OnDeathStatic;

        private void Awake()
        {
            if (navAgent == null)
                navAgent = GetComponent<NavMeshAgent>();

            if (playerTarget == null)
            {
                Gameplay.PlayerController player = FindObjectOfType<Gameplay.PlayerController>();
                if (player != null)
                    playerTarget = player.transform;
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (enemyDef == null)
            {
                Debug.LogError($"EnemyBase {gameObject.name}: нет EnemyDef!");
                return;
            }

            currentHealth = enemyDef.maxHealth;
            
            // Настроить NavMeshAgent
            navAgent.speed = enemyDef.moveSpeed;
            navAgent.stoppingDistance = enemyDef.attackRange * 0.8f;

            ChangeState(EnemyState.Idle);
        }

        private void Update()
        {
            if (!IsAlive) return;

            UpdateFSM();
        }

        private void UpdateFSM()
        {
            stateTimer += Time.deltaTime;

            switch (currentState)
            {
                case EnemyState.Idle:
                    State_Idle();
                    break;
                case EnemyState.Chase:
                    State_Chase();
                    break;
                case EnemyState.Attack:
                    State_Attack();
                    break;
                case EnemyState.Stagger:
                    State_Stagger();
                    break;
                case EnemyState.Dead:
                    // Ничего не делать
                    break;
            }
        }

        private void State_Idle()
        {
            // Проверить, виден ли игрок
            if (playerTarget != null)
            {
                float distance = Vector3.Distance(transform.position, playerTarget.position);
                if (distance <= enemyDef.detectionRange)
                {
                    ChangeState(EnemyState.Chase);
                }
            }
        }

        private void State_Chase()
        {
            if (playerTarget == null) return;

            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // Проверить, можно ли атаковать
            if (distance <= enemyDef.attackRange)
            {
                ChangeState(EnemyState.Attack);
                return;
            }

            // Проверить, не ушел ли игрок слишком далеко
            if (distance > enemyDef.detectionRange * 1.5f)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            // Движение к игроку в зависимости от паттерна
            MoveTowardsPlayer();
        }

        private void State_Attack()
        {
            if (playerTarget == null)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // Если игрок ушел слишком далеко, преследовать
            if (distance > enemyDef.attackRange * 1.2f)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // Атаковать
            if (Time.time >= lastAttackTime + enemyDef.attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }

            // Повернуться к игроку
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0f;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 5f
                );
            }
        }

        private void State_Stagger()
        {
            // Временное состояние после получения урона
            if (stateTimer >= 0.3f)
            {
                ChangeState(EnemyState.Chase);
            }
        }

        private void MoveTowardsPlayer()
        {
            if (playerTarget == null || navAgent == null) return;

            Vector3 targetPosition = playerTarget.position;

            // Применить паттерн движения
            switch (enemyDef.aiPattern)
            {
                case AIPattern.Rusher:
                    // Прямо на игрока
                    navAgent.SetDestination(targetPosition);
                    break;

                case AIPattern.ZigZag:
                    // Зигзагообразное движение
                    Vector3 zigzagOffset = new Vector3(
                        Mathf.Sin(Time.time * 3f) * 2f,
                        0f,
                        Mathf.Cos(Time.time * 3f) * 2f
                    );
                    navAgent.SetDestination(targetPosition + zigzagOffset);
                    break;

                case AIPattern.Shooter:
                    // Держать дистанцию
                    Vector3 direction = (transform.position - targetPosition).normalized;
                    Vector3 preferredPosition = targetPosition + direction * (enemyDef.attackRange * 1.5f);
                    navAgent.SetDestination(preferredPosition);
                    break;

                case AIPattern.Suicide:
                    // Быстро к игроку
                    navAgent.speed = enemyDef.moveSpeed * 1.5f;
                    navAgent.SetDestination(targetPosition);
                    break;
            }
        }

        private void PerformAttack()
        {
            if (playerTarget == null) return;

            IDamageable damageable = playerTarget.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                DamageInfo damageInfo = new DamageInfo(
                    enemyDef.attackDamage,
                    transform.position,
                    false,
                    3f
                );

                damageable.TakeDamage(damageInfo);

                // Для Suicide врага - взорваться
                if (enemyDef.aiPattern == AIPattern.Suicide)
                {
                    Explode();
                }
            }
        }

        private void Explode()
        {
            // AoE урон вокруг
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
            foreach (var hitCollider in hitColliders)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null && damageable != this)
                {
                    DamageInfo damageInfo = new DamageInfo(
                        enemyDef.attackDamage * 2f,
                        transform.position,
                        false,
                        5f
                    );
                    damageable.TakeDamage(damageInfo);
                }
            }

            Die();
        }

        private void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            stateTimer = 0f;

            // Действия при смене состояния
            switch (currentState)
            {
                case EnemyState.Idle:
                    navAgent.isStopped = true;
                    break;
                case EnemyState.Chase:
                    navAgent.isStopped = false;
                    break;
                case EnemyState.Attack:
                    navAgent.isStopped = true;
                    break;
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive) return;

            currentHealth = Mathf.Max(0f, currentHealth - damageInfo.damage);
            OnTakeDamage?.Invoke(this);

            // Нокбек
            if (damageInfo.knockbackForce > 0f)
            {
                Vector3 knockbackDirection = (transform.position - damageInfo.sourcePosition).normalized;
                knockbackDirection.y = 0f;
                GetComponent<Rigidbody>()?.AddForce(knockbackDirection * damageInfo.knockbackForce, ForceMode.Impulse);
                
                ChangeState(EnemyState.Stagger);
            }

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            if (!IsAlive) return;

            ChangeState(EnemyState.Dead);
            navAgent.isStopped = true;

            OnDeath?.Invoke(this);
            OnDeathStatic?.Invoke(this);

            // Вернуть в пул или уничтожить
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

        /// <summary>
        /// Инициализировать врага с данными
        /// </summary>
        public void Initialize(EnemyDef def)
        {
            enemyDef = def;
            Initialize();
        }

        /// <summary>
        /// Получить EnemyDef
        /// </summary>
        public EnemyDef GetEnemyDef() => enemyDef;

        /// <summary>
        /// Применить усиление (от AIDirector)
        /// </summary>
        public void ApplyPowerUp(float multiplier)
        {
            if (enemyDef == null) return;

            currentHealth *= multiplier;
            enemyDef.maxHealth *= multiplier;
            enemyDef.moveSpeed *= multiplier;
            enemyDef.attackDamage *= multiplier;
            
            if (navAgent != null)
            {
                navAgent.speed = enemyDef.moveSpeed;
            }
        }
    }

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Stagger,
        Dead
    }
}

