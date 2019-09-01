//==============================
// Created by KAITO-I
//==============================
using UnityEngine;

//==============================
// MasterVolumeを扱うクラス
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
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float defaultVol; // デフォルト音量値

    private MasterVolume masterVolume;
    public float MasterVolume
    {
        get { return this.masterVolume.Value; }
        set
        {
            this.masterVolume.Value = value;

            PlayerPrefs.SetFloat("MasterVol", value);
            PlayerPrefs.Save();

            this.BGM.UpdateVolume();
            this.se.UpdateVolume();
        }
    }

    public Sound BGM { get; private set; }

    [SerializeField]
    private SoundEffect se;
    public  SoundEffect SE { get { return this.se; } }

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SoundManager.instance != null) return;

        SoundManager.instance = this;
        this.BGM = GetComponent<Sound>();

        //===== 音量初期値 =====
        this.masterVolume = new MasterVolume(PlayerPrefs.GetFloat("MasterVol", defaultVol));
        this.BGM.Init(masterVolume, "BGMVol", defaultVol);
        this.se.Init(masterVolume, "SEVol", defaultVol);
    }
}