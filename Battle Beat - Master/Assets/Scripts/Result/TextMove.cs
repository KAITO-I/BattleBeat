using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//左から文字をスライドさせてくるクラス
public class TextMove : BaseResultState
{
    int flag;
    int _id;
    Transform[] Moves;
    Transform[] Gole;
    Image _playerStandImg,_charaNameImg;
    float MoveTime;

    public TextMove(SoundManager s,Transform[] move) : base(s)
    {
        _className = ClassName.TextMove;

        flag = 0;
        MoveTime = 0f;
        _id = (int)AttackManager.winner;

        Moves = move;
        Gole = move;
        //勝利したほうを表示
        _playerStandImg = Moves[0].GetComponent<Image>();
        _playerStandImg.sprite = _date.Avatar;
        _charaNameImg = Moves[1].GetComponent<Image>();
        _charaNameImg.sprite = _date.CharaTextImage;

        //初期位置
        for (int i = 0; i < 3; ++i)
        {
            Vector3 pos = Moves[i].transform.position;
            pos.x = -300;
            Moves[i].gameObject.transform.position = pos;
        }
    }

    public override bool Update()
    {
        Vector3 rect = Moves[flag].GetComponent<Transform>().position;
        Moves[flag].transform.position = Vector3.Lerp(rect, Gole[flag].position, MoveTime);
        MoveTime += 0.1f;
        if (Moves[flag].transform.position == rect)
        {
            flag++;
            MoveTime = 0f;
            if (flag == 3)
            {
                _finish = true;
            }
            else
            {
                _soundManager.PlaySE(SEID.Game_Character_General_Move);
            }
        }
        return _finish;
    }
}
