using UnityEngine;
using System.Collections.Generic;

namespace MegabonkMobile.Data
{
    /// <summary>
    /// ScriptableObject с данными перка
    /// </summary>
    [CreateAssetMenu(fileName = "New Perk", menuName = "MegabonkMobile/Perk")]
    public class Perk : ScriptableObject
    {
        [Header("Perk Info")]
        public string id;
        public string displayName;
        [TextArea(3, 5)]
        public string description;
        public Sprite icon;
        public Rarity rarity = Rarity.Common;

        [Header("Effects")]
        public List<StatModifier> effects = new List<StatModifier>();

        [Header("Synergy")]
        [Tooltip("ID перков, с которыми есть синергия (увеличивает вес при выборе)")]
        public List<string> synergyPerkIds = new List<string>();
    }

    public enum Rarity
    {
        Common,    // Обычный
        Rare,      // Редкий
        Epic,      // Эпический
        Legendary  // Легендарный
    }
}

