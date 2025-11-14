# 🎨 ТРЕТИЙ FOLLOW-UP PROMPT ДЛЯ CURSOR: UI System & Final Integration

> 🧠 Использовать только после создания всех C# скриптов и базовых сцен

---

## 🎯 ЗАДАНИЕ:

Создай **полноценную UI систему** для MegaBonk 3D и **свяжи все компоненты** в единую игру.
Добавь все необходимые Canvas, UI элементы, анимации и **UnityEvent связи** между скриптами и интерфейсом.

---

## 🖼️ UI СИСТЕМА - СОЗДАТЬ ФАЙЛЫ:

### 📁 `Assets/_Project/Scripts/UI/`

**Дополнительные UI скрипты:**

* `UIManager.cs` — центральный менеджер всех UI панелей
* `HUDController.cs` — обновление HP, XP, Level, Coins, Timer
* `LevelUpMenu.cs` — окно выбора апгрейдов (3 кнопки)
* `MenuController.cs` — главное меню (Start, Exit, Settings)
* `PauseMenu.cs` — пауза игры (Resume, Restart, Main Menu)
* `GameOverScreen.cs` — экран поражения (Restart, Main Menu)
* `VictoryScreen.cs` — экран победы (Next Level, Main Menu)
* `SettingsMenu.cs` — настройки (Volume, Graphics, Controls)

### 📁 `Assets/_Project/Scripts/UI/Components/`

* `HealthBar.cs` — анимированная полоса здоровья
* `XPBar.cs` — анимированная полоса опыта
* `CoinCounter.cs` — счетчик монет с анимацией
* `LevelDisplay.cs` — отображение текущего уровня
* `TimerDisplay.cs` — обратный отсчет до босса
* `UpgradeButton.cs` — кнопка выбора апгрейда

---

## 🎨 UI СЦЕНЫ И CANVAS:

### 1. **MainMenu Scene** (`MainMenu.unity`):

**Canvas Structure:**
```
MainMenuCanvas (Screen Space - Overlay)
├── Background (Image - темный фон)
├── Title (Text - "MegaBonk 3D")
├── StartButton (Button - "Start Game")
├── SettingsButton (Button - "Settings")
├── ExitButton (Button - "Exit")
└── SettingsPanel (Panel - скрыт по умолчанию)
    ├── VolumeSlider (Slider)
    ├── GraphicsDropdown (Dropdown)
    └── BackButton (Button)
```

### 2. **Game Scene** (`Level1_Forest.unity`):

**Canvas Structure:**
```
GameCanvas (Screen Space - Overlay)
├── HUD (Panel)
│   ├── HealthBar (Slider + Text)
│   ├── XPBar (Slider + Text)
│   ├── LevelText (Text)
│   ├── CoinCounter (Text + Icon)
│   ├── TimerText (Text)
│   └── PauseButton (Button)
├── LevelUpPanel (Panel - скрыт)
│   ├── Background (Image - полупрозрачный)
│   ├── Title (Text - "Level Up!")
│   ├── Upgrade1Button (Button + Text + Icon)
│   ├── Upgrade2Button (Button + Text + Icon)
│   ├── Upgrade3Button (Button + Text + Icon)
│   └── Description (Text)
├── PausePanel (Panel - скрыт)
│   ├── ResumeButton (Button)
│   ├── RestartButton (Button)
│   └── MainMenuButton (Button)
└── GameOverPanel (Panel - скрыт)
    ├── GameOverText (Text)
    ├── RestartButton (Button)
    └── MainMenuButton (Button)
```

### 3. **Boss Scene** (`BossArena.unity`):

**Canvas Structure:**
```
BossCanvas (Screen Space - Overlay)
├── HUD (Panel) — как в Game Scene
├── BossHealthBar (Slider + Text)
├── BossPhaseIndicator (Text)
└── VictoryPanel (Panel - скрыт)
    ├── VictoryText (Text)
    ├── NextLevelButton (Button)
    └── MainMenuButton (Button)
```

