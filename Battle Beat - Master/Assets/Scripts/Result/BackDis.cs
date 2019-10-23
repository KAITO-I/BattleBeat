using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackDis :BaseResultState
{
    GameObject BlackImg;

    public BackDis(SoundManager s,GameObject g) : base(s)
    {
        BlackImg = g;
        BlackImg.SetActive(true);

    }

    public override bool Update()
    {
        BlackImg.transform.position += new Vector3(150f, 0f, 0f);
        if (BlackImg.transform.position.x > 3000) _finish = true;
        _soundManager.PlaySE(SEID.Game_Character_General_Move);
        return _finish;
    }
}
