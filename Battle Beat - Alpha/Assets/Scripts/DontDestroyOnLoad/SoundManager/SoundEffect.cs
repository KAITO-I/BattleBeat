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
    private bool destroyed;

    //------------------------------
    // 再生(override)
    //------------------------------
    public override Sound Play(AudioClip clip)
    {
        SoundEffect se = Instantiate(this).GetComponent<SoundEffect>();
        se.AudioSource = se.gameObject.GetComponent<AudioSource>();

        this.destroyed = false;
        
        StartCoroutine(se.DestroyTimer(clip));
        return se;
    }

    //------------------------------
    // 再生(Coroutine)
    //------------------------------
    private IEnumerator DestroyTimer(AudioClip clip)
    {
        this.AudioSource.clip = clip;
        this.AudioSource.Play();

        yield return new WaitForSeconds(clip.length);

        if (!this.destroyed)
        {
            this.destroyed = true;
            Destroy(this.gameObject);
        }
    }

    public override void Stop()
    {
        if (!this.destroyed)
        {
            base.Stop();
            this.destroyed = true;
            Destroy(this.gameObject);
        }
    }
}
