//==============================
// Created by KAITO-I (稲福)
//==============================
using UnityEngine;

//==============================
// BGM、SEへの継承専用クラス
//==============================
public abstract class Sound : MonoBehaviour
{
    protected AudioSource AudioSource { get; set; }

    protected MasterVolume MasterVol { get; private set; }
    private string prefsName;
    private float volume;
    public float Volume
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
        this.volume = PlayerPrefs.GetFloat(prefsName, defVol);

        UpdateVolume();
    }

    public void UpdateVolume()
    {
        this.AudioSource.volume = MasterVol.Value * volume;
    }

    //==============================
    // 継承専用
    //==============================
    //------------------------------
    // 再生
    //------------------------------
    // [引数]
    // AudioClip clip : 再生する音源
    //------------------------------
    // [返り値]
    // Sound : 自身のクラス（音楽再生管理）
    //------------------------------
    public abstract Sound Play(AudioClip clip);

    //------------------------------
    // Resourcesから再生（ファイルパス：Sounds)
    //------------------------------
    // [引数]
    // string name : 再生する音源の名前
    //------------------------------
    // [返り値]
    // Sound : 自身のクラス（音楽再生管理）
    //------------------------------
    public abstract Sound PlayFromResources(string name);

    //------------------------------
    // 停止
    //------------------------------
    public abstract void  Stop();
}