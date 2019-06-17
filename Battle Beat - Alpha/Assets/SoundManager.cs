using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volume
{
    public float Master { get; set; }
    public float BGM { get; set; }
    public float SE { get; set; }
    public float Voice { get; set; }
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
            if (instance == null)
            {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
                if (instance == null) Debug.Log("SoundManagerが読み込まれていません");
            }
            return instance;
        }
    }

    //==============================
    // クラス
    //==============================
    private AudioSource bgmAS;
    private GameObject  seObj;

    public Volume Volume { get; private set; }

    private void Awake() {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //===== BGM(GameObject) =====
        this.bgmAS = GetComponent<AudioSource>();

        //===== SE(GameObject) =====
        this.seObj = new GameObject("SE");
        this.seObj.AddComponent<SEManager>();
        AudioSource audioSource = this.seObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop        = false;
        Destroy(this.seObj);

        //===== Volume =====
        // Prefsより
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

//==============================
// SE専用クラス
//==============================
public class SEManager : MonoBehaviour
{
    //------------------------------
    // 再生
    //------------------------------
    // [引数]
    // AudioClip se : 再生するSE
    //------------------------------
    public void Play(AudioClip se)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = se;
        audioSource.Play();
        StartCoroutine(DestroyCoroutine(se.length));
    }

    //------------------------------
    // 削除コルーチン
    //------------------------------
    // [引数]
    // float timer : 再生時間
    //------------------------------
    public IEnumerator DestroyCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}