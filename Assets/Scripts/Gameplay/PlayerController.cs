using UnityEngine;
using MegabonkMobile.Core;
using System;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Контроллер игрока для top-down вида с мобильным управлением
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 15f;
        
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("Invincibility")]
        [SerializeField] private float invincibilityDuration = 0.5f;
        [SerializeField] private bool isInvincible = false;
        
        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform visualTransform;

        // Input
        private Vector2 moveInput;
        private Vector3 moveDirection;

        // Health
        public bool IsAlive => currentHealth > 0f;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        // События
        public event Action<float, float> OnHealthChanged; // current, max
        public event Action OnDeath;
        public event Action OnTakeDamage;

        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            
            currentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleInput()
        {
            // Мобильный ввод через виртуальный стик
            // В Unity это будет через Input System или UI Virtual Joystick
            // Здесь заглушка - в реальности будет подключен к UI Joystick
            
            // Для тестирования на PC можно использовать WASD
            #if UNITY_EDITOR || UNITY_STANDALONE
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
            #endif
        }

        /// <summary>
        /// Установить ввод движения (вызывается из UI Virtual Joystick)
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input.normalized;
        }

        private void HandleMovement()
        {
            if (!IsAlive) return;

            // Вычислить направление движения
            moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            // Применить движение через Rigidbody
            Vector3 targetVelocity = moveDirection * moveSpeed;
            Vector3 velocityChange = targetVelocity - rb.velocity;
            velocityChange.y = 0f; // Не трогать вертикальную скорость

            rb.AddForce(velocityChange * acceleration, ForceMode.VelocityChange);

            // Поворот визуала в направлении движения
            if (moveDirection.magnitude > 0.1f && visualTransform != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                visualTransform.rotation = Quaternion.Slerp(
                    visualTransform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * 10f
                );
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive || isInvincible) return;

            currentHealth = Mathf.Max(0f, currentHealth - damageInfo.damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnTakeDamage?.Invoke();

            // Нокбек
            if (damageInfo.knockbackForce > 0f)
            {
                Vector3 knockbackDirection = (transform.position - damageInfo.sourcePosition).normalized;
                knockbackDirection.y = 0f;
                rb.AddForce(knockbackDirection * damageInfo.knockbackForce, ForceMode.Impulse);
            }

            // Ифреймы
            if (invincibilityDuration > 0f)
            {
                StartCoroutine(InvincibilityCoroutine());
            }

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibilityDuration);
            isInvincible = false;
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();
            Core.GameController.Instance?.GameOver();
        }

        // Визуализация в редакторе
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}

