using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//左から文字をスライドさせてくるクラス
public class TextMove : BaseResultState
{
    int flag;                           //動かすもののID
    GameObject[] Moves;     //動かすもの
    List<Vector3> Gole;   //到達地点//<-GetCompornentにすると同期してしまう
    Image _playerDisImg,_charaNameImg;    //画像
    float MoveTime;             
    Vector3 _start;             //初期位置を保存するため

    public TextMove(SoundManager s,GameObject[] move,List<Vector3> gole,Sprite[] im) : base(s)
    {
        _className = ClassName.TextMove;

        flag = 0;
        MoveTime = 0f;
        Moves = move;
        Gole = gole;
        
        for (int i = 0; i < 3; i++)//ここで位置をずらす
        {
            Vector3 pos = Moves[i].transform.position;
            pos.x = -300;
            Moves[i].transform.position = pos;               //<-スタート地点をずらして入れる
        }
        _start = Moves[flag].transform.position;      //初期位置を保存する
        //勝利したほうを表示
        _playerDisImg = Moves[0].GetComponent<Image>();
        _playerDisImg.sprite = im[(int)TurnManager.winner - 1];   //Playerのどっちが勝ったかを表示
        _charaNameImg = Moves[1].GetComponent<Image>();
        _charaNameImg.sprite = _date.CharaTextImage;                    //キャラの名前を表示

        //Debug.Log(_updateMove);
    }

    public override bool Update()
    {
        Moves[flag].transform.position = Vector3.Lerp(_start, Gole[flag], MoveTime);
        MoveTime += 0.1f;
        if (Moves[flag].transform.position == Gole[flag])
        {
            flag++;
            MoveTime = 0f;
            _start = Moves[flag].transform.position;//初期位置を保存する
            if (flag == 3)
            {
                _updateMove = false;
            }
            else
            {
                _soundManager.PlaySE(SEID.Game_Character_General_Move);
            }
        }
        return _updateMove;
    }
}
