# 🎮 MegaBonk 3D - Инструкция по настройке проекта

## ✅ Что уже сделано

- ✅ Создана полная структура папок проекта
- ✅ Созданы все C# скрипты (30+ файлов)
- ✅ Реализованы все игровые системы
- ✅ Настроен Input System

## 📋 Что нужно сделать в Unity Editor

### 1. Настройка тегов и слоев

**Edit → Project Settings → Tags and Layers**

**Добавить теги:**
- `Player`
- `Enemy`
- `Boss`
- `Loot`
- `Interactable`

**Настроить слои:**
- Layer 8: `Player`
- Layer 9: `Enemy`
- Layer 10: `Loot`
- Layer 11: `UI`

### 2. Настройка Input System

1. Откройте `Assets/InputSystem_Actions.inputactions`
2. Убедитесь, что файл правильно настроен
3. В **Edit → Project Settings → Player → Other Settings**:
   - **Active Input Handling**: Both (или Input System Package (New))

### 3. Создание префабов

#### Player Prefab:
1. Создайте GameObject: `Capsule` → назовите `Player`
2. Добавьте компоненты:
   - `Rigidbody` (Mass: 1, Drag: 5)
   - `Capsule Collider`
   - `PlayerController`
   - `PlayerStats`
   - `PlayerAttack`
   - `PlayerHealth`
3. Настройте тег: `Player`
4. Настройте слой: `Player`
5. Сохраните как префаб: `Assets/_Project/Prefabs/Player.prefab`

#### Enemy Prefabs:
Для каждого типа врага (Slime, Skeleton, Bat, Demon):
1. Создайте GameObject (можно использовать простой Cube/Capsule)
2. Добавьте компоненты:
   - `NavMeshAgent`
   - `Capsule Collider`
   - `EnemyController`
   - `EnemyHealth`
3. Настройте тег: `Enemy`
4. Настройте слой: `Enemy`
5. Сохраните как префаб

#### Boss Prefab:
1. Создайте GameObject (больше обычного врага)
2. Добавьте компоненты:
   - `NavMeshAgent`
   - `Capsule Collider`
   - `BossController`
   - `EnemyHealth` (с большим HP)
3. Настройте тег: `Boss`
4. Сохраните как префаб

#### Interactables:
- **Chest**: GameObject с `Chest.cs` + Collider (IsTrigger)
- **Vase**: GameObject с `Vase.cs` + Rigidbody + Collider
- **Artifact**: GameObject с `Artifact.cs` + Collider (IsTrigger)
- **HealthPickup**: GameObject с `HealthPickup.cs` + Collider (IsTrigger)
- **XPOrb**: GameObject с `XPOrb.cs` + Collider (IsTrigger)

### 4. Настройка сцен

#### MainMenu.unity:
1. Создайте Canvas (Screen Space - Overlay)
2. Добавьте UI элементы:
   - Title Text: "MegaBonk 3D"
   - Start Button → подключите к `MenuController.OnStartClicked()`
   - Settings Button → подключите к `MenuController.OnSettingsClicked()`
   - Exit Button → подключите к `MenuController.OnExitClicked()`
3. Добавьте GameObject с `MenuController.cs`

#### Level1_Forest.unity:
1. Создайте Terrain (100x100 единиц)
2. Добавьте Directional Light
3. Создайте Player из префаба
4. Добавьте камеру с `ThirdPersonCamera.cs`
5. Создайте Canvas для HUD:
   - Health Bar (Slider)
   - XP Bar (Slider)
   - Level Text
   - Coin Text
   - Timer Text
6. Добавьте GameObject с `GameManager.cs`
7. Добавьте GameObject с `XPManager.cs`
8. Добавьте GameObject с `UpgradeManager.cs`
9. Добавьте GameObject с `OverrunManager.cs`
10. Добавьте GameObject с `LootSystem.cs`
11. Добавьте GameObject с `EnemySpawner.cs` → назначьте префабы врагов
12. Добавьте GameObject с `UIManager.cs` → назначьте все UI панели

#### BossArena.unity:
Аналогично Level1_Forest, но:
- Меньшая арена
- Точка спавна босса
- Boss Health Bar в UI

### 5. Настройка NavMesh

1. Выделите Terrain/Floor
2. **Window → AI → Navigation**
3. Выделите объекты для навигации
4. Нажмите **Bake** в Navigation window
5. Убедитесь, что `EnemyController` и `BossController` используют NavMeshAgent

### 6. Настройка апгрейдов

1. **Assets → Create → MegaBonk3D → Upgrade Data**
2. Создайте несколько ScriptableObjects с апгрейдами:
   - Attack Boost (+5 урона)
   - Health Boost (+20 HP)
   - Speed Boost (+1 скорость)
   - Critical Boost (+5% крит)
3. В `UpgradeManager` назначьте эти ScriptableObjects в соответствующие списки

### 7. Настройка лута

В `LootSystem` назначьте префабы:
- Coin Prefab
- HealthPotion Prefab
- XPOrb Prefab
- Artifact Prefab

### 8. Настройка звуков

В `AudioManager` добавьте звуки в массив `sounds`:
- `attack` - звук атаки
- `level_up` - звук повышения уровня
- `pickup` - звук подбора
- `death` - звук смерти врага

### 9. Настройка Build Settings

**File → Build Settings:**
1. Добавьте сцены:
   - [0] MainMenu
   - [1] Level1_Forest
   - [2] BossArena
2. Выберите платформу (PC, Android и т.д.)

## 🔧 Важные настройки компонентов

### PlayerController:
- `Move Speed`: 5
- `Jump Force`: 10
- `Mouse Sensitivity`: 2
- `Ground Layer Mask`: Player layer

### PlayerAttack:
- `Attack Range`: 2
- `Attack Cooldown`: 1
- `Enemy Layer Mask`: Enemy layer

### EnemyController:
- `Detection Range`: 10
- `Attack Range`: 2
- `Attack Cooldown`: 2

### EnemySpawner:
- `Spawn Interval`: 5
- `Max Enemies`: 20
- `Spawn Radius`: 20

### OverrunManager:
- `Enemy Power Up Interval`: 60 (1 минута)
- `Power Up Multiplier`: 1.2
- `Boss Spawn Time`: 600 (10 минут)

## 🐛 Решение проблем

### Ошибка "Input System not found":
→ Установите Input System Package через Package Manager

### Враги не двигаются:
→ Проверьте, что NavMesh запечен и NavMeshAgent настроен

### UI не обновляется:
→ Проверьте, что все ссылки назначены в Inspector

### Апгрейды не применяются:
→ Убедитесь, что `UpgradeManager` имеет ссылки на ScriptableObjects

## ✅ Чеклист перед запуском

- [ ] Теги и слои настроены
- [ ] Префабы созданы и настроены
- [ ] Сцены настроены с GameObjects
- [ ] NavMesh запечен
- [ ] Input System Actions назначен
- [ ] Все ссылки в Inspector заполнены
- [ ] UI Canvas создан и настроен
- [ ] Звуки назначены (опционально)
- [ ] Build Settings настроены

## 🚀 Запуск игры

1. Откройте сцену `MainMenu.unity`
2. Нажмите **Play**
3. Нажмите **Start Game**
4. Игра должна запуститься!

---

**Примечание:** Этот проект требует настройки в Unity Editor. Все скрипты готовы, но визуальные компоненты, префабы и связи нужно настроить вручную через Inspector.

