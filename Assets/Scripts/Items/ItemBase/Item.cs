using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Rotate))]
public abstract class Item : MonoBehaviour {

    public bool normalItemPool;
    public bool shopItemPool;
    public bool bossItemPool;

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
