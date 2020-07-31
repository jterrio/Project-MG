using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GetComponent<Item>().GetItemEffects();
            Destroy(this.gameObject);
        }
    }

}
