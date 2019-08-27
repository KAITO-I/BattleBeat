using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    enum ResultState
    {
        BackDis,
        TextMove,
        CharaDis,
        TextDis,
        SceneJump
    }
    struct CharaStatus
    {
        public string name;
        public Sprite backimg;
        public Color charaColor;
        public string Word;
    }
    //キャラクターの名前
    string[] Name =
    {
        "カグラ",
        "ホーミー",
        "アナ",
        "ユニ＆ゾーン"
    };
    //キャラクター台詞
    string[] Words ={
        "カグラ勝利,これはテストの表示です",
        "ホーミー勝利,これはテストの表示です",
        "アナ勝利,これはテストの表示です",
        "ユニ＆ゾーン勝利,これはテストの表示です "
    };
    //キャラクターの色
    Color[] characolors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    //ここのintをSettingのCharaに変えると全て変わる
    Dictionary<int, CharaStatus> chara = new Dictionary<int, CharaStatus>();
    ResultState state;
    Text PlayerT, Charaname, Word;
    [SerializeField]
    GameObject[] Texts;
    [SerializeField]
    Image BackGraund, CharaImg;
    Color changecolor;
    [SerializeField]
    GameObject BlackImg;

    //背景の画像
    [SerializeField]
    Sprite[] BackImgs;

    public float MoveTime;
    Vector3[] vec = new Vector3[3];
    RectTransform[] Gole = new RectTransform[3];

    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間

    private float timeUntilDisplay = 0;     // 表示にかかる時間
    private float timeElapsed = 1;          // 文字列の表示を開始した時間
    private int lastUpdateCharacter = -1;       // 表示中の文字数

    [SerializeField]
    GameObject WordPos;
    //勝利したほうの情報を受け取る
    [SerializeField]
    int WinPlayerID;
    [SerializeField]
    int WinCharaID;

    int flag;
    // Start is called before the first frame update
    void Start()
    {
        //キャラ情報初期化
        for(int i = 0; i < 4; ++i)
        {
            CharaStatus status = new CharaStatus();
            status.name = Name[i];
            status.charaColor = characolors[i];
            status.Word = Words[i];
            status.backimg = BackImgs[i];
            chara.Add(i, status);
        }
        //Text取得
        PlayerT = Texts[0].GetComponent<Text>();
        Charaname = Texts[1].GetComponent<Text>();
        Word = Texts[3].GetComponent<Text>();
        //位置取得
        for (int i = 0; i < 3; i++)
        {
            Gole[i] = Texts[i].GetComponent<RectTransform>();
            vec[i] = Texts[i].GetComponent<RectTransform>().position;
            Vector3 pos = Texts[i].transform.position;
            pos.x = -300;
            Texts[i].gameObject.transform.position = pos;
        }
        //色初期化
        changecolor =  new Color();
        changecolor = Color.white;
        changecolor.a = 0f;
        //背景
        BackGraund.sprite = chara[WinCharaID - 1].backimg;
        //キャラクター
        CharaImg.color = changecolor;

        PlayerT.text = WinPlayerID.ToString() + "P";

        Charaname.text = chara[WinCharaID - 1].name;
        Charaname.color = chara[WinCharaID - 1].charaColor;

        WordPos.SetActive(false);
        state = ResultState.BackDis;

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ResultState.BackDis:
                BlackImg.transform.position += new Vector3(150f, 0f, 0f);
                if (BlackImg.transform.position.x > 3000) state = ResultState.TextMove;
                break;
            case ResultState.TextMove:
                TextMove();
                break;
                //背景の透明度を変えている
            case ResultState.CharaDis:
                changecolor.a += 0.1f;
                CharaImg.color = changecolor;
                if (CharaImg.color.a >= 1)
                {
                    SetText();
                    WordPos.SetActive(true);
                    state = ResultState.TextDis;
                }
                break;
            case ResultState.TextDis:
                TextDis();
                break;
            case ResultState.SceneJump:
                OnClick();
                break;
        }

    }
    //シーン移動
    void OnClick()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject.Find("GameObject").GetComponent<SceneJump>().Jump("Test");
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject.Find("GameObject").GetComponent<SceneJump>().Jump("LoadScene");
        }
    }
    //キャラクターコメント表示
    void TextDis()
    {
        // クリックから経過した時間が想定表示時間の何%か確認し、表示文字数を出す
        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * chara[WinCharaID - 1].Word.Length);
        // 表示文字数が前回の表示文字数と異なるならテキストを更新する
        if (displayCharacterCount != lastUpdateCharacter)
        {
            Word.text = chara[WinCharaID-1].Word.Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
        //すべて表示したら飛べるようにする
        if (displayCharacterCount == chara[WinCharaID - 1].Word.Length)
        {
            state = ResultState.SceneJump;
        }

    }
    //テキスト表示初期化
    void SetText()
    {
        // 想定表示時間と現在の時刻をキャッシュ
        timeUntilDisplay = chara[WinCharaID - 1].Word.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        // 文字カウントを初期化
        lastUpdateCharacter = -1;
    }


    //
    void TextMove()
    {
        RectTransform rect = Texts[flag].GetComponent<RectTransform>();
        switch (flag)
        {
            case 0:
                Vector3 pos = new Vector3(MoveTime, 0, 0);
                rect.position += pos;
                if (vec[flag].x <= Gole[flag].position.x)
                {
                    flag++;
                }
                break;
            case 1:
                Vector3 pos2 = new Vector3(MoveTime, 0, 0);
                rect.position += pos2;
                if (vec[flag].x <= Gole[flag].position.x)
                {
                    flag++;
                }
                break;
            case 2:
                Vector3 pos3 = new Vector3(MoveTime, 0, 0);
                rect.position += pos3;
                if (vec[flag].x <= Gole[flag].position.x)
                {
                    state = ResultState.CharaDis;
                }
                break;
        }
    }
}
