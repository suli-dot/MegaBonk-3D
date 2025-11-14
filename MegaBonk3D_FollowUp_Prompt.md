# 🚀 FOLLOW-UP PROMPT ДЛЯ CURSOR: Автогенерация C# файлов

---

**ЗАДАНИЕ:**
Теперь, основываясь на структуре проекта MegaBonk 3D, **автоматически сгенерируй все C# скрипты** согласно техническому заданию выше.

**ТРЕБОВАНИЯ:**
- Создай ВСЕ файлы из структуры папок одновременно
- Каждый скрипт должен быть полностью функциональным
- Используй современные Unity практики (2022.3 LTS+)
- Добавь комментарии на русском языке
- Все публичные поля должны быть настроены через Inspector

---

## 📋 СПИСОК ФАЙЛОВ ДЛЯ СОЗДАНИЯ

### Player System:
- `Assets/_Project/Scripts/Player/PlayerController.cs`
- `Assets/_Project/Scripts/Player/PlayerAttack.cs`
- `Assets/_Project/Scripts/Player/PlayerHealth.cs`
- `Assets/_Project/Scripts/Player/PlayerStats.cs`

### Enemy System:
- `Assets/_Project/Scripts/Enemies/EnemyController.cs`
- `Assets/_Project/Scripts/Enemies/EnemyHealth.cs`
- `Assets/_Project/Scripts/Enemies/EnemySpawner.cs`

### Boss System:
- `Assets/_Project/Scripts/Enemies/BossController.cs`

### Systems:
- `Assets/_Project/Scripts/Systems/LootSystem.cs`
- `Assets/_Project/Scripts/Systems/XPManager.cs`
- `Assets/_Project/Scripts/Systems/UpgradeManager.cs`
- `Assets/_Project/Scripts/Systems/OverrunManager.cs`
- `Assets/_Project/Scripts/Systems/GameManager.cs`

### Interactables:
- `Assets/_Project/Scripts/Interactables/Chest.cs`
- `Assets/_Project/Scripts/Interactables/Vase.cs`
- `Assets/_Project/Scripts/Interactables/Artifact.cs`
- `Assets/_Project/Scripts/Interactables/HealthPickup.cs`
- `Assets/_Project/Scripts/Interactables/XPOrb.cs`

### UI:
- `Assets/_Project/Scripts/UI/HUDController.cs`
- `Assets/_Project/Scripts/UI/LevelUpMenu.cs`
- `Assets/_Project/Scripts/UI/MenuController.cs`

### Utilities:
- `Assets/_Project/Scripts/Utilities/ThirdPersonCamera.cs`

---

## 🎯 КЛЮЧЕВЫЕ ОСОБЕННОСТИ РЕАЛИЗАЦИИ

### PlayerController.cs:
```csharp
// Движение WASD + прыжок Space
// Поворот камеры мышью
// Анимации через Animator
// Rigidbody для физики
```

### EnemyController.cs:
```csharp
// NavMeshAgent для AI
// Состояния: Patrol, Chase, Attack, Dead
// Атака в радиусе при приближении к игроку
// Разные типы: Slime, Skeleton, Bat, Demon
```

### BossController.cs:
```csharp
// 3 фазы боя с разными атаками
// Спавн миньонов во 2 фазе
// Усиление в 3 фазе (ярость)
// Переход на следующий уровень после смерти
```

### LootSystem.cs:
```csharp
// Дроп: Coin(60%), HealthPotion(20%), XPOrb(15%), Artifact(5%)
// Подбор через триггеры или E-кнопку
// Артефакты дают постоянные бонусы
```

### XPManager.cs:
```csharp
// Формула XP: 100 * Level для следующего уровня
// Триггер LevelUp при достижении порога
// Интеграция с UpgradeManager
```

### UpgradeManager.cs:
```csharp
// Показ 3 случайных апгрейдов
// Категории: Attack, Defense, Speed, Passive, Critical
// Применение бонусов к PlayerStats
```

### OverrunManager.cs:
```csharp
// Усиление врагов каждую минуту
// Спавн босса через 10 минут
// Overrun Phase если босс не убит
```

### GameManager.cs:
```csharp
// Singleton паттерн
// Управление игровым циклом
// Сохранение через PlayerPrefs
// Переходы между сценами
```

---

## 🔧 ТЕХНИЧЕСКИЕ ДЕТАЛИ

### Используй:
- **Singleton** для GameManager, XPManager, UpgradeManager
- **Events/Delegates** для связи между системами
- **Coroutines** для таймеров и задержек
- **ScriptableObjects** для настроек врагов и апгрейдов
- **Object Pooling** для оптимизации спавна врагов и лута

### Inspector поля:
- Все настройки через SerializeField
- Tooltips для удобства настройки
- Range атрибуты для ограничения значений
- Header атрибуты для группировки

### Производительность:
- Кэширование компонентов в Start()
- Использование CompareTag вместо string сравнений
- Оптимизация Update() через события

---

## 📝 ПРИМЕР СТРУКТУРЫ КЛАССА

```csharp
using UnityEngine;
using System.Collections;

namespace MegaBonk3D.Player
{
    /// <summary>
    /// Контроллер игрока - движение, прыжки, поворот камеры
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float mouseSensitivity = 2f;
        
        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform cameraTransform;
        
        // Приватные переменные
        private Vector3 moveInput;
        private float mouseX, mouseY;
        private bool isGrounded;
        
        // События
        public System.Action OnJump;
        public System.Action OnLand;
        
        // Методы...
    }
}
```

---

## 🎮 ГОТОВЫЕ ПРЕФАБЫ

После создания скриптов, создай базовые префабы:
- **Player** (с PlayerController, PlayerAttack, PlayerHealth, PlayerStats)
- **Enemy_Slime** (с EnemyController, EnemyHealth)
- **Enemy_Skeleton** (с EnemyController, EnemyHealth)
- **Enemy_Bat** (с EnemyController, EnemyHealth)
- **Enemy_Demon** (с EnemyController, EnemyHealth)
- **Boss** (с BossController)
- **Chest** (с Chest.cs)
- **Vase** (с Vase.cs)
- **Artifact** (с Artifact.cs)
- **HealthPickup** (с HealthPickup.cs)
- **XPOrb** (с XPOrb.cs)
- **Coin** (с Coin.cs)

---

## 🎯 РЕЗУЛЬТАТ

После выполнения этого промпта у тебя должен быть:
✅ Полностью функциональный Unity проект
✅ Все C# скрипты с правильной архитектурой
✅ Готовые префабы для всех объектов
✅ Настроенные системы взаимодействия
✅ Базовая сцена с игроком и тестовыми врагами
✅ UI система с меню и HUD
✅ Система сохранения прогресса

**Проект готов к запуску и тестированию в Unity!**

---

**ВАЖНО:** Создавай файлы последовательно, проверяя зависимости между классами. Начни с базовых систем (GameManager, PlayerStats), затем переходи к более сложным (EnemyController, BossController).
