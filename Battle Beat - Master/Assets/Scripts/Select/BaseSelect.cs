using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSelect: MonoBehaviour
{
    protected SelectCountroll _controll;
    protected SoundManager _soundManager;
    protected ControllerManager.Controller _controller;
    [SerializeField]
    protected SpriteRenderer _playerPicture, _playerDescrition;
    [SerializeField]
    protected Image _playerNameImg;
    protected int _playerID;
    protected bool _playerOK, _playerDecritionOK;

    protected  BaseSelect(SelectCountroll c ,SoundManager s) 
    {
        c = _controll;
        _soundManager = s;
    }
    //指定されたものを返す関数
    public T GetItem<T>(string item=null)
    {
        if (typeof(T) == typeof(SpriteRenderer))
        {
            if (item == "player")
            {
                return (T)(object)_playerPicture;
            }
            return (T)(object)_playerDescrition;
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)_playerID;
        }
        else if (typeof(T) == typeof(bool))
        {
            if (item == "player")
            {
                return (T)(object)_playerOK;
            }
            return (T)(object)_playerDecritionOK;
        }
        else if (typeof(T) == typeof(Image))
        {
            return (T)(object)_playerNameImg;
        }

        return default(T);
    }

    public abstract void Update();  
}
