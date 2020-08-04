using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Rotate))]
public abstract class Item : MonoBehaviour {

    [Header("Item Pool Settings")]
    public bool normalItemPool;
    public bool shopItemPool;
    public bool bossItemPool;

    [Header("Item Settings")]
    public bool isUnique;
    public int ID;
    public string displayName;
    public Rarity rarity;

    public enum Rarity {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public abstract void GetItemEffects();

}
