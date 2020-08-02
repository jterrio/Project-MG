using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public static ItemManager im;
    
    public List<GameObject> commonItems;
    public List<GameObject> uncommonItems;
    public List<GameObject> rareItems;
    public List<GameObject> epicItems;
    public List<GameObject> legendaryItems;

    private List<GameObject> allItems;
    private List<GameObject> shopItems;
    private List<GameObject> bossItems;

    public float commonItemChance = 20f;
    public float uncommonItemChance = 15f;
    public float rareItemChance = 10f;
    public float epicItemChance = 8.5f;
    public float legendaryItemChance = 5f;

    private float totalItemChanceCost;
    private float shopChanceCost;
    private float bossChanceCost;

    private void Start() {

        if(im == null) {
            im = this;
        }else if(im != this) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        allItems = new List<GameObject>();
        allItems.AddRange(commonItems);
        allItems.AddRange(uncommonItems);
        allItems.AddRange(rareItems);
        allItems.AddRange(epicItems);
        allItems.AddRange(legendaryItems);
        shopItems = new List<GameObject>();
        bossItems = new List<GameObject>();

        foreach(GameObject g in allItems) {
            Item i = g.GetComponent<Item>();
            float c = GetCostFromRarity(i.rarity);
            totalItemChanceCost += c;
            if (i.shopItemPool) {
                shopChanceCost += c;
                shopItems.Add(g);
            }
            if (i.bossItemPool) {
                bossChanceCost += c;
                bossItems.Add(g);
            }
        }
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
        return allItems[Random.Range(0, allItems.Count)];
    }

    public GameObject GetRandomShopItem() {
        return shopItems[Random.Range(0, shopItems.Count)];
    }

    public GameObject GetRandomBossItem() {
        return bossItems[Random.Range(0, bossItems.Count)];
    }

    public GameObject GetRandomAnyItemWeighted() {
        float cost = Random.Range(0, totalItemChanceCost);
        foreach (GameObject i in allItems) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
            }
        }
        return allItems[0];
    }

    public GameObject GetRandomShopItemWeighted() {
        float cost = Random.Range(0, shopChanceCost);
        foreach (GameObject i in shopItems) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
            }
        }
        return allItems[0];
    }

    public GameObject GetRandomBossItemWeighted() {
        float cost = Random.Range(0, bossChanceCost);
        foreach (GameObject i in bossItems) {
            Item item = i.GetComponent<Item>();
            cost -= GetCostFromRarity(item.rarity);
            if (cost < 0) {
                return i;
            }
        }
        return allItems[0];
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

}
