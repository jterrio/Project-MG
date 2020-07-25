using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorClear : MonoBehaviour {

    //public string levelToLoad;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Player") {
            GameManager.gm.playerMinimapObject.SetActive(false);
            Destroy(RoomManager.rm.transform.parent.gameObject);
            RoomManager.rm = null;
            LevelManager.lm.LoadFarmLevel();
        }
    }
}
