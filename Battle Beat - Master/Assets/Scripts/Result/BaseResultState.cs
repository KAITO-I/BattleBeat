using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseResultState 
{
    protected SoundManager _soundManager;
    protected bool _finish;
    protected CharaData _date;
    protected BaseResultState(SoundManager sound,CharaData date)
    {
        _date = date;
        _soundManager = sound;
        _finish = false;
    }

    public abstract bool Update();
}
