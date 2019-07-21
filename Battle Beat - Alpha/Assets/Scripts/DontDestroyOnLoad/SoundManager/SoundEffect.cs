//==============================
// Created by KAITO-I (稲福)
//==============================
using System.Collections;
using UnityEngine;

//==============================
// SE専用クラス(Soundを継承)
//==============================
public class SoundEffect : Sound
{
    //------------------------------
    // 再生(override)
    //------------------------------
    public override void Play(AudioClip clip)
    {
        StartCoroutine(destroy(clip));
    }

    //------------------------------
    // 再生(Coroutine)
    //------------------------------
    private IEnumerator destroy(AudioClip clip)
    {
        this.AudioSource.clip = clip;
        this.AudioSource.Play();

        yield return new WaitForSeconds(clip.length);

        Destroy(this.gameObject);
    }
}
