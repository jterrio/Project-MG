using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour {

    private void OnCollisionEnter(Collision collision) {
        GameManager.gm.CycleWeapon();
    }

}
