//==============================
// Created by KAITO-I (稲福)
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================
// MasterVolumeをポインターのように扱う為のクラス
//==============================
public class MasterVolume
{
    public float Value { get; set; }

    public MasterVolume(float value)
    {
        this.Value = value;
    }
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
    [SerializeField] float defVol; // デフォルト音量値
    private MasterVolume masterVolume;
    public float MasterVolume
    {
        get { return this.masterVolume.Value; }
        set
        {
            this.masterVolume.Value = value;

            PlayerPrefs.SetFloat("MasterVol", value);
            PlayerPrefs.Save();

            this.bgm.UpdateVolume();
            this.se.UpdateVolume();
            this.voice.UpdateVolume();
        }
    }

    // 再生オブジェクト
    [SerializeField] Sound     bgm;
    public           Sound     BGM { get { return this.bgm; } }
    [SerializeField] SEManager se;
    public           SEManager SE { get { return this.se; } }
    [SerializeField] Sound     voice;
    public           Sound     Voice { get { return this.voice; } }

    private void Awake()
    {
        SoundManager.instance = this;

        //===== 音量初期値 =====
        this.masterVolume = new MasterVolume(PlayerPrefs.GetFloat("MasterVol", defVol));
        this.bgm.Init(masterVolume, "BGMVol", defVol);
        this.se.Init(masterVolume, "SEVol", defVol);
        this.voice.Init(masterVolume, "VoiceVol", defVol);
    }
}