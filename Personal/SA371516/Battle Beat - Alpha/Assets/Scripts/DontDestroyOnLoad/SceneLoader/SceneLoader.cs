//==============================
// Created by SA371516
// Customized by KAITO-I
//==============================
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        Title           = 0,
        MainMenu        = 1,
        CharacterSelect = 2,
        MainGame        = 3,
        Training        = 4,
        Result          = 5,
        Config          = 6,
        Credit          = 7
    }

    // フェード用
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;

    // ロード画面
    [SerializeField] Image      background;
    [SerializeField] GameObject loadScreen;
    [SerializeField] Slider     slider;

    // ロード状態
    private bool isLoading;

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SceneLoader.instance != null) return;

        SceneLoader.instance = this;

        this.background.color = new Color(0f, 0f, 0f, 0f);
        this.loadScreen.SetActive(false);
        this.slider.value = 0f;

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
    }
}
