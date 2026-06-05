using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Achievement
{
    public LocalizedString unlockedTitleKey; // Key para el título desbloqueado
    public LocalizedString descriptionKey;   // Key para la descripción
    public Sprite icon;             // Icono del logro
    public bool isUnlocked = false; // Estado del logro
    public Rarity rarity;
    public enum Rarity
    {
        common, rare, veryRare, epic, legendary
    }
}