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
    [SerializeField] GameObject loadingObj;

    // シャッターアニメーション
    [Header("Shutter Animation")]
    [SerializeField] float shutterDownTime;
    [SerializeField] float shutterUpTime;

    // ロード
    public bool isLoading { private set; get; }
    [Header("Shutter")]
    [SerializeField]
    private TextMeshProUGUI pressButtonText;
    [SerializeField]
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private Image loadingGauge;

    private string loadingTextMsg;

    [Header("Fade")]
    [SerializeField]
    private Image fade;
    [SerializeField]
    private float fadeTime;

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (SceneLoader.instance != null) return;
        SceneLoader.instance = this;

        Screen.SetResolution(1920, 1080, true);
        Cursor.visible = false;

        this.isLoading = false;
        this.pressButtonText.color = new Color(this.pressButtonText.color.r, this.pressButtonText.color.g, this.pressButtonText.color.b);
        this.loadingText.color     = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b);
        this.loadingTextMsg        = this.loadingText.text;
        this.loadingGauge.color    = new Color(this.loadingGauge.color.r, this.loadingGauge.color.g, this.loadingGauge.color.b);

        this.loadingObj.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f + Screen.height);
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
        bool title = (SceneManager.GetActiveScene().buildIndex == 0);
        float time;
        Transform loadingObjTF = this.loadingObj.transform;

        // 音声停止
        SoundManager.Instance.StopBGM();

        // ロード文字初期化
        this.loadingText.text = this.loadingTextMsg;
        this.loadingText.color = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b, 0f);

        //===== タイトル以外への遷移 =====
        if (target != Scenes.Title)
        {
            loadingObjTF.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f + Screen.height);
            this.pressButtonText.color = new Color(this.pressButtonText.color.r, this.pressButtonText.color.g, this.pressButtonText.color.b, 0f);
            this.loadingGauge.fillAmount = 0f;

            //===== シャッター降下 =====
            time = 0f;
            SoundManager.Instance.PlaySE(SEID.Shutter_Down);
            while (time <= this.shutterDownTime)
            {
                loadingObjTF.position =
                    new Vector3(
                        Screen.width / 2.0f,
                        Mathf.Lerp(Screen.height / 2.0f + Screen.height, Screen.height / 2.0f, time / this.shutterDownTime)
                    );
                time += Time.deltaTime;
                yield return null;
            }

            SoundManager.Instance.PlaySE(SEID.Shutter_Close);
            loadingObjTF.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f);

            // タイトルならLoading文字表示して待機
            if (title)
            {
                this.pressButtonText.GetComponent<Animation>().Play();
                while (true)
                {
                    if (Input.anyKeyDown)
                    {
                        SoundManager.Instance.PlaySE(SEID.General_Controller_Decision);
                        this.pressButtonText.GetComponent<Animation>().Stop();
                        this.pressButtonText.color = new Color(this.pressButtonText.color.r, this.pressButtonText.color.g, this.pressButtonText.color.b, 0f);
                        break;
                    }
                    yield return null;
                }
            }

            //===== 呼び出し =====
            // ローディング文字コルーチン再生
            // Coroutine loadingTextAnim = StartCoroutine(Loading());

            this.loadingText.color = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b, 1f);
            AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f)
            {
                this.loadingGauge.fillAmount = async.progress;
                yield return null;
            }
            this.loadingGauge.fillAmount = 1f;

            yield return new WaitForSeconds(1f);

            // ローディング文字コルーチン停止
            // StopCoroutine(loadingTextAnim);

            this.loadingText.color = new Color(this.loadingText.color.r, this.loadingText.color.g, this.loadingText.color.b, 0f);
            this.loadingGauge.fillAmount = 0f;
            async.allowSceneActivation = true;

            yield return 0;

            //===== シャッター上昇 =====
            time = 0f;
            SoundManager.Instance.PlaySE(SEID.Shutter_Up);
            while (time <= this.shutterUpTime)
            {
                loadingObjTF.position =
                    new Vector3(
                        Screen.width / 2.0f,
                        Mathf.Lerp(Screen.height / 2.0f, Screen.height / 2.0f + Screen.height, time / this.shutterUpTime)
                    );
                time += Time.deltaTime;
                yield return null;
            }

            // 修正
            loadingObjTF.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f + Screen.height);
        }
        //===== タイトルへの遷移 =====
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
            this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, 1f);

            //===== 呼び出し =====
            AsyncOperation async = SceneManager.LoadSceneAsync((int)target);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f) yield return null;
            async.allowSceneActivation = true;

            yield return 0;

            this.fade.color = new Color(this.fade.color.r, this.fade.color.g, this.fade.color.b, 0f);
        }
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
