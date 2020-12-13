using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour {

    public static HazardManager hm;

    [HideInInspector]
    public bool hasFinishedLoading = false;

    [Header("Monsters")]
    public List<GameObject> monstersList;

    [Header("Status Effects")]
    private Dictionary<Actor, Coroutine> activeBurning;
    private Dictionary<Actor, Coroutine> activePoison;
    private Dictionary<Actor, Coroutine> activeCurse;
    private Dictionary<Actor, Coroutine> activeBlind;
    private Dictionary<Actor, Coroutine> activeSnared;
    private Dictionary<Actor, Coroutine> activeBleeding;

    [Header("Burn")]
    public float burnDamage;
    public float burnTime;
    public float burnStep;

    [Header("Poison")]
    public float poisonDamage;
    public float poisonTime;

    [Header("Curse")]
    public float curseDamage;
    public float curseTime;

    [Header("Blind")]
    public float blindTime;

    [Header("Snare")]
    public float snareTime;

    [Header("Bleed")]
    public float bleedDamage;
    public float bleedTime;

    public enum StatusEffect {
        NONE,
        BURNING,
        POISON,
        CURSE,
        BLIND,
        SNARED,
        BLEEDING
    }

    void Start(){
        //singeton
        if(hm == null) {
            hm = this;
        } else if(hm != this) {
            Destroy(this.gameObject);
        }
        ValidateItemIDs();
        hasFinishedLoading = true;

        activeBurning = new Dictionary<Actor, Coroutine>();
        activePoison = new Dictionary<Actor, Coroutine>();
        activeCurse = new Dictionary<Actor, Coroutine>();
        activeBlind = new Dictionary<Actor, Coroutine>();
        activeSnared = new Dictionary<Actor, Coroutine>();
        activeBleeding = new Dictionary<Actor, Coroutine>();
    }


    // Update is called once per frame
    void Update() {
        
    }


    private void ValidateItemIDs() {
        for (int i = 0; i < monstersList.Count; i++) {
            Enemy e = monstersList[i].GetComponent<Enemy>();
            if (e != null) {
                if (i != e.enemyID) {
                    print("ERROR ID WARNING: " + monstersList[i].name + " has an ID of " + e.enemyID + " ; It should have an ID of " + i);
                }
            }
        }
    }

    /// <summary>
    /// Gets a monster by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject GetMonsterByID(int id) {
        if(id > monstersList.Count) {
            return null;
        }
        return monstersList[id];
    }


    /// <summary>
    /// Attempts to return monster by name (needs to be exact)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetMonsterByName(string name) {
        foreach(GameObject monster in monstersList) {
            if(monster.name == name) {
                return monster;
            }
        }
        return null;
    }

    /*
     * ////////
     * STATUS METHODS
     * ////////
     * */

    public void ApplyStatusEffect(StatusEffect status, GameObject g) {
        Actor a = g.GetComponent<Actor>();
        bool alreadyHasStatus = false;
        if (a.statusEffects.ContainsKey(status)) {
            alreadyHasStatus = true;
        }
        switch (status) {
            case StatusEffect.BURNING:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += burnTime;
                } else {
                    a.statusEffects.Add(StatusEffect.BURNING, burnTime);
                    Coroutine c = StartCoroutine(Burn(a));
                    activeBurning.Add(a, c);
                }
                break;
            case StatusEffect.POISON:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += poisonTime;
                } else {
                    a.statusEffects.Add(StatusEffect.POISON, poisonTime);
                }
                break;
            case StatusEffect.CURSE:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += curseTime;
                } else {
                    a.statusEffects.Add(StatusEffect.CURSE, curseTime);
                }
                break;
            case StatusEffect.BLIND:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += blindTime;
                } else {
                    a.statusEffects.Add(StatusEffect.BURNING, blindTime);
                }
                break;
            case StatusEffect.SNARED:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += snareTime;
                } else {
                    a.statusEffects.Add(StatusEffect.BURNING, snareTime);
                }
                break;
            case StatusEffect.BLEEDING:
                if (alreadyHasStatus) {
                    a.statusEffects[status] += bleedTime;
                } else {
                    a.statusEffects.Add(StatusEffect.BURNING, bleedTime);
                }
                break;
            default:
                break;
        }
    }


    public void RemoveStatusEffect(Actor a, StatusEffect status) {
        if (!a.statusEffects.ContainsKey(status)) {
            return;
        }
        a.statusEffects.Remove(status);
        switch (status) {
            case StatusEffect.BURNING:
                StopCoroutine(activeBurning[a]);
                activeBurning.Remove(a);
                break;
            case StatusEffect.POISON:
                StopCoroutine(activePoison[a]);
                activePoison.Remove(a);
                break;
            case StatusEffect.CURSE:
                StopCoroutine(activeCurse[a]);
                activeCurse.Remove(a);
                break;
            case StatusEffect.BLIND:
                StopCoroutine(activeBlind[a]);
                activeBlind.Remove(a);
                break;
            case StatusEffect.SNARED:
                StopCoroutine(activeSnared[a]);
                activeSnared.Remove(a);
                break;
            case StatusEffect.BLEEDING:
                StopCoroutine(activeBleeding[a]);
                activeBleeding.Remove(a);
                break;
            default:
                break;
        }
    }

    public void CombineStatusEffects() {

    }

    IEnumerator Burn(Actor a) {
        while(a.statusEffects[StatusEffect.BURNING] > 0) {
            yield return new WaitForSeconds(burnStep);
            print("Burning with " + a.statusEffects[StatusEffect.BURNING] + " time left!");
            GameManager.gm.HaveActorTakeDamageFromStatus(a, burnDamage);
            a.statusEffects[StatusEffect.BURNING] -= burnStep;
        }
        a.statusEffects.Remove(StatusEffect.BURNING);
        activeBurning.Remove(a);
        yield return null;
    }

    IEnumerator Poison() {
        yield return null;
    }

    IEnumerator Curse() {
        yield return null;
    }

    IEnumerator Blind() {
        yield return null;
    }

    IEnumerator Snared() {
        yield return null;
    }

    IEnumerator Bleeding() {
        yield return null;
    }


}
