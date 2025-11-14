using UnityEngine;
using MegabonkMobile.Core;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Подбираемое здоровье
    /// </summary>
    public class HealthPickup : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float healAmount = 25f;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float attractionRange = 5f;
        [SerializeField] private float attractionSpeed = 5f;

        [Header("Visual")]
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private float floatAmount = 0.3f;

        private Vector3 startPosition;
        private Transform playerTarget;
        private bool isAttracted = false;

        private void Start()
        {
            startPosition = transform.position;
            
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
                playerTarget = player.transform;
        }

        private void Update()
        {
            if (playerTarget == null) return;

            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // Притяжение к игроку
            if (distance <= attractionRange)
            {
                isAttracted = true;
                Vector3 direction = (playerTarget.position - transform.position).normalized;
                transform.position += direction * attractionSpeed * Time.deltaTime;
            }
            else
            {
                // Парение
                float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            // Вращение
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Подбор
            if (distance <= pickupRange)
            {
                Pickup();
            }
        }

        private void Pickup()
        {
            // Восстановить здоровье игроку
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.Heal(healAmount);
            }

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
    }
}