---

## 🔗 СВЯЗЫВАНИЕ СИСТЕМ ЧЕРЕЗ UNITYEVENT:

### 1. **PlayerHealth → UI:**

```csharp
// В PlayerHealth.cs
[System.Serializable]
public class HealthEvent : UnityEvent<float, float> { } // currentHP, maxHP

public HealthEvent OnHealthChanged;

// В HUDController.cs
public void UpdateHealthBar(float currentHP, float maxHP)
{
    healthBar.value = currentHP / maxHP;
    healthText.text = $"{currentHP:F0}/{maxHP:F0}";
}
```

### 2. **XPManager → UI:**

```csharp
// В XPManager.cs
[System.Serializable]
public class XPEvent : UnityEvent<int, int, int> { } // currentXP, requiredXP, level

public XPEvent OnXPChanged;

// В HUDController.cs
public void UpdateXPBar(int currentXP, int requiredXP, int level)
{
    xpBar.value = (float)currentXP / requiredXP;
    xpText.text = $"{currentXP}/{requiredXP}";
    levelText.text = $"Level {level}";
}
```

### 3. **UpgradeManager → UI:**

```csharp
// В UpgradeManager.cs
[System.Serializable]
public class LevelUpEvent : UnityEvent<UpgradeOption[]> { }

public LevelUpEvent OnLevelUp;

// В LevelUpMenu.cs
public void ShowLevelUpMenu(UpgradeOption[] options)
{
    levelUpPanel.SetActive(true);
    Time.timeScale = 0f; // Пауза игры
    
    // Заполнить кнопки апгрейдов
    for (int i = 0; i < options.Length; i++)
    {
        upgradeButtons[i].SetUpgrade(options[i]);
    }
}
```

### 4. **GameManager → UI:**

```csharp
// В GameManager.cs
[System.Serializable]
public class GameStateEvent : UnityEvent<GameState> { }

public GameStateEvent OnGameStateChanged;

// В UIManager.cs
public void OnGameStateChanged(GameState newState)
{
    switch (newState)
    {
        case GameState.Playing:
            ShowHUD();
            break;
        case GameState.Paused:
            ShowPauseMenu();
            break;
        case GameState.GameOver:
            ShowGameOverScreen();
            break;
        case GameState.Victory:
            ShowVictoryScreen();
            break;
    }
}
```

---

## 🎮 INPUT SYSTEM ИНТЕГРАЦИЯ:

### Создай Input Actions (`Assets/_Project/Input/PlayerInputActions.inputactions`):

```json
{
    "name": "PlayerInputActions",
    "maps": [
        {
            "name": "Player",
            "actions": [
                {"name": "Move", "type": "Value", "expectedControlType": "Vector2"},
                {"name": "Attack", "type": "Button", "expectedControlType": "Button"},
                {"name": "Interact", "type": "Button", "expectedControlType": "Button"},
                {"name": "Pause", "type": "Button", "expectedControlType": "Button"}
            ],
            "bindings": [
                {"name": "WASD", "path": "2DVector", "interactions": ""},
                {"name": "Left Click", "path": "<Mouse>/leftButton", "interactions": ""},
                {"name": "E", "path": "<Keyboard>/e", "interactions": ""},
                {"name": "Escape", "path": "<Keyboard>/escape", "interactions": ""}
            ]
        }
    ]
}
```

---

## 🎨 UI АНИМАЦИИ И ЭФФЕКТЫ:

### 1. **HealthBar анимация:**

```csharp
// В HealthBar.cs
public void UpdateHealth(float newHealth, float maxHealth)
{
    float targetValue = newHealth / maxHealth;
    StartCoroutine(AnimateHealthBar(targetValue));
}

private IEnumerator AnimateHealthBar(float targetValue)
{
    float startValue = healthBar.value;
    float duration = 0.5f;
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        healthBar.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
        yield return null;
    }
}
```

