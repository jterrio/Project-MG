using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunEffect : MonoBehaviour {

    public ParticleSystem fire;
    public Light fireLight;
    public Color color;

    public bool tryingToStop = false;
    public bool canMove = true;


    public Light sun;
    public float secondsInFullDay = 10f;
    public float oldTime;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;

    float sunInitialIntensity;

    void Start() {
        sunInitialIntensity = sun.intensity;
    }

    void Update() {
        if (!canMove) {
            return;
        }
        UpdateSun();

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;
        //night
        if(currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f) {
            if (!fire.isPlaying) {
                fire.Play();
                fireLight.enabled = true;
            }

        } else { //day 
            if (fire.isPlaying) {
                fire.Stop();
                fireLight.enabled = false;
            }
            if (tryingToStop) {
                if (currentTimeOfDay > 0.35f && currentTimeOfDay < 0.45f) {
                    canMove = false;
                }
            }
        }

        if (currentTimeOfDay >= 1) {
            currentTimeOfDay = 0;
        }
    }

    public void SetDay() {
        tryingToStop = true;
        oldTime = secondsInFullDay;
        secondsInFullDay = 2f;
    }

    public void SetCycle() {
        tryingToStop = false;
        canMove = true;
        secondsInFullDay = oldTime;
    }

    void UpdateSun() {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f) {
            intensityMultiplier = 0;
        } else if (currentTimeOfDay <= 0.25f) {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        } else if (currentTimeOfDay >= 0.73f) {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}
