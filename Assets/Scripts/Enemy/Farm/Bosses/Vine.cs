using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    public GameObject vine;
    public AudioSource audioSource;
    public int vineDamage = 1;
    public float vineLift = 50f;
    public bool hasLift = true;


    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameManager.gm.p.TakeDamage(vineDamage);
            if (hasLift) {
                GameManager.gm.playerRB.AddForce(Vector3.up * vineLift, ForceMode.Impulse);
            }
        }
    }
}
