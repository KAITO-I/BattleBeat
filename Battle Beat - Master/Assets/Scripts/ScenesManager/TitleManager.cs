using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleManager : MonoBehaviour
{
    private VideoPlayer vp;

    [SerializeField]
    private Image fade;
    [SerializeField]
    private float darkToLightFadeTime;
    [SerializeField]
    private float lightToDarkFadeTime;

    private void Start()
    {
        // アニメーション再生
        (this.vp = GetComponent<VideoPlayer>()).Play();

        // シャッタータイマー開始
        StartCoroutine(ShutterTimer(this.vp.clip.length));

        // 明転フェード開始
        StartCoroutine(StartFade());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            this.vp.Pause();
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect);
        }
    }

    private IEnumerator StartFade()
    {
        float timer = 0f;
        while (true)
        {
            this.fade.color = new Color(0f, 0f, 0f, Mathf.Lerp(1.0f, 0.0f, timer / this.darkToLightFadeTime));

            if (Mathf.Approximately(this.fade.color.a, 0f)) break;
            
            yield return 0;
            timer += Time.deltaTime;
        }
    }

    private IEnumerator ShutterTimer(double time)
    {
        yield return new WaitForSeconds((float) time);
        this.vp.Pause();
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
    }
}
