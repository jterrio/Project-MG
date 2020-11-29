﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public static ItemManager im;
    
    [Header("Item Lists")]
    public List<GameObject> commonItems;
    public List<GameObject> uncommonItems;
    public List<GameObject> rareItems;
    public List<GameObject> epicItems;
    public List<GameObject> legendaryItems;

    private Hashtable allItems;
    private Hashtable shopItems;
    private Hashtable bossItems;

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
    public Hashtable obtainedItems;

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

        allItems = new Hashtable();
        foreach(GameObject g in commonItems) {
            allItems.Add(g, g.GetComponent<Item>().ID);
        }
        foreach (GameObject g in uncommonItems) {
            allItems.Add(g, g.GetComponent<Item>().ID);
        }
        foreach (GameObject g in rareItems) {
            allItems.Add(g, g.GetComponent<Item>().ID);
        }
        foreach (GameObject g in epicItems) {
            allItems.Add(g, g.GetComponent<Item>().ID);
        }
        foreach (GameObject g in legendaryItems) {
            allItems.Add(g, g.GetComponent<Item>().ID);
        }
        shopItems = new Hashtable();
        bossItems = new Hashtable();

        foreach(GameObject g in allItems.Keys) {
            Item i = g.GetComponent<Item>();
            float c = GetCostFromRarity(i.rarity);
            totalItemChanceCost += c;
            if (i.shopItemPool) {
                shopChanceCost += c;
                shopItems.Add(g, i.ID);
            }
            if (i.bossItemPool) {
                bossChanceCost += c;
                bossItems.Add(g, i.ID);
            }
        }

        obtainedItems = new Hashtable();
    }

    public GameObject GetRandomCommonItem() {
        return commonItems[Random.Range(0, commonItems.Count)];
    }

    public GameObject GetRandomUncommonItem() {
        return uncommonItems[Random.Range(0, uncommonItems.Count)];
    }

    public GameObject GetRandomRareItem() {
        return rareItems[Random.Range(0, rareItems.Count)];
    }

    public GameObject GetRandomEpicItem() {
        return epicItems[Random.Range(0, epicItems.Count)];
    }

    public GameObject GetRandomLegItem() {
        return legendaryItems[Random.Range(0, legendaryItems.Count)];
    }

    public GameObject GetRandomAnyItem() {
        float cost = Random.Range(0, allItems.Count);
        foreach (GameObject i in allItems.Keys) {
            cost--;
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomShopItem() {
        float cost = Random.Range(0, shopItems.Count);
        foreach (GameObject i in shopItems.Keys) {
            cost--;
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomBossItem() {
        float cost = Random.Range(0, bossItems.Count);
        foreach (GameObject i in bossItems.Keys) {
            cost--;
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomAnyItemWeighted() {
        float cost = Random.Range(0, totalItemChanceCost);
        foreach (GameObject i in allItems.Keys) {
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
        foreach (GameObject i in shopItems.Keys) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
            }
        }
        return null;
    }

    public GameObject GetRandomBossItemWeighted() {
        float cost = Random.Range(0, bossChanceCost);
        foreach (GameObject i in bossItems.Keys) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
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
            obtainedItems.Add(i, item.ID);
        }
        item.GetItemEffects();
    }

    public bool DoesPlayerHaveItem(Item i) {
        if (obtainedItems.ContainsValue(i.ID)){
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
