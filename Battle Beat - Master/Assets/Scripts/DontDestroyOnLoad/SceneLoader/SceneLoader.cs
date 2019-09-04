//==============================
// Created by SA371516
// Customized by KAITO-I
//==============================
using System.Collections;
using System.Text;
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
        Title           = 0,
        MainMenu        = 1,
        CharacterSelect = 2,
        MainGame        = 3,
        Training        = 5,
        Result          = 4,
        Config          = 6,
        Credit          = 7
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
    [SerializeField]
    private float shutterDownTime;
    [SerializeField]
    private float shutterUpTime;

    // ロード中
    public bool isLoading { private set; get; }

    [Header("Loading")]
    [SerializeField]
    private TextMeshProUGUI pressButtonText;
    [SerializeField]
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private Image loadingGauge;

    private string loadingTextMsg;

    [Header("LoadTitle")]
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Image fade;

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SceneLoader.instance != null) return;
        SceneLoader.instance = this;

        this.loadingObj.SetActive(false);

        this.isLoading      = false;
        this.loadingTextMsg = this.loadingText.text;

        this.fade.color = new Color(0f, 0f, 0f, 1f);
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

        this.loadingObj.SetActive(true);
        this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, 0f); // 念のためAlphaを0に

        float time;

        //===== タイトルシーン以外への遷移 =====
        if (target != Scenes.Title)
        {
            Transform loadingObjTF = this.loadingObj.transform;
            bool isTitle = SceneManager.GetActiveScene().buildIndex == (int)Scenes.Title;

            //===== ロード文字初期化 =====
            this.loadingText.text = this.loadingTextMsg;

            //===== シャッター降下 =====
            // 設定
            loadingObjTF.position = new Vector3(SceneLoader.canvasCenterX, SceneLoader.canvasCenterY + Screen.height);
            this.pressButtonText.color = new Color(this.pressButtonText.color.r, this.pressButtonText.color.g, this.pressButtonText.color.b, 0f);
            this.loadingText.color = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b, isTitle ? 0f : 1f);
            this.loadingGauge.fillAmount = 0f;

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

            //タイトルなら文字アニメーション再生
            if (isTitle)
            {
                this.pressButtonText.GetComponent<Animation>().Play();
                while (true)
                {
                    if (Input.anyKeyDown)
                    {
                        this.pressButtonText.GetComponent<Animation>().Stop();
                        this.pressButtonText.color = new Color(this.pressButtonText.color.r, this.pressButtonText.color.g, this.pressButtonText.color.b, 0f);
                        break;
                    }
                    yield return null;
                }
            }

            //===== 呼び出し =====
            //ローディング文字コルーチン再生
            Coroutine loadingTextAnim = StartCoroutine(Loading());

            AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
            async.allowSceneActivation = false;

            // 呼び出し待機
            while (async.progress < 0.9f)
            {
                this.loadingGauge.fillAmount = async.progress;
                yield return null;
            }

            // 修正
            this.loadingGauge.fillAmount = 1f;

            // ローディング文字コルーチン停止
            StopCoroutine(loadingTextAnim);

            // シーン移動
            async.allowSceneActivation = true;

            // 1フレーム待機
            yield return 0;

            //===== シャッター上昇 =====
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
        }
        //===== タイトルシーンへの遷移 =====
        else
        {
            //===== フェード =====
            time = 0f;
            while (time <= this.fadeTime)
            {
                this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, Mathf.Lerp(0f, 1f, time / this.fadeTime));

                time += Time.deltaTime;
                yield return 0;
            }

            // 修正
            this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, 1f);

            //===== 呼び出し =====
            AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
            async.allowSceneActivation = false;

            // 呼び出し待機
            while (async.progress < 0.9f) yield return null;

            // シーン移動
            async.allowSceneActivation = true;

            // 1フレーム待機
            yield return 0;

            this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, 0f);
        }

        this.loadingObj.SetActive(false);

        this.isLoading = false;
    }

    private IEnumerator Loading()
    {
        if (Mathf.Approximately(this.loadingText.color.a, 0f))
            this.loadingText.color = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b, 1f);

        int count = 0;
        while (true)
        {
            StringBuilder sb = new StringBuilder(this.loadingTextMsg);
            if (count != 0) sb.Append('.', count);
            this.loadingText.text = sb.ToString();

            count++;
            if (count > 3) count = 0;

            yield return new WaitForSeconds(1.0f);
        }
    }
}
