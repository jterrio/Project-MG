using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunEffect : MonoBehaviour {

    [Range(0,100)]
    public float speed;
    private float oldSpeed;
    public ParticleSystem fire;
    public Light fireLight;
    public Color color;

    public bool tryingToStop = false;
    public bool canMove = true;

    // Update is called once per frame
    void Update() {
        if (!canMove) {
            return;
        }
        transform.Rotate(speed * Time.deltaTime, 0, 0);
        if(transform.rotation.eulerAngles.x % 360 > 200 && transform.rotation.eulerAngles.x % 360 < 359) {
            //RenderSettings.ambientLight = new Color(Mathf.Clamp((RenderSettings.ambientLight.r + (0.03f * Time.deltaTime)), 0.152f, 0.502f), Mathf.Clamp((RenderSettings.ambientLight.g + (0.034f * Time.deltaTime)), 0.18f, 0.56f), Mathf.Clamp((RenderSettings.ambientLight.b + (0.036f * Time.deltaTime)), 0.208f, 0.586f));
            if (!fire.isPlaying) {
                fire.Play();
                fireLight.enabled = true;
            }
        } else {
            //RenderSettings.ambientLight = new Color(Mathf.Clamp((RenderSettings.ambientLight.r - (.02f * Time.deltaTime)), 0.152f, 0.502f), Mathf.Clamp((RenderSettings.ambientLight.g - (0.027f * Time.deltaTime)), 0.18f, 0.56f), Mathf.Clamp((RenderSettings.ambientLight.b - (0.029f * Time.deltaTime)), 0.208f, 0.586f));
            fire.Stop();
            fireLight.enabled = false;
            if (tryingToStop) {
                if (transform.rotation.eulerAngles.x % 360 > 90) {
                    canMove = false;
                }
            }
        }
    }

    public void SetDay() {
        tryingToStop = true;
        oldSpeed = speed;
        speed = 100;
    }

    public void SetCycle() {
        tryingToStop = false;
        canMove = false;
        speed = oldSpeed;
    }
}
