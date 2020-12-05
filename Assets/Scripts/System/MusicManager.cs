using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager mm;
    private Coroutine musicCoroutine;

    public float waitTime;

    public AudioSource audioSource;
    public Animator am;

    public AudioClip farmMusic;
    public AudioClip turnipLordMusic;
    public AudioClip deathMusic;


    private void Awake() {
        if(mm == null) {
            mm = this;
            DontDestroyOnLoad(mm);
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip ac) {
        audioSource.clip = ac;
        am.ResetTrigger("FadeOut");
        am.SetTrigger("FadeIn");
        audioSource.Play();
    }


    public void TransitionBGM(AudioClip ac) {
        am.SetTrigger("FadeOut");
        musicCoroutine = StartCoroutine(Transition(ac));

    }

    IEnumerator Transition(AudioClip ac) {
        yield return new WaitForSeconds(waitTime);
        PlayBGM(ac);
        musicCoroutine = null;
    }


}
