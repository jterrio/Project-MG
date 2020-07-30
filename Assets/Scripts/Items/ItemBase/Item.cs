using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item : MonoBehaviour {

    public bool normalItemPool;
    public bool shopItemPool;
    public bool bossItemPool;

    public enum Rarity {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public abstract void GetItemEffects();

}
