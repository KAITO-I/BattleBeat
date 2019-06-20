//==============================
// Created by KAITO-I (稲福)
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================
// 音量値格納
//==============================
public struct Volume
{

}

//==============================
// サウンド管理
//==============================
public class SoundManager : MonoBehaviour {
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("SoundManagerが読み込まれていません");
            return instance;
        }
    }

    //==============================
    // クラス
    //==============================
    // 音量
    [SerializeField] private float defVol; // デフォルト値
    private float master;
    public float Master
    {
        get { return this.master; }
        set
        {
            PlayerPrefs.SetFloat("MasterVol", this.master = value);
            PlayerPrefs.Save();
        }
    }

    private float bgm;
    public float BGM
    {
        get { return this.bgm; }
        set
        {
            PlayerPrefs.SetFloat("BGMVol", this.bgm = value);
            PlayerPrefs.Save();
        }
    }

    private float se;
    public float SE
    {
        get { return this.se; }
        set
        {
            PlayerPrefs.SetFloat("SEVol", this.se = value);
            PlayerPrefs.Save();
        }
    }

    private float voice;
    public float Voice
    {
        get { return this.voice; }
        set
        {
            PlayerPrefs.SetFloat("VoiceVol", this.voice = value);
            PlayerPrefs.Save();
        }
    }

    // 再生オブジェクト
    [SerializeField] AudioSource bgmAS;
    [SerializeField] GameObject  seObj;
    [SerializeField] AudioSource voiceAS;

    private void Awake()
    {
        SoundManager.instance = this;

        //===== 音量初期値 =====
        this.Master = PlayerPrefs.GetFloat("MasterVol", defVol);
        this.BGM    = PlayerPrefs.GetFloat("BGMVol", defVol);
        this.SE     = PlayerPrefs.GetFloat("SEVol", defVol);
        this.Voice  = PlayerPrefs.GetFloat("VoiceVol", defVol);
    }

    //------------------------------
    // BGM再生
    //------------------------------
    // [引数]
    // AudioClip bgm : 再生するBGM
    //------------------------------
    public void PlayBGM(AudioClip bgm)
    {
        if (this.bgmAS.clip == bgm) return;

        if (this.bgmAS.isPlaying) StopBGM();
        this.bgmAS.clip = bgm;
        this.bgmAS.Play();
    }

    //------------------------------
    // BGM停止
    //------------------------------
    public void StopBGM()
    {
        this.bgmAS.Stop();
    }

    //------------------------------
    // BGM音量設定
    //------------------------------
    public void SetBGMVolume(float volume)
    {
        this.bgmAS.volume = volume;
    }

    //------------------------------
    // SE再生
    //------------------------------
    // [引数]
    // AudioClip se : 再生するSE
    //------------------------------
    public void PlaySE(AudioClip se)
    {
        Instantiate(this.seObj).GetComponent<SEManager>().Play(se);
    }
}