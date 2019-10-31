using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select2P :BaseSelect
{
    public override void Inctance(SelectCountroll c, SoundManager s)
    {
        _controller = ControllerManager.Instance.Player2;
        _controll.CharaObj[_charactorID].charaSelect(2, true);//選択画面の枠を変える処理
        Gole = Teap.transform.position;
        Gole2 = Teap.transform.position + new Vector3(500f, 0, 0);  
        base.Inctance(c, s);
    }
    public override void playerUpdate()
    {
        InputProcess(2);
        ButtonInput();
        _teapMoveTime = ReadyBerMove(_playerOK, _teapMoveTime);
        _playerDescrition.sprite = discritions[_charactorID]._discritionSprites[_charactorDecritionID];
        _playerDescrition.enabled = _playerDecritionOK;
        _playerPicture.sprite = _controll.CharaObj[_charactorID].GetCharaSprite;
    }

}
