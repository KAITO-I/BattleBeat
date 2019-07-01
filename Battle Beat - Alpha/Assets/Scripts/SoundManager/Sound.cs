//==============================
// Created by KAITO-I (稲福)
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    protected AudioSource AudioSource;

    protected MasterVolume MasterVol;
    private   string       prefsName;
    private   float        volume;
    public    float        Volume
    {
        get { return this.volume; }
        set
        {
            this.volume = value;

            PlayerPrefs.SetFloat(prefsName, value);
            PlayerPrefs.Save();

            UpdateVolume();
        }
    }

    //------------------------------
    // 初期化
    //------------------------------
    public void Init(MasterVolume masterVol, string prefsName, float defVol)
    {
        this.AudioSource = GetComponent<AudioSource>();

        this.MasterVol = masterVol;
        this.prefsName = prefsName;
        this.volume    = PlayerPrefs.GetFloat(prefsName, defVol);

        UpdateVolume();
    }

    //------------------------------
    // 音量更新
    //------------------------------
    public void UpdateVolume()
    {
        this.AudioSource.volume = MasterVol.Value * volume;
    }

    //------------------------------
    // 再生
    //------------------------------
    // [引数]
    // AudioClip sound : 再生する音源
    //------------------------------
    public virtual void Play(AudioClip clip)
    {
        // 既に再生していれば実行しない
        if (this.AudioSource.clip == clip) return;

        if (this.AudioSource.isPlaying) Stop();
        this.AudioSource.clip = clip;
        this.AudioSource.Play();
    }

    //------------------------------
    // 停止
    //------------------------------
    public void Stop()
    {
        this.AudioSource.Stop();
    }
}