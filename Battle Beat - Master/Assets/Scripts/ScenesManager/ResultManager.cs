using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;
    SoundManager _soundManager = SoundManager.Instance;


    enum ResultState
    {
        BackDis,
        TextMove,
        CharaDis,
        TextDis,
        SceneJump
    }

    ResultState state;
    Image PlayerT, charatext;
    Text  Word;
    string WordText;
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
    Sprite[]  PlayerImgs;
    [SerializeField]
    GameObject WordPos;
    //勝利したほうの情報を受け取る
    [SerializeField]
    int WinPlayerID;

    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間
    float _yPos = -200;//アナの位置直し
    float _xPos = -300f;

    Setting.Chara winChara = Setting.Chara.HOMI;
    Setting.Chara loseChara = Setting.Chara.HOMI;
    //キャラの位置調節に必要
    float[] _xSize =
    {
        1.4f,
        1f,
        1.2f,
        1f
    };
    float[] _ySize =
    {
        0.8f,
        1.3f,
        1.2f,
        1f
    };

    BaseResultState _state;
    
    void Start()
    {
        _state = new BackDis(_soundManager, BlackImg);
        //プレイヤー情報
        WinPlayerID = (int)AttackManager.winner;
        //_loseCharaId = 3 - WinPlayerID;
        
        if (WinPlayerID == 1)
        {
            winChara = Setting.p1c;
            loseChara = Setting.p2c;
        }
        else if(WinPlayerID == 2)
        {
            winChara = Setting.p2c;
            loseChara = Setting.p1c;
        }
        string path = "CharacterData/" + winChara.ToString();
        CharaData winnerData = Resources.Load<CharaData>(path);
        if (winnerData.backGraund == null)
        {
            Debug.Log("背景が挿入されていません");
        }

        //勝利したほうを表示
        PlayerT = Moves[0].GetComponent<Image>();
        PlayerT.sprite = PlayerImgs[WinPlayerID - 1];
        charatext = Moves[1].GetComponent<Image>();
        charatext.sprite = winnerData.CharaTextImage;
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
        if (winChara == Setting.Chara.ANA)
        {
            Vector3 _vec = rect.position;
            _vec.y += _yPos;
            CharaImg.GetComponent<RectTransform>().position = _vec;
        }
        else if (winChara == Setting.Chara.HOMI)
        {
            Vector3 _vec = rect.position;
            _vec.x += _xPos;
            CharaImg.GetComponent<RectTransform>().position = _vec;
        }
        rect.transform.localScale = new Vector3(_xSize[(int)winChara], _ySize[(int)winChara], 1f);

        //背景
        BackGraund.sprite = winnerData.backGraund;
        ////色初期化
        changecolor = Color.white;
        //changecolor.a = 0.1f;
        //CharaImg.color = changecolor;

        //キャラクター
        CharaImg.sprite = winnerData.Avatar;
        WordText = CharaWoadInstance(winnerData);

        WordPos.SetActive(false);
        state = ResultState.BackDis;

        _soundManager.PlayBGM(BGMID.Result);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ResultState.TextDis:
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
            _soundManager.PlaySE(SEID.General_Controller_Decision);
        }
        else if (_1Pcontroller.GetButtonDown(ControllerManager.Button.B) || _2Pcontroller.GetButtonDown(ControllerManager.Button.B))
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
            _soundManager.PlaySE(SEID.General_Controller_Decision);
        }
    }
    string CharaWoadInstance(CharaData data)
    {
        string _charaText = null;
        if (winChara == loseChara)
        {
            int _index = Random.Range(0, 2);
            switch (_index)
            {
                case 0:
                    _charaText = data.Serifs[(int)winChara];
                    break;
                case 1:
                    _charaText = data.Serifs[data.Serifs.Length-1];
                    break;
            }
        }
        else
        {
            int _index = Random.Range(0, 3);
            switch (_index)
            {
                case 0:
                    _charaText = data.Serifs[(int)loseChara];
                    break;
                case 1:
                    _charaText = data.Serifs[(int)winChara];
                    break;
                case 2:
                    _charaText = data.Serifs[data.Serifs.Length - 1];
                    break;
            }
        }
        return _charaText;
    }
}
