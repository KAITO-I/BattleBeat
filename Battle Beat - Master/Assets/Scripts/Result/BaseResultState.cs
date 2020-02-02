using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseResultState 
{
    protected SoundManager _soundManager;
    protected bool _updateMove;
    protected CharaData _date;
    protected Setting.Chara winChara;
    protected Setting.Chara loseChara;

    public enum ClassName
    {
        BackDis,
        TextMove,
        CharaDis,
        TextDis,
    }

    public ClassName _className;

    protected BaseResultState(SoundManager sound)
    {
        //勝者を保存
        int _id = (int)TurnManager.winner;
        if (_id == 1)
        {
            winChara = Setting.p1c;
            loseChara = Setting.p2c;
        }
        else if (_id == 2)
        {
            winChara = Setting.p2c;
            loseChara = Setting.p1c;
        }
        string path = "CharacterData/" + winChara.ToString();
      _date = Resources.Load<CharaData>(path);
        if (_date == null)
        {
            Debug.Log("データが存在しておりません");
        }
        _soundManager = sound;
        _updateMove = true;
    }

    public abstract bool Update();
}
