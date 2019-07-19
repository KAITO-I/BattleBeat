//==============================
// Created by KAITO-I
//==============================
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
        }
    }

    // 再生オブジェクト
    [SerializeField]
    private Sound bgm;
    public  Sound BGM { get { return this.bgm; } }
    [SerializeField]
    private SE se;
    public  SE SE { get { return this.se; } }

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SoundManager.instance != null) return;

        SoundManager.instance = this;

        //===== 音量初期値 =====
        this.masterVolume = new MasterVolume(PlayerPrefs.GetFloat("MasterVol", defVol));
        this.bgm.Init(masterVolume, "BGMVol", defVol);
        this.se.Init(masterVolume, "SEVol", defVol);
    }
}