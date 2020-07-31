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
            if (i.shopItemPool) {
                shopItems.Add(g);
            }
            if (i.bossItemPool) {
                bossItems.Add(g);
            }
        }
    }

    public GameObject getRandomCommonItem() {
        return commonItems[Random.Range(0, commonItems.Count)];
    }

    public GameObject getRandomUncommonItem() {
        return uncommonItems[Random.Range(0, uncommonItems.Count)];
    }

    public GameObject getRandomRareItem() {
        return rareItems[Random.Range(0, rareItems.Count)];
    }

    public GameObject getRandomEpicItem() {
        return epicItems[Random.Range(0, epicItems.Count)];
    }

    public GameObject getRandomLegItem() {
        return legendaryItems[Random.Range(0, legendaryItems.Count)];
    }

    public GameObject getRandomAnyItem() {
        return allItems[Random.Range(0, allItems.Count)];
    }

    public GameObject getRandomShopItem() {
        return shopItems[Random.Range(0, shopItems.Count)];
    }

    public GameObject getRandomBossItem() {
        return bossItems[Random.Range(0, bossItems.Count)];
    }

}
