using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select1P : BaseSelect
{

    public override void Inctance(SelectCountroll c, SoundManager s)
    {
        _controller = ControllerManager.Instance.Player1;
        _controll.CharaObj[_charactorID].charaSelect(1, true);//選択画面の枠を変える処理
        Gole = Teap.transform.position;
        Gole2 = Teap.transform.position + new Vector3(-500f, 0, 0);
        base.Inctance(c, s);
    }

    public override void playerUpdate()
    {
        InputProcess(1);
        ButtonInput();
        _teapMoveTime = ReadyBerMove(_playerOK, _teapMoveTime);
        _playerDescrition.sprite = _charaDescrition[_charactorDecritionID,_charactorID];
        _playerDescrition.enabled = _playerDecritionOK;
    }

}
