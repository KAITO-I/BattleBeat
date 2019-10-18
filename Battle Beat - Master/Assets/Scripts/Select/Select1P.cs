using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select1P : BaseSelect
{
    public Select1P(SelectCountroll c,SoundManager s):base(c,s)
    {
        _controller = ControllerManager.Instance.Player1;

        _playerID = 0;
        _playerOK = false;
        _playerDecritionOK = false;
        _playerNameImg.sprite =_controll. _ChataText[_playerID];
        _playerPicture.sprite = _controll.CharaObj[_playerID].GetCharaSprite;
        //Player01_Obj.transform.localScale = new Vector3(_xSize[_Player1], _ySize[_Player1], 1);
        _playerDescrition.sprite = _controll.ChareDescriptions[_playerID];
        _playerDescrition.enabled = _playerDecritionOK;

        _controll.CharaObj[_playerID].charaSelect(1, true);//選択画面の枠を変える処理

    }

    public override void Update()
    {
        InputProcess();
    }

    private int InputProcess()
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

}
