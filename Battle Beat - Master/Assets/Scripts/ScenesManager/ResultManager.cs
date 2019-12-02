using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;
    SoundManager _soundManager = SoundManager.Instance;
    Setting.Chara winChara = Setting.Chara.HOMI;
    Setting.Chara loseChara = Setting.Chara.HOMI;

    Transform[] Gole;

    [SerializeField]
    Image BackGraund, CharaImg;
    [SerializeField]
    GameObject BlackImg;
    [SerializeField,Header("動かす画像")]
    GameObject[] Moves;
    //背景の画像
    [SerializeField,Header("1Por2P")]
    Sprite[]  PlayerImgs;
    [SerializeField]
    GameObject WordPos;
    //勝利したほうの情報を受け取る
    [SerializeField]
    int WinPlayerID;
    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間

    //キャラの位置調節に必要 
    float _yPos = -200;//アナの位置直し
    float _xPos = -300f;

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
        //プレイヤー情報
        WinPlayerID = (int)AttackManager.winner;
        
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
        //動かすオブジェクト初期化
        for (int i = 0; i < 3; i++)
        {
            Gole[i] = Moves[i].GetComponent<RectTransform>();
        }
        #region========立ち絵修正===========
        //アナ立ち絵位置修正
        RectTransform rect = CharaImg.GetComponent<RectTransform>();
        if (winChara == Setting.Chara.ANA)
        {
            Vector3 _vec = rect.position;
            _vec.y += _yPos;
            CharaImg.GetComponent<RectTransform>().position = _vec;
        }
        else if (winChara == Setting.Chara.HOMI)//ホーミー立ち絵位置修正
        {
            Vector3 _vec = rect.position;
            _vec.x += _xPos;
            CharaImg.GetComponent<RectTransform>().position = _vec;
        }
        rect.transform.localScale = new Vector3(_xSize[(int)winChara], _ySize[(int)winChara], 1f);
        #endregion
        WordPos.SetActive(false);

        _state = new BackDis(_soundManager, BlackImg, BackGraund);
        _soundManager.PlayBGM(BGMID.Result);
    }

    // Update is called once per frame
    void Update()
    {

        if (_state.Update())
        {
            switch (_state._className)//処理が終わったら
            { /* BackDis->TextMove-> CharaDis->TextDis */
                case BaseResultState.ClassName.BackDis: new TextMove(_soundManager, Gole); break;
                case BaseResultState.ClassName.TextMove: new CharaDis(_soundManager, CharaImg); break;
                case BaseResultState.ClassName.CharaDis: new TextDis(_soundManager, intervalForCharacterDisplay); break;
                case BaseResultState.ClassName.TextDis: OnClick(); break;
            }
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
}
