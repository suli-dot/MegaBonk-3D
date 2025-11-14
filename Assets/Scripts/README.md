# 🎮 Megabonk Mobile - Архитектура кода

## 📁 Структура проекта

```
Assets/Scripts/
├── Core/              # Базовая инфраструктура
│   ├── IDamageable.cs
│   ├── ObjectPool.cs
│   └── GameController.cs
├── Gameplay/          # Игрок, оружие, лут
│   ├── PlayerController.cs
│   ├── WeaponSystem.cs
│   ├── Projectile.cs
│   ├── XPManager.cs
│   ├── LevelUpSystem.cs
│   ├── LootSystem.cs
│   ├── XPOrb.cs
│   └── HealthPickup.cs
├── AI/                # Враги, волны, AI Director
│   ├── EnemyBase.cs
│   ├── WaveSpawner.cs
│   └── AIDirector.cs
├── Data/              # ScriptableObjects
│   ├── WeaponDef.cs
│   ├── EnemyDef.cs
│   ├── Perk.cs
│   ├── WaveConfig.cs
│   └── LootTable.cs
├── UI/                # Логика UI (без Canvas)
│   ├── HUDController.cs
│   └── LevelUpMenuController.cs
├── Meta/              # Сохранения
│   └── SaveService.cs
└── Utilities/         # Утилиты
    └── AudioManager.cs
```

## 🔧 Как подключить системы в Unity

### 1. Core Systems

#### GameController
- Создайте пустой GameObject `GameController`
- Добавьте компонент `GameController`
- Этот объект будет DontDestroyOnLoad

#### ObjectPool
- Создайте пустой GameObject `ObjectPool`
- Добавьте компонент `ObjectPool`
- В Inspector назначьте префабы для пула (враги, снаряды, лут)

### 2. Player

#### PlayerController
- Создайте GameObject для игрока (например, Capsule)
- Добавьте компоненты:
  - `Rigidbody` (Mass: 1, Drag: 5, Freeze Rotation Y)
  - `Capsule Collider`
  - `PlayerController`
- Настройте тег: `Player`
- Настройте слой: `Player`

#### WeaponSystem
- Добавьте `WeaponSystem` на тот же GameObject что и PlayerController
- В Inspector назначьте:
  - `Current Weapon` (WeaponDef ScriptableObject)
  - `Attack Point` (Transform для точки атаки)
  - `Enemy Layer Mask` (слой врагов)

### 3. AI Systems

#### EnemyBase
- Создайте префабы врагов
- Добавьте компоненты:
  - `NavMeshAgent`
  - `Capsule Collider`
  - `Rigidbody`
  - `EnemyBase`
- Настройте тег: `Enemy`
- Настройте слой: `Enemy`

#### WaveSpawner
- Создайте пустой GameObject `WaveSpawner`
- Добавьте компонент `WaveSpawner`
- В Inspector назначьте:
  - `Wave Configs` (список WaveConfig ScriptableObjects)
  - `Spawn Center` (центр арены)
  - `Arena Radius` (радиус арены)

#### AIDirector
- Создайте пустой GameObject `AIDirector`
- Добавьте компонент `AIDirector`
- Настройте кривые множителей в Inspector

### 4. Gameplay Systems

#### XPManager
- Создайте пустой GameObject `XPManager`
- Добавьте компонент `XPManager`
- Этот объект будет DontDestroyOnLoad

#### LevelUpSystem
- Создайте пустой GameObject `LevelUpSystem`
- Добавьте компонент `LevelUpSystem`
- В Inspector назначьте все доступные `Perk` ScriptableObjects

#### LootSystem
- Создайте пустой GameObject `LootSystem`
- Добавьте компонент `LootSystem`

### 5. UI Systems

#### HUDController
- Создайте пустой GameObject `HUDController`
- Добавьте компонент `HUDController`
- Подпишите UI элементы на события:
  - `OnHealthChanged` → обновить Health Bar
  - `OnXPChanged` → обновить XP Bar и Level
  - `OnWaveChanged` → обновить номер волны
  - `OnSurvivalTimeChanged` → обновить таймер

#### LevelUpMenuController
- Создайте пустой GameObject `LevelUpMenuController`
- Добавьте компонент `LevelUpMenuController`
- Подпишите UI меню на события:
  - `OnShowPerkSelection` → показать меню с перками
  - `OnHidePerkSelection` → скрыть меню

### 6. Audio

#### AudioManager
- Создайте пустой GameObject `AudioManager`
- Добавьте компонент `AudioManager`
- В Inspector назначьте:
  - `Sound Clips` (массив звуков)
  - `Music Tracks` (массив музыки)

### 7. ScriptableObjects

Создайте через **Assets → Create → MegabonkMobile**:

- **WeaponDef** - данные оружия
- **EnemyDef** - данные врагов (назначьте LootTable)
- **Perk** - данные перков
- **WaveConfig** - конфигурация волн
- **LootTable** - таблица дропа

## 🎯 Важные настройки

### Tags & Layers
- Теги: `Player`, `Enemy`
- Слои: `Player` (Layer 8), `Enemy` (Layer 9)

### NavMesh
- Выделите пол арены
- **Window → AI → Navigation**
- Нажмите **Bake**

### Input System
- Для мобильного управления создайте UI Virtual Joystick
- Подключите к `PlayerController.SetMoveInput(Vector2)`
- Кнопку атаки подключите к `WeaponSystem.TryAttack()`

## 📝 Порядок подключения

1. **Core**: GameController, ObjectPool
2. **Player**: PlayerController, WeaponSystem
3. **AI**: EnemyBase (префабы), WaveSpawner, AIDirector
4. **Gameplay**: XPManager, LevelUpSystem, LootSystem
5. **UI**: HUDController, LevelUpMenuController
6. **Audio**: AudioManager
7. **ScriptableObjects**: создать и настроить все данные

## ⚠️ Важно

- **Не трогаю сцены** - только скрипты
- **Не создаю Canvas/UI элементы** - только логика
- **Не настраиваю URP/Quality** - только код
- Все визуальные компоненты и связи настраиваете в Unity Editor

## 🐛 Решение проблем

### Враги не спавнятся
→ Проверьте, что WaveConfig назначен в WaveSpawner
→ Проверьте, что префабы врагов добавлены в ObjectPool

### Перки не применяются
→ Проверьте, что все Perk ScriptableObjects назначены в LevelUpSystem
→ Проверьте, что WeaponSystem найден при применении перка

### Лут не дропается
→ Проверьте, что LootTable назначен в EnemyDef
→ Проверьте, что префабы лута добавлены в ObjectPool

---

**Все системы готовы к использованию!** Настройте визуальные компоненты и связи в Unity Editor.

