using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour {



    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.layer == 12) {
            RoomManager.rm.currentRoom.DefeatMonster(collider.gameObject);
            return;
        }
        if(collider.gameObject.layer == 8) {
            if (!GameManager.gm.p.IsUplifting()) {
                GameManager.gm.playerRB.AddForce(Vector3.up * 80f * 3.5f, ForceMode.Impulse);
                GameManager.gm.p.TakeDamage(2);
                GameManager.gm.p.SetUplift(true);
            }

        }
    }

}
