using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    protected AudioSource audio;
    protected float masterVol;
    protected float eachVol;

    private void Awake()
    {
        this.audio = GetComponent<AudioSource>();
    }

    public void Init()

    //------------------------------
    // 再生
    //------------------------------
    // [引数]
    // AudioClip sound : 再生する音
    //------------------------------
    public virtual void Play(AudioClip sound)
    {
        // 既に再生していれば実行しない
        if (this.audio.clip == sound) return;

        if (this.audio.isPlaying) Stop();
        this.audio.clip   = sound;
        this.audio.Play();
    }

    //------------------------------
    // 停止
    //------------------------------
    public void Stop()
    {
        this.audio.Stop();
    }

    //------------------------------
    // 音量設定
    //------------------------------
    // [引数]
    // float master : Master音量
    // float each   : 各音量
    //------------------------------
    public void SetVolume(float master, float each)
    {
        this.soundMgr = soundMgr;
    }
}
