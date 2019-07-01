using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleMG : MonoBehaviour
{
    //タイトルの状態
    enum TitleStatus
    {
        Anim,
        BackMove,
        Title
    }
    TitleStatus title_;

    //アニメーション
    VideoPlayer video;
    int d = 0;

    //背景
    public RectTransform Titleback;
    float BackW = Screen.width / 2;
    float BackH = Screen.height / 2;
    public float Backmove_speed;
    //Text
    public Text text;
    public float UIinterval;
    Color color_;
    bool B;
    //BGM
    AudioSource audio_;

    // Start is called before the first frame update
    void Start()
    {
        audio_ = GetComponent<AudioSource>();
        video = GetComponent<VideoPlayer>();
        video.Play();
        //透明
        color_.a = 0f;
        text.color = color_;
        title_ = TitleStatus.Anim;
    }

    // Update is called once per frame
    void Update()
    {
        switch (title_)
        {
            case TitleStatus.Anim:
                if(!video.isPlaying)title_ = TitleStatus.BackMove;
                break;
            case TitleStatus.BackMove:
                BackGraund();
                break;
            case TitleStatus.Title:
                JumpTitle();
                break;
        }
    }
    //背景を動かす処理
    void BackGraund()
    {
        Titleback.transform.position -= new Vector3(0f,Backmove_speed,0f);
        //行き過ぎた用
        if (Titleback.transform.position.y <= BackH)
        {
            Titleback.transform.position = new Vector3(BackW, BackH, 0f);
            title_ = TitleStatus.Title;
            //BGM再生
            audio_.Play();
        }
    }

    //タイトル次の画面に移動出来る状態
    void JumpTitle()
    {
        if (!B)  StartCoroutine(UIStart());

        //ボタンが入力されたら
        if (Input.GetKeyDown(KeyCode.Return)||
            Input.GetKeyDown(KeyCode.Backspace)||
            Input.GetKeyDown(KeyCode.Space)||
            Input.GetKeyDown(KeyCode.Tab)
            )
        {
            GameObject.Find("GameObject").GetComponent<SceneJump>().Jump("LoadScene");
        }
    }
    //UIのアニメーション処理
    IEnumerator UIStart()
    {
        float time = 0;
        B = true;
        //見えるように
        while (time <= UIinterval)
        {
            int a = 1;
            time += Time.deltaTime * a;
            color_.a = time / UIinterval;
            text.color = color_;
            yield return new WaitForSeconds(1f / 60f);
        }
        time = UIinterval;
        //見えなくなるように
        while (time >= 0)
        {
            int a = -1;
            time += Time.deltaTime * a;
            color_.a = time / UIinterval;
            text.color = color_;
            yield return new WaitForSeconds(1f / 60f);
        }
        B = false;
    }
}
