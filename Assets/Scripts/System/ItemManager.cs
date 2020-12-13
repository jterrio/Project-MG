using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public static ItemManager im;

    [HideInInspector]
    public bool hasFinishedLoading = false;

    [Header("Item Lists")]
    public List<GameObject> allItemReferences;

    [Header("Rarity Pools")]
    public List<int> commonItems;
    public List<int> uncommonItems;
    public List<int> rareItems;
    public List<int> epicItems;
    public List<int> legendaryItems;

    [Header("Special Pools")]
    public List<int> shopItems;
    public List<int> bossItems;

    [Header("Item Chances")]
    public float commonItemChance = 20f;
    public float uncommonItemChance = 15f;
    public float rareItemChance = 10f;
    public float epicItemChance = 8.5f;
    public float legendaryItemChance = 5f;

    private float totalItemChanceCost;
    private float shopChanceCost;
    private float bossChanceCost;

    [Header("Obtained Unique Items")]
    public HashSet<int> obtainedItems;

    public delegate void GunDelegate();
    public delegate void BeforeReloadDelegate();
    public delegate void AfterReloadDelegate();
    public delegate void BulletVelocityDelegate(GameObject bullet);
    public delegate void BulletHitDelegate(GameObject bullet, Bullet thisBullet, GameObject monsterHit);
    public GunDelegate gunDelegate;
    public BulletVelocityDelegate bulletVelocityDelegate;
    public BeforeReloadDelegate beforeReloadDelegate;
    public AfterReloadDelegate afterReloadDelegate;
    public BulletHitDelegate bulletHitDelegate;


    private void Start() {

        if(im == null) {
            im = this;
        }else if(im != this) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        ValidateItemIDs();

        commonItems = new List<int>();
        uncommonItems = new List<int>();
        rareItems = new List<int>();
        epicItems = new List<int>();
        legendaryItems = new List<int>();
        bossItems = new List<int>();
        shopItems = new List<int>();
        foreach(GameObject g in allItemReferences) {
            Item i = g.GetComponent<Item>();
            AddItemByRarity(i);
            AddItemSpecialPools(i);
        }
        obtainedItems = new HashSet<int>();
        hasFinishedLoading = true;
    }

    private void ValidateItemIDs() {
       for(int i = 0; i < allItemReferences.Count; i++) {
            if (i != allItemReferences[i].GetComponent<Item>().ID) {
                print("ERROR ID WARNING: " + allItemReferences[i].name + " has an ID of " + allItemReferences[i].GetComponent<Item>().ID + " ; It should have an ID of " + i);
            }
        }
    }

    void AddItemByRarity(Item i) {
        totalItemChanceCost += GetCostFromRarity(i.rarity);
        switch (i.rarity) {
            case Item.Rarity.COMMON:
                commonItems.Add(i.ID);
                break;
            case Item.Rarity.UNCOMMON:
                uncommonItems.Add(i.ID);
                break;
            case Item.Rarity.RARE:
                rareItems.Add(i.ID);
                break;
            case Item.Rarity.EPIC:
                epicItems.Add(i.ID);
                break;
            case Item.Rarity.LEGENDARY:
                legendaryItems.Add(i.ID);
                break;
        }
    }

    void AddItemSpecialPools(Item i) {
        float c = GetCostFromRarity(i.rarity);
        if (i.bossItemPool) {
            bossChanceCost += c;
            bossItems.Add(i.ID);
        }
        if (i.shopItemPool) {
            shopChanceCost += c;
            shopItems.Add(i.ID);
        }
    }

    public GameObject GetRandomCommonItem() {
        return allItemReferences[commonItems[Random.Range(0, commonItems.Count)]];
    }

    public GameObject GetRandomUncommonItem() {
        return allItemReferences[uncommonItems[Random.Range(0, uncommonItems.Count)]];
    }

    public GameObject GetRandomRareItem() {
        return allItemReferences[rareItems[Random.Range(0, rareItems.Count)]];
    }

    public GameObject GetRandomEpicItem() {
        return allItemReferences[epicItems[Random.Range(0, epicItems.Count)]];
    }

    public GameObject GetRandomLegItem() {
        return allItemReferences[legendaryItems[Random.Range(0, legendaryItems.Count)]];
    }

    public GameObject GetRandomAnyItem() {
        float cost = Random.Range(0, allItemReferences.Count);
        foreach (GameObject i in allItemReferences) {
            cost--;
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomShopItem() {
        float cost = Random.Range(0, shopItems.Count);
        foreach (int i in shopItems) {
            cost--;
            if (cost < 0) {
                return allItemReferences[i];
            }
        }
        return null;
    }

    public GameObject GetRandomBossItem() {
        float cost = Random.Range(0, bossItems.Count);
        foreach (int i in bossItems) {
            cost--;
            if (cost < 0) {
                return allItemReferences[i];
            }
        }
        return null;
    }

    public GameObject GetRandomAnyItemWeighted() {
        float cost = Random.Range(0, totalItemChanceCost);
        foreach (GameObject i in allItemReferences) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomShopItemWeighted() {
        float cost = Random.Range(0, shopChanceCost);
        foreach (int i in shopItems) {
            Item item = allItemReferences[i].GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return allItemReferences[i];
            }
        }
        return null;
    }

    public GameObject GetRandomBossItemWeighted() {
        float cost = Random.Range(0, bossChanceCost);
        foreach (int i in bossItems) {
            Item item = allItemReferences[i].GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return allItemReferences[i];
            }
        }
        return null;
    }

    public float GetCostFromRarity(Item.Rarity r) {
        switch (r) {
            case Item.Rarity.COMMON:
                return commonItemChance;
            case Item.Rarity.UNCOMMON:
                return uncommonItemChance;
            case Item.Rarity.RARE:
                return rareItemChance;
            case Item.Rarity.EPIC:
                return epicItemChance;
            case Item.Rarity.LEGENDARY:
                return legendaryItemChance;
            default:
                return commonItemChance;
        }
    }

    public void GetItem(GameObject i) {
        Item item = i.GetComponent<Item>();
        if (item.isUnique) {
            if (DoesPlayerHaveItem(item)) {
                return;
            }
            obtainedItems.Add(item.ID);
        }
        item.GetItemEffects();
    }

    public bool DoesPlayerHaveItem(Item i) {
        if (obtainedItems.Contains(i.ID)){
            return true;
        }
        return false;
    }

    public void ResetPlayerItems() {
        obtainedItems.Clear();
        gunDelegate = null;
        bulletVelocityDelegate = null;
        beforeReloadDelegate = null;
        afterReloadDelegate = null;
        bulletHitDelegate = null;
        GameManager.gm.p.wepFireIncrease = 0;
        GameManager.gm.p.wepDMGIncrease = 0;
        GameManager.gm.p.wepAmmoIncrease = 0;
        GameManager.gm.p.wepFireMulti = 1;
        GameManager.gm.p.wepDMGMulti = 1;
        GameManager.gm.p.wepAmmoMulti = 1;
        GameManager.gm.p.numberOfBulletBounces = 0;
        GameManager.gm.p.piercingShots = false;
}

    /*
     * \\\\\\\
     * 
     * ITEM METHODS
     * 
     * \\\\\\\
     **/


    public void CurveBullet(GameObject bullet) {
        Collider[] hitColliders = Physics.OverlapSphere(bullet.transform.position, 30f, GameManager.gm.enemyLayers);
        if(hitColliders.Length == 0) {
            return;
        }
        List<GameObject> mobsToCheck = new List<GameObject>();
        foreach(Collider col in hitColliders) {
            if(GameManager.gm.HasLineOfSight(bullet, col.gameObject)) {
                mobsToCheck.Add(col.gameObject);
            }
        }
        if(mobsToCheck.Count == 0) {
            return;
        }
        GameObject c = mobsToCheck[0];
        float distance = Vector3.Distance(bullet.transform.position, c.transform.position);
        foreach(GameObject g in mobsToCheck) {
            if(c == g) {
                continue;
            }
            float checkDistance = Vector3.Distance(g.transform.position, bullet.transform.position);
            if (checkDistance < distance) {
                distance = checkDistance;
                c = g;
            }
        }

        bullet.GetComponent<Rigidbody>().AddForce((c.gameObject.transform.position - bullet.transform.position).normalized * GameManager.gm.p.gun.bulletSpeed * 2.5f, ForceMode.VelocityChange);
    }

    public void NoReload() {
        GameManager.gm.p.gun.bulletsInMag = Mathf.FloorToInt((GameManager.gm.p.gun.magSize + GameManager.gm.p.wepAmmoIncrease) * GameManager.gm.p.wepAmmoMulti);
    }

    public void SlowOnReloadStart() {
        Time.timeScale = 0.35f;
    }

    public void SlowOnReloadFinish() {
        Time.timeScale = 1f;
    }

    public void BounceOffEnemy(GameObject originalBullet, Bullet thisBullet, GameObject monsterHit) {
        if(thisBullet.numberOfBounces >= GameManager.gm.p.numberOfBulletBounces) {
            return;
        }

        List<GameObject> visibleMonster = new List<GameObject>();
        foreach(Collider m in Physics.OverlapSphere(originalBullet.transform.position, 30f, GameManager.gm.enemyLayers)) {
            if(monsterHit == m.gameObject) {
                continue;
            }
            if (GameManager.gm.HasLineOfSight(originalBullet, m.gameObject)) {
                visibleMonster.Add(m.gameObject);
            }
        }
        if(visibleMonster.Count == 0) {
            return;
        }

        int bulletInt = 0;
        float bulletDistance = Vector3.Distance(visibleMonster[0].transform.position, originalBullet.transform.position);
        for(int i = 1; i < visibleMonster.Count; i++) {
            float d = Vector3.Distance(visibleMonster[i].transform.position, originalBullet.transform.position);
            if(d < bulletDistance) {
                bulletDistance = d;
                bulletInt = i;
            }
        }

        thisBullet.monsterToIgnore = visibleMonster[bulletInt];
        GameObject b = Instantiate(GameManager.gm.p.gun.bullet);
        b.transform.position = originalBullet.transform.position + ((visibleMonster[bulletInt].transform.position - originalBullet.transform.position) * 0.5f);
        b.transform.LookAt(visibleMonster[bulletInt].transform);
        b.GetComponent<Bullet>().numberOfBounces = originalBullet.GetComponent<Bullet>().numberOfBounces + 1;
        b.GetComponent<Rigidbody>().AddForce(b.transform.forward * GameManager.gm.p.gun.bulletSpeed);
    }

}
