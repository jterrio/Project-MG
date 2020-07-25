using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerPosition : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameManager.gm.playerMinimapObject.transform.position = this.transform.position + new Vector3(0, 10f, 0);
            GameManager.gm.playerMinimapObject.SetActive(true);
        }
    }

}
