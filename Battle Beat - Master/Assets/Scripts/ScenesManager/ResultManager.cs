using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoreManager;

public class ResultManager : MonoBehaviour
{
    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;
    SoundManager _soundManager = SoundManager.Instance;
    Setting.Chara winChara = Setting.Chara.HOMI;
    Setting.Chara loseChara = Setting.Chara.HOMI;

    List<Vector3> Gole = new List<Vector3>();

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
    //=============キャラの位置調節に必要 =============//
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
    //================================================//
    BaseResultState[] _states = new BaseResultState[4];
    int _stateid;
    
    void Start()
    {
        //==========立ち絵がちゃんとできていたら消す=========
        //プレイヤー情報
        WinPlayerID = (int)TurnManager.winner;
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
        //======================================
        //動かすオブジェクト初期化
        for (int i = 0; i < 3; i++)
        {
            Gole.Add(Moves[i].GetComponent<Transform>().position);   //<-ゴール地点は今の場所だから
        }
        WordPos.SetActive(false);
        //=========動かすクラスを一度にすべて初期化===========
        foreach(var v in Enum.GetValues(typeof(BaseResultState.ClassName)))
        {
            switch (v)
            {/* BackDis->TextMove-> CharaDis->TextDis */
                case BaseResultState.ClassName.BackDis: _states[0] = new BackDis(_soundManager, BlackImg, BackGraund);break;
                case BaseResultState.ClassName.TextMove: _states[1] = new TextMove(_soundManager, Moves, Gole, PlayerImgs); break;
                case BaseResultState.ClassName.CharaDis: _states[2] = new CharaDis(_soundManager, CharaImg); break;
                case BaseResultState.ClassName.TextDis: _states[3] = new TextDis(_soundManager, intervalForCharacterDisplay, WordPos); break;
            }
        }
        //====================================================
        _soundManager.PlayBGM(BGMID.Result);
        _stateid = 0;
    }
    void Update()
    {
        if (SceneLoader.Instance.isLoading) return; // シーン読み込み中は実行しない

        if (_stateid == 4)//すべて終わり次第
        {
            OnClick();
            return;
        }
        //===========終わり次第falseが来る===========//
        if (!_states[_stateid].Update())
        {
            _stateid++;
        }
        //==========================================//
    }
    //==============シーン移動するため==================//
    void OnClick()
    {
        if (PopupManager.IsActive) return;
        if (Input.anyKey) PopupManager.Show(
            ("モウ一度　遊ブ", () => { SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect); }),
            ("タイトルへ戻ル", () => { SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title); })
        );

        //SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect);
        //_soundManager.PlaySE(SEID.General_Controller_Decision);
    }
    //==================================================//
}
