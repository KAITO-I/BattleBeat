using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select1P : BaseSelect
{
    public override void Inctance(SelectCountroll c, SoundManager s)
    {
        base.Inctance(c, s);
        _controller = ControllerManager.Instance.Player1;
        _controll.CharaObj[_charactorID].charaSelect(1, true);//選択画面の枠を変える処理
        Gole = Teap.transform.position;
        Gole2 = Teap.transform.position + new Vector3(-500f, 0, 0);
    }

    public override void playerUpdate()
    {
        InputProcess(1);
        ButtonInput();
        _teapMoveTime = ReadyBerMove(_playerOK, _teapMoveTime);
        _playerDescrition.sprite = discritions[_charactorID]._discritionSprites[_charactorDecritionID];
        _playerDescrition.enabled = _playerDecritionOK;
        _playerPicture.sprite = _controll.CharaObj[_charactorID].GetCharaSprite;
        _playerNameImg.sprite = _charaName[_charactorID];
    }

}
