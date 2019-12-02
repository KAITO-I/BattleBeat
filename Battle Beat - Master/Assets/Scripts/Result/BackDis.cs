using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackDis :BaseResultState
{
    GameObject Blackobj;
    Image _backImg;
    public BackDis(SoundManager s,GameObject g,Image img) : base(s)
    {
        _className = ClassName.BackDis;
        _backImg = img;
        Blackobj = g;
        Blackobj.SetActive(true);
        //背景
        _backImg.sprite = _date.backGraund;

    }

    public override bool Update()
    {
        Blackobj.transform.position += new Vector3(150f, 0f, 0f);
        if (Blackobj.transform.position.x > 3000) _finish = true;
        _soundManager.PlaySE(SEID.Game_Character_General_Move);
        return _finish;
    }
}
