using UnityEngine;
using MegabonkMobile.Core;
using MegabonkMobile.Gameplay;
using MegabonkMobile.AI;

namespace MegabonkMobile.UI
{
    /// <summary>
    /// Контроллер HUD - логика обновления интерфейса (без Canvas)
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private XPManager xpManager;
        [SerializeField] private WaveSpawner waveSpawner;

        // События для UI (UI будет подписываться на эти события)
        public event System.Action<float, float> OnHealthChanged; // current, max
        public event System.Action<int, int, int> OnXPChanged; // currentXP, requiredXP, level
        public event System.Action<int> OnWaveChanged; // waveNumber
        public event System.Action<float> OnSurvivalTimeChanged; // seconds

        private float survivalTime = 0f;
        private bool isGameActive = false;

        private void Start()
        {
            // Найти компоненты
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();
            
            if (xpManager == null)
                xpManager = XPManager.Instance;
            
            if (waveSpawner == null)
                waveSpawner = FindObjectOfType<WaveSpawner>();

            // Подписаться на события
            if (playerController != null)
            {
                playerController.OnHealthChanged += OnPlayerHealthChanged;
            }

            if (xpManager != null)
            {
                xpManager.OnXPChanged += OnXPChangedHandler;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveStart += OnWaveStartHandler;
            }

            // Подписаться на события GameController
            if (GameController.Instance != null)
            {
                GameController.Instance.OnGameStart += OnGameStart;
                GameController.Instance.OnGameOver += OnGameOver;
            }
        }

        private void Update()
        {
            if (isGameActive)
            {
                survivalTime += Time.deltaTime;
                OnSurvivalTimeChanged?.Invoke(survivalTime);
            }
        }

        private void OnDestroy()
        {
            if (playerController != null)
            {
                playerController.OnHealthChanged -= OnPlayerHealthChanged;
            }

            if (xpManager != null)
            {
                xpManager.OnXPChanged -= OnXPChangedHandler;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveStart -= OnWaveStartHandler;
            }
        }

        private void OnPlayerHealthChanged(float current, float max)
        {
            OnHealthChanged?.Invoke(current, max);
        }

        private void OnXPChangedHandler(int currentXP, int requiredXP, int level)
        {
            OnXPChanged?.Invoke(currentXP, requiredXP, level);
        }

        private void OnWaveStartHandler(int waveNumber)
        {
            OnWaveChanged?.Invoke(waveNumber);
        }

        private void OnGameStart()
        {
            isGameActive = true;
            survivalTime = 0f;
        }

        private void OnGameOver()
        {
            isGameActive = false;
        }
    }
}

