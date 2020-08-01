using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerPosition : MonoBehaviour {

    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity, GameManager.gm.minimap)) {
            GameManager.gm.playerMinimapObject.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 10f, 0);
            GameManager.gm.playerMinimapObject.SetActive(true);
        }
    }

}
