using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopUp : MonoBehaviour {

    public TMPro.TextMeshPro text;
    public float distanceToPop;

    private void Update() {
        if(text == null) {
            return;
        }
        if(Vector3.Distance(GameManager.gm.player.transform.position, this.gameObject.transform.position) <= distanceToPop) {
            text.gameObject.SetActive(true);
            text.gameObject.transform.LookAt(2 * text.gameObject.transform.position - GameManager.gm.player.transform.position);
        } else {
            text.gameObject.SetActive(false);
        }
    }

}
