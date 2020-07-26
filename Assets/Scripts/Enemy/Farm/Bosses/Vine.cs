using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    public GameObject vine;
    public AudioSource audioSource;
    public int vineDamage = 1;


    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameManager.gm.p.TakeDamage(vineDamage);
        }
    }
}
