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
        string path = "CharacterData/" + winChara.ToString();
      _date = Resources.Load<CharaData>(path);
        if (_date.backGraund == null)
        {
            Debug.Log("データが存在しておりません");
        }
        _soundManager = sound;
        _updateMove = true;
    }

    public abstract bool Update();
}
