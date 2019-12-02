using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaDis : BaseResultState
{
    Color changecolor = new Color();
    Image CharaImg;
    public CharaDis(SoundManager s,Image c):base(s)
    {
        _className = ClassName.CharaDis;

        CharaImg = c;
        CharaImg.sprite = _date.Avatar;
        ////色初期化
        changecolor = Color.white;
        //changecolor.a = 0.1f;
        //CharaImg.color = changecolor;

    }
    public override bool Update()
    {
        changecolor.a += 0.1f;
        if (changecolor.a >= 1)
        {
            changecolor.a = 1f;
            CharaImg.color = changecolor;
            _updateMove = true;
        }
        else
        {
            CharaImg.color = changecolor;
        }
        return _updateMove;
    }

}
