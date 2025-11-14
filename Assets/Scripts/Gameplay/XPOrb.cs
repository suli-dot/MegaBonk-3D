using UnityEngine;
using MegabonkMobile.Core;

namespace MegabonkMobile.Gameplay
{
    /// <summary>
    /// Сфера опыта - дает XP при подборе
    /// </summary>
    public class XPOrb : MonoBehaviour
    {
        [Header("XP Settings")]
        [SerializeField] private int xpAmount = 10;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float attractionRange = 5f;
        [SerializeField] private float attractionSpeed = 5f;

        [Header("Visual")]
        [SerializeField] private float rotationSpeed = 120f;
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
            // Добавить XP через XPManager
            XPManager xpManager = XPManager.Instance;
            if (xpManager != null)
            {
                xpManager.AddXP(xpAmount);
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

