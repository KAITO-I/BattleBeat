//==============================
// Created by SA371516
// Customized by KAITO-I
//==============================
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//==============================
// シーン読み込み
//==============================
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance
    {
        get
        {
            if (SceneLoader.instance == null) Debug.LogError("SceneLoaderが読み込まれていません");
            return SceneLoader.instance;
        }
    }

    //==============================
    // シーン一覧
    //==============================
    public enum Scenes
    {
        Title = 0,
        MainMenu = 1,
        CharacterSelect = 2,
        MainGame = 3,
        Training = 4,
        Result = 5,
        Config = 6,
        Credit = 7
    }

    //==============================
    // class
    //==============================
    // フェード用
    /*
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;

    // ロード画面
    [SerializeField] Image      background;
    [SerializeField] GameObject loadScreen;
    [SerializeField] Slider     slider;
    */
    private const float canvasCenterX = 960f;
    private const float canvasCenterY = 540f;

    [SerializeField] GameObject loadingObj;

    // シャッターアニメーション
    [Header("Shutter Animation")]
    [SerializeField] float shutterDownTime;
    [SerializeField] float shutterUpTime;

    // ロード中
    private bool isLoading;
    [Header("Loading")]
    [SerializeField] Image loadingGauge;
    [SerializeField] Text  loadingText;
    [SerializeField] Color loadingTextColor;

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SceneLoader.instance != null) return;

        SceneLoader.instance = this;

        this.isLoading = false;
    }

    //------------------------------
    // シーン読込
    //------------------------------
    public void LoadScene(Scenes target)
    {
        if (!this.isLoading)
            StartCoroutine(Load(target));
        else
            Debug.LogWarning("すでにロード処理が開始しているため、新しく開始できません");
    }

    private IEnumerator Load(Scenes target)
    {
        this.isLoading = true;
        float time;
        Transform loadingObjTF = this.loadingObj.transform;

        //===== シャッター降下 =====
        // 位置設定
        loadingObjTF.position = new Vector3(SceneLoader.canvasCenterX, SceneLoader.canvasCenterY + Screen.height);
        this.loadingGauge.fillAmount = 0f;
        this.loadingText.color = new Color(loadingTextColor.r, loadingTextColor.g, loadingTextColor.b, 0f);

        // 降下
        time = 0f;
        while (time <= this.shutterDownTime)
        {
            loadingObjTF.position = 
                new Vector3(
                    SceneLoader.canvasCenterX,
                    Mathf.Lerp(SceneLoader.canvasCenterY + Screen.height, SceneLoader.canvasCenterY, time / this.shutterDownTime)
                );
            time += Time.deltaTime;
            yield return null;
        }

        // 修正
        loadingObjTF.position = new Vector3(SceneLoader.canvasCenterX, SceneLoader.canvasCenterY);

        //===== タイトルなら文字アニメーション再生 =====
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            this.loadingText.GetComponent<Animation>().Play();
            while (true)
            {
                if (Input.anyKeyDown)
                {
                    this.loadingText.GetComponent<Animation>().Stop();
                    this.loadingText.color = new Color(loadingTextColor.r, loadingTextColor.g, loadingTextColor.b, 0f);
                    break;
                }
                yield return null;
            }
        }

        //===== 呼び出し =====
        AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            this.loadingGauge.fillAmount = async.progress;
            yield return null;
        }

        // 修正
        this.loadingGauge.fillAmount = 1f;

        //===== シャッター上昇 =====
        async.allowSceneActivation = true;
        time = 0f;
        while (time <= this.shutterUpTime)
        {
            loadingObjTF.position =
                new Vector3(
                    SceneLoader.canvasCenterX,
                    Mathf.Lerp(SceneLoader.canvasCenterY, SceneLoader.canvasCenterY + Screen.height, time / this.shutterUpTime)
                );
            time += Time.deltaTime;
            yield return null;
        }

        // 修正
        loadingObjTF.position = new Vector3(SceneLoader.canvasCenterX, SceneLoader.canvasCenterY + Screen.height);

        this.isLoading = false;

        /*
        this.slider.value = 0f;
        this.isLoading = true;

        // 暗転
        float time = 0f;
        this.background.color = new Color(0f, 0f, 0f, 0f);
        while (time <= this.fadeInTime)
        {
            this.background.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, time / this.fadeInTime));
            time += Time.deltaTime;
            yield return null;
        }

        // 呼び出し
        this.loadScreen.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            this.slider.value = async.progress;
            yield return null;
        }

        // 呼び出し完了
        this.loadScreen.SetActive(false);
        async.allowSceneActivation = true;

        // 明転
        time = 0f;
        this.background.color = new Color(0f, 0f, 0f, 1f);
        while (time <= this.fadeOutTime)
        {
            this.background.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, time / this.fadeOutTime));
            time += Time.deltaTime;
            yield return null;
        }

        this.isLoading = false;
        */
    }
}
