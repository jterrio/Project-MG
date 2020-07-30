using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopStand : MonoBehaviour {

    public GameObject shopItem;
    public float moneyCost;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if(GameManager.gm.p.money >= moneyCost) {
                //PLAY SOUND
                Purchase();
            } else {
                //PLAY SOUND
            }
        }
    }

    private void Update() {
        if(shopItem != null) {
            shopItem.transform.Rotate(Vector3.up * 20f * Time.deltaTime);
        }
    }


    public void Purchase() {
        shopItem.GetComponent<Item>().GetItemEffects();
        GameManager.gm.p.SetMoney(moneyCost);
    }

    public void SetShop(GameObject item, float money) {
        shopItem = item;
        shopItem.transform.position = this.gameObject.transform.position + (Vector3.up * 3f);
        shopItem.transform.parent = this.gameObject.transform;
        moneyCost = money;
    }

}
