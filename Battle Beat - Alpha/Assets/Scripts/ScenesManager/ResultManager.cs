using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;


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
        public Sprite _backImg;
        public Sprite _charaImg;
        public Sprite _CharaText;
        public string _disWord;
    }
    //キャラクター台詞
    string[,] _words =
    {  
        {
            "悪くなかった。\nいずれまた、手合わせを願おう。",
            "もう少しマシなものは居ないのか？\n……これでは腕が鈍るばかりだ。",
            "六弦琴を振るい戦うか\n……小僧、中々に楽しめたぞ。",
            "その腕の武装\n……少し見せてはもらえないだろうか？",
            "なんとも珍妙なものに乗っているな。\n戦の流儀も人それぞれ、か。"
        },

        {
            "楽しかったぜ！またやろうな！！",
            "今日も良いビートだった！アンタも結構ノれてたぜ？",
            "そのガッチガチな装備でよく動けんな……重くねーの？",
            "オネーサンの店にライブステージ作ってくれよ！毎日盛り上げてやっからさ！	",
            "お前らのゴーグルお揃いなのか？ッハハ、仲いいんだな！"
        },
        {
            "",
            "",
            "",
            "",
            ""
        },
        {
            "",
            "",
            "",
            "",
            ""
        },
    };

    //ここのintをSettingのCharaに変えると全て変わる
    Dictionary<int, CharaStatus> chara = new Dictionary<int, CharaStatus>();
    ResultState state;
    Image PlayerT, charatext;
    Text  Word;
    Vector3[] vec = new Vector3[3];
    RectTransform[] Gole = new RectTransform[3];
    private float timeUntilDisplay = 0;     // 表示にかかる時間
    private float timeElapsed = 1;          // 文字列の表示を開始した時間
    private int lastUpdateCharacter = -1;       // 表示中の文字数
    int flag;
    Color changecolor = new Color();

    [SerializeField]
    Image BackGraund, CharaImg;
    [SerializeField]
    GameObject BlackImg;
    [SerializeField,Header("動かす画像")]
    GameObject[] Moves;
    //背景の画像
    [SerializeField,Header("オブジェクトリスト")]
    Sprite[] BackImgs, CharaImags, PlayerImgs, CharaTexts;
    [SerializeField]
    GameObject WordPos;
    //勝利したほうの情報を受け取る
    [SerializeField]
    int WinPlayerID;
    [SerializeField]
    int WinCharaID;
    [SerializeField]
    float _loseCharaId;
    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間
    public float MoveTime;
    float _yPos = -91;

    //キャラの位置調節に必要
    float[] _xSize =
    {
        1.2f,
        1f,
        1.2f,
        1f
    };
    float[] _ySize =
    {
        1.3f,
        1.3f,
        1.2f,
        1f
    };
    
    void Start()
    {
        BlackImg.SetActive(true);
        //プレイヤー情報
        WinPlayerID = (int)AttackManager.winner;
        if (WinPlayerID == 1)
        {
            WinCharaID = (int)Setting.p1c + 1;
        }

        //キャラ情報初期化
        for(int i = 0; i < 4; ++i)
        {
            CharaStatus status = new CharaStatus();
            //セリフをランダムに出現させるため
            status._disWord = CharaWoadInstance();
            status._backImg = BackImgs[i];
            status._charaImg = CharaImags[i];
            status._CharaText = CharaTexts[i];
            chara.Add(i, status);
        }
        
        PlayerT = Moves[0].GetComponent<Image>();
        charatext = Moves[1].GetComponent<Image>();
        Word = Moves[3].GetComponent<Text>();
        //位置取得
        for (int i = 0; i < 3; i++)
        {
            Gole[i] = Moves[i].GetComponent<RectTransform>();
            vec[i] = Moves[i].GetComponent<RectTransform>().position;
            Vector3 pos = Moves[i].transform.position;
            pos.x = -300;
            Moves[i].gameObject.transform.position = pos;
        }
        //アナのみ立ち絵位置修正
        RectTransform rect = CharaImg.GetComponent<RectTransform>();
        if (WinCharaID == 3)
        {
            Vector3 _vec = rect.position;
            _vec.y += _yPos;
            CharaImg.GetComponent<RectTransform>().position = _vec;
        }
        rect.transform.localScale = new Vector3(_xSize[WinCharaID], _ySize[WinCharaID], 1f);
        //背景
        BackGraund.sprite = chara[WinCharaID - 1]._backImg;
        ////色初期化
        changecolor = Color.white;
        //changecolor.a = 0.1f;
        //CharaImg.color = changecolor;

        //キャラクター
        CharaImg.sprite = chara[WinCharaID - 1]._charaImg;
        charatext.sprite = chara[WinCharaID - 1]._CharaText;

        PlayerT.sprite = PlayerImgs[WinPlayerID - 1];

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
                if (changecolor.a >= 1)
                {
                    changecolor.a = 1f;
                    CharaImg.color = changecolor;
                    TextInstance();
                    WordPos.SetActive(true);
                    state = ResultState.TextDis;
                }
                else
                {
                    CharaImg.color = changecolor;
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
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.A)|| _2Pcontroller.GetButtonDown(ControllerManager.Button.A))
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect);
        }
        else if (_1Pcontroller.GetButtonDown(ControllerManager.Button.B) || _2Pcontroller.GetButtonDown(ControllerManager.Button.B))
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
        }
    }
    //キャラクターコメント表示
    void TextDis()
    {
        // クリックから経過した時間が想定表示時間の何%か確認し、表示文字数を出す
        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * chara[WinCharaID - 1]._disWord.Length);
        // 表示文字数が前回の表示文字数と異なるならテキストを更新する
        if (displayCharacterCount != lastUpdateCharacter)
        {
            Word.text = chara[WinCharaID-1]._disWord.Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
        //すべて表示したら飛べるようにする
        if (displayCharacterCount == chara[WinCharaID - 1]._disWord.Length)
        {
            state = ResultState.SceneJump;
        }

    }

    string CharaWoadInstance()
    {
        string _charaText = null;
        //int _charaWordId = Random.Range(0, 3);
        int _charaWordId = Random.Range(0, 2);
        switch (WinCharaID)
        {
            case 1:
                if (_charaWordId==2)
                {
                    switch (_loseCharaId)
                    {
                        case 2:
                            _charaText = _words[WinCharaID - 1, 2];
                            break;
                        case 3:
                            _charaText = _words[WinCharaID - 1, 3];
                            break;
                        case 4:
                            _charaText = _words[WinCharaID - 1, 4];
                            break;
                        default:
                            _charaWordId = Random.Range(0, 2);
                            _charaText = _words[WinCharaID - 1, _charaWordId];
                            break;
                    }

                }
                else
                {
                    _charaText = _words[WinCharaID - 1, _charaWordId];
                }
                break;
            case 2:
                if (_charaWordId == 2)
                {
                    switch (_loseCharaId)
                    {
                        case 1:
                            _charaText = _words[WinCharaID - 1, 2];
                            break;
                        case 3:
                            _charaText = _words[WinCharaID - 1, 3];
                            break;
                        case 4:
                            _charaText = _words[WinCharaID - 1, 4];
                            break;
                        default:
                            _charaWordId = Random.Range(0, 2);
                            _charaText = _words[WinCharaID - 1, _charaWordId];
                            break;

                    }

                }
                else
                {
                    _charaText = _words[WinCharaID - 1, _charaWordId];
                }

                break;
            case 3:
                if (_charaWordId == 2)
                {
                    switch (_loseCharaId)
                    {
                        case 1:
                            _charaText = _words[WinCharaID - 1, 2];
                            break;
                        case 2:
                            _charaText = _words[WinCharaID - 1, 3];
                            break;
                        case 4:
                            _charaText = _words[WinCharaID - 1, 4];
                            break;
                        default:
                            _charaWordId = Random.Range(0, 2);
                            _charaText = _words[WinCharaID - 1, _charaWordId];
                            break;
                    }
                }
                else
                {
                    _charaText = _words[WinCharaID - 1, _charaWordId];
                }

                break;
            case 4:
                if (_charaWordId == 2)
                {
                    switch (_loseCharaId)
                    {
                        case 1:
                            _charaText = _words[WinCharaID - 1, 2];
                            break;
                        case 3:
                            _charaText = _words[WinCharaID - 1, 3];
                            break;
                        case 4:
                            _charaText = _words[WinCharaID - 1, 4];
                            break;
                        default:
                            _charaWordId = Random.Range(0, 2);
                            _charaText = _words[WinCharaID - 1, _charaWordId];
                            break;
                    }
                }
                else
                {
                    _charaText = _words[WinCharaID - 1, _charaWordId];
                }
                break;
            default:
                _charaText = "範囲外が選択されました";
                break;
        }
        return _charaText;
    }

    //テキスト表示初期化
    void TextInstance()
    {
        // 想定表示時間と現在の時刻をキャッシュ
        timeUntilDisplay = chara[WinCharaID - 1]._disWord.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        // 文字カウントを初期化
        lastUpdateCharacter = -1;
    }

    void TextMove()
    {
        RectTransform rect = Moves[flag].GetComponent<RectTransform>();
        //動かす順番を変えている
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
