using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextPopUp))]
public class ShopStand : MonoBehaviour {

    public GameObject shopItem;
    public float moneyCost;
    public GameObject purchaseTrigger;
    public TMPro.TextMeshPro text;

    public void Purchase() {
        shopItem.GetComponent<Item>().GetItemEffects();
        GameManager.gm.p.LoseMoney(moneyCost);
        Destroy(purchaseTrigger);
        Destroy(shopItem);
        Destroy(text.gameObject);
    }

    public void SetShop(GameObject item, float baseCost) {
        shopItem = item;
        Item i = item.GetComponent<Item>();
        shopItem.transform.position = this.gameObject.transform.position + (Vector3.up * 3f);
        shopItem.transform.parent = this.gameObject.transform;

        switch (i.rarity) {
            case Item.Rarity.COMMON:
                moneyCost = 1f * baseCost;
                break;
            case Item.Rarity.UNCOMMON:
                moneyCost = 1.5f * baseCost;
                break;
            case Item.Rarity.RARE:
                moneyCost = 3.5f * baseCost;
                break;
            case Item.Rarity.EPIC:
                moneyCost = 6f * baseCost;
                break;
            case Item.Rarity.LEGENDARY:
                moneyCost = 11f * baseCost;
                break;
        }

        text.text = shopItem.gameObject.name + ": " + Mathf.RoundToInt(moneyCost).ToString();
    }

}
