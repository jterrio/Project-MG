using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purchase : MonoBehaviour
{
    public ShopStand ss;


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (GameManager.gm.p.money >= ss.moneyCost) {
                //PLAY SOUND
                ss.Purchase();
            } else {
                //PLAY SOUND
            }
        }
    }
}
