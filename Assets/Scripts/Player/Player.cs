using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Health")]
    public float healthTotal = 8f;
    public float healthCurrent = 8f;

    [Header("References")]
    public TMPro.TextMeshProUGUI ammoCountText;
    public PlayerMovement pm;
    public Gun gun;

    void Start() {
        gun = GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update() {
        UpdateAmmoCount();
    }


    void UpdateAmmoCount() {
        if (ammoCountText != null) {
            if (gun.isReloading) {
                ammoCountText.text =   "--/" + gun.magSize.ToString();
            } else {
                ammoCountText.text = gun.bulletsInMag.ToString() + "/" + gun.magSize.ToString();
            }

        }
    }
}