### 2. **LevelUp эффекты:**

```csharp
// В LevelUpMenu.cs
public void ShowLevelUpEffect()
{
    // Вспышка экрана
    StartCoroutine(FlashEffect());
    
    // Частицы LevelUp
    levelUpParticles.Play();
    
    // Звук LevelUp
    AudioManager.PlaySound("level_up");
}
```

### 3. **Coin counter анимация:**

```csharp
// В CoinCounter.cs
public void AddCoins(int amount)
{
    currentCoins += amount;
    StartCoroutine(AnimateCoinCounter());
}

private IEnumerator AnimateCoinCounter()
{
    // Анимация увеличения текста
    coinText.transform.localScale = Vector3.one * 1.2f;
    yield return new WaitForSeconds(0.1f);
    coinText.transform.localScale = Vector3.one;
}
```

---

## 🎵 АУДИО СИСТЕМА:

### Создай `AudioManager.cs`:

```csharp
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = false;
    }
    
    public Sound[] sounds;
    private Dictionary<string, AudioSource> audioSources;
    
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void PlaySound(string soundName)
    {
        if (Instance != null)
        {
            Instance.Play(soundName);
        }
    }
}
```

---

## 🎯 ФИНАЛЬНАЯ ИНТЕГРАЦИЯ:

### 1. **Создай GameManager Singleton:**

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public GameState currentState = GameState.Playing;
    
    [Header("UI References")]
    public UIManager uiManager;
    
    [Header("Player References")]
    public PlayerController player;
    public PlayerStats playerStats;
    
    [Header("Systems")]
    public XPManager xpManager;
    public UpgradeManager upgradeManager;
    public OverrunManager overrunManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        // Подключить все события
        playerStats.OnHealthChanged.AddListener(uiManager.UpdateHealthBar);
        xpManager.OnXPChanged.AddListener(uiManager.UpdateXPBar);
        upgradeManager.OnLevelUp.AddListener(uiManager.ShowLevelUpMenu);
        
        // Начать игру
        ChangeGameState(GameState.Playing);
    }
}
```

### 2. **Создай UIManager:**

```csharp
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject hudPanel;
    public GameObject levelUpPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    [Header("HUD Elements")]
    public Slider healthBar;
    public Slider xpBar;
    public Text healthText;
    public Text xpText;
    public Text levelText;
    public Text coinText;
    public Text timerText;
    
    [Header("Level Up Elements")]
    public Button[] upgradeButtons;
    public Text descriptionText;
    
    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        healthBar.value = currentHP / maxHP;
        healthText.text = $"{currentHP:F0}/{maxHP:F0}";
    }
    
    public void UpdateXPBar(int currentXP, int requiredXP, int level)
    {
        xpBar.value = (float)currentXP / requiredXP;
        xpText.text = $"{currentXP}/{requiredXP}";
        levelText.text = $"Level {level}";
    }
    
    public void ShowLevelUpMenu(UpgradeOption[] options)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
        
        for (int i = 0; i < options.Length; i++)
        {
            upgradeButtons[i].GetComponent<UpgradeButton>().SetUpgrade(options[i]);
        }
    }
}
```

---

## ✅ РЕЗУЛЬТАТ:

После выполнения этого промпта у вас будет:

🎮 **Полноценная игра MegaBonk 3D** с:
- ✅ Рабочим главным меню
- ✅ Игровым HUD с анимациями
- ✅ Системой Level Up с выбором апгрейдов
- ✅ Паузой и настройками
- ✅ Экранами победы и поражения
- ✅ Звуками и эффектами
- ✅ Полной интеграцией всех систем

🚀 **Готовый к запуску Unity проект** без дополнительных правок!

---

💡 *Этот промпт завершает создание полноценной игры. После его выполнения у вас будет готовая к демонстрации версия MegaBonk 3D.*
