using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    public bool hasCleared = false;

    private void OnTriggerEnter(Collider other) {
        if (!hasCleared) {
            RoomManager.rm.ChangeRoom(this.GetComponentInParent<Room>());
        }
    }

}

