using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        transform.position = new Vector3(GameManager.gm.player.transform.position.x, 1.5f, GameManager.gm.player.transform.position.z);
        if(RoomManager.rm.currentRoom.monsters.Count == 0) {
            return;
        }
        if(RoomManager.rm.currentRoom.monsters.Count == 1) {
            transform.LookAt(RoomManager.rm.currentRoom.monsters[0].transform.position);
            return;
        }

        float small = Vector3.Distance(GameManager.gm.player.transform.position, RoomManager.rm.currentRoom.monsters[0].transform.position);
        GameObject chosen = RoomManager.rm.currentRoom.monsters[0];
        foreach (GameObject c in RoomManager.rm.currentRoom.monsters) {
            if(c == chosen) {
                continue;
            }
            float temp = Vector3.Distance(GameManager.gm.player.transform.position, c.transform.position);
            if(temp < small) {
                small = temp;
                chosen = c;
            }
        }
        Vector3 targetPos = new Vector3(chosen.transform.position.x, this.transform.position.y, chosen.transform.position.z);
        transform.LookAt(targetPos);
    }
}
