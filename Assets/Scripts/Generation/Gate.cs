using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    public bool hasCleared = false;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (!hasCleared) {
                RoomManager.rm.ChangeRoom(this.GetComponentInParent<Room>());
            }
        }
    }

}

