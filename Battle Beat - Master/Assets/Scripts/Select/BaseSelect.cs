using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSelect: MonoBehaviour
{
    protected SelectCountroll _controll;
    protected SoundManager _soundManager;
    protected ControllerManager.Controller _controller;

    [SerializeField]
    protected SpriteRenderer _playerPicture, _playerDescrition;
    [SerializeField]
    protected Image _playerNameImg;
    [SerializeField]
    protected Sprite[] _charaName;
    [SerializeField]
    protected Sprite[] _charaDescrition;

    protected int length;//操作キャラ数
    protected int _playerID;//選択されているキャラ
    protected bool _playerOK, _playerDecritionOK;//選択されているか、説明を出すか

    protected  BaseSelect(SelectCountroll c ,SoundManager s) 
    {
        c = _controll;
        _soundManager = s;
    }
    //指定されたものを返す関数
    public T GetItem<T>(string item=null)
    {
        if (typeof(T) == typeof(SpriteRenderer))
        {
            if (item == "player")
            {
                return (T)(object)_playerPicture;
            }
            return (T)(object)_playerDescrition;
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)_playerID;
        }
        else if (typeof(T) == typeof(bool))
        {
            if (item == "player")
            {
                return (T)(object)_playerOK;
            }
            return (T)(object)_playerDecritionOK;
        }
        else if (typeof(T) == typeof(Image))
        {
            return (T)(object)_playerNameImg;
        }

        return default(T);
    }

    public abstract void playerUpdate();

    protected int InputProcess()
    {
        if (!_playerOK)
        {
            if (_controller.GetAxisUp(ControllerManager.Axis.DpadY) < 0)//上入力
            {
                _controll.CharaObj[_playerID].charaSelect(1, false);
                _playerID++;
                _playerID = _playerID % length;
                _controll.CharaObj[_playerID].charaSelect(1, true);

                //Player_Obj.transform.localScale = new Vector3(_xSize[_Player], _ySize[_Player], 1);
                _soundManager.PlaySE(SEID.General_Controller_Select);
            }
            else if (_controller.GetAxisUp(ControllerManager.Axis.DpadY) > 0)//下入力
            {
                _controll.CharaObj[_playerID].charaSelect(1, false);
                _playerID--;
                _playerID = _playerID % length;
                if (_playerID < 0) _playerID = 3;
                _controll.CharaObj[_playerID].charaSelect(1, true);

                //Player_Obj.transform.localScale = new Vector3(_xSize[_Player], _ySize[_Player], 1);
                _soundManager.PlaySE(SEID.General_Controller_Select);
            }
        }
        return _playerID;
    }

    protected void SelectMove()
    {
        //1P処理
        if (!_boss[0] && _1Pcontroller.GetAxis(ControllerManager.Axis.DpadY) != 0)
        {
            _Player1 = InputProcess(Player01_Obj, _player1Text, _1Pcontroller, Player01, _Player1, Player1_OK, Description_1P, 1);
        }
        else if (_boss[0])
        {
            Player01.sprite = _boss_Sprite;
            _player1Text.sprite = _void;
            Description_1P.sprite = _void;
        }
        #region=============ここからボタン操作======================
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.A))
        {
            Player1_OK = true;
            _MoveTime_1P = 0f;
            if (Player2_OK)
            {
                _soundManager.PlaySE(SEID.General_Siren);
            }
            else
            {
                _soundManager.PlaySE(SEID.General_Controller_Decision);
            }
        }
        //×ボタンの処理
        //長押しで画面移動処理
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.B))
        {
            //キャラ選択時は選択を外す
            if (Player1_OK)
            {
                Player1_OK = false;
                _MoveTime_1P = 0;
                _soundManager.PlaySE(SEID.General_Controller_Back);
            }
            //ホップアップで表示
            if (false)
            {
                if (true) SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
            }
        }
        //説明画面
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.X))
        {
            if (_1PDes) _1PDes = false;
            else _1PDes = true;
        }
        //両方入力されていない
        if (!_1Pcontroller.GetButton(ControllerManager.Button.B) && !_2Pcontroller.GetButton(ControllerManager.Button.B))
        {
        }
        #endregion
        Description_1P.enabled = _1PDes;
        Description_2P.enabled = _2PDes;
    }

}
