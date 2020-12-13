using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    [Header("Health")]
    public float healthTotal = 50f;
    public float health = 50f;

    [Header("Status Effects")]
    public Dictionary<HazardManager.StatusEffect, float> statusEffects = new Dictionary<HazardManager.StatusEffect, float>();

}
