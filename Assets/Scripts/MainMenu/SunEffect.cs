using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunEffect : MonoBehaviour {

    [Range(0,100)]
    public float speed;
    public ParticleSystem fire;
    public Light fireLight;

    // Update is called once per frame
    void Update(){
        transform.Rotate(speed * Time.deltaTime, 0, 0);
        if(transform.rotation.eulerAngles.x % 360 > 200 && transform.rotation.eulerAngles.x % 360 < 340) {
            if (!fire.isPlaying) {
                fire.Play();
                fireLight.enabled = true;
            }
        } else {
            fire.Stop();
            fireLight.enabled = false;
        }
    }
}
