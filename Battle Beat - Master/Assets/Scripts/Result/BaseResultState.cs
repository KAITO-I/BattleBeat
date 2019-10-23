using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseResultState 
{
    protected SoundManager _soundManager;
    protected bool _finish;
    protected BaseResultState(SoundManager sound)
    {
        _soundManager = sound;
        _finish = false;
    }

    public abstract bool Update();
}
