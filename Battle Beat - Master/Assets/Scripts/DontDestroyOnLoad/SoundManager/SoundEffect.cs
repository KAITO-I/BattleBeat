//==============================
// Created by KAITO-I (稲福)
//==============================
using System.Collections;
using UnityEngine;

//==============================
// SEクラス
//==============================
public class SoundEffect : Sound
{
    private bool destroyed;

    //------------------------------
    // 再生
    //------------------------------
    public override Sound Play(AudioClip clip)
    {
        this.destroyed = false;

        SoundEffect se = Instantiate(this).GetComponent<SoundEffect>();
        se.AudioSource = se.gameObject.GetComponent<AudioSource>();
        
        StartCoroutine(se.DestroyTimer(clip));
        return se;
    }

    public override Sound PlayFromResources(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sound/SE/" + name);
        return Play(clip);
    }

    //------------------------------
    // SE再生後に自身を消去
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

    //------------------------------
    // 停止
    //------------------------------
    public override void Stop()
    {
        if (!this.destroyed)
        {
            this.AudioSource.Stop();

            this.destroyed = true;

            Destroy(this.gameObject);
        }
    }
}
