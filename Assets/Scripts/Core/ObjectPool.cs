using UnityEngine;
using System.Collections.Generic;

namespace MegabonkMobile.Core
{
    /// <summary>
    /// Универсальный Object Pool для оптимизации на мобильных устройствах
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class PoolItem
        {
            public GameObject prefab;
            public int initialSize = 10;
            public int maxSize = 50;
        }

        [Header("Pool Settings")]
        [SerializeField] private PoolItem[] poolItems;
        
        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, PoolItem> poolConfigs = new Dictionary<string, PoolItem>();
        private Dictionary<GameObject, string> activeObjects = new Dictionary<GameObject, string>();

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var item in poolItems)
            {
                if (item.prefab == null) continue;

                string poolName = item.prefab.name;
                Queue<GameObject> pool = new Queue<GameObject>();

                // Предварительно создать объекты
                for (int i = 0; i < item.initialSize; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    pool.Enqueue(obj);
                }

                pools[poolName] = pool;
                poolConfigs[poolName] = item;
            }
        }

        /// <summary>
        /// Получить объект из пула
        /// </summary>
        public GameObject Get(string poolName)
        {
            if (!pools.ContainsKey(poolName))
            {
                Debug.LogError($"Pool '{poolName}' не найден!");
                return null;
            }

            GameObject obj;

            if (pools[poolName].Count > 0)
            {
                obj = pools[poolName].Dequeue();
            }
            else
            {
                // Создать новый объект, если пул пуст
                PoolItem config = poolConfigs[poolName];
                if (config != null)
                {
                    obj = Instantiate(config.prefab);
                }
                else
                {
                    Debug.LogError($"Конфигурация для пула '{poolName}' не найдена!");
                    return null;
                }
            }

            obj.SetActive(true);
            activeObjects[obj] = poolName;
            return obj;
        }

        /// <summary>
        /// Вернуть объект в пул
        /// </summary>
        public void Return(GameObject obj)
        {
            if (obj == null) return;

            if (!activeObjects.ContainsKey(obj))
            {
                Debug.LogWarning($"Объект {obj.name} не был взят из пула!");
                Destroy(obj);
                return;
            }

            string poolName = activeObjects[obj];
            activeObjects.Remove(obj);

            // Проверить лимит размера пула
            PoolItem config = poolConfigs[poolName];
            if (pools[poolName].Count >= config.maxSize)
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pools[poolName].Enqueue(obj);
        }

        /// <summary>
        /// Вернуть объект в пул через время
        /// </summary>
        public void ReturnAfterDelay(GameObject obj, float delay)
        {
            StartCoroutine(ReturnAfterDelayCoroutine(obj, delay));
        }

        private System.Collections.IEnumerator ReturnAfterDelayCoroutine(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Return(obj);
        }
    }
}

