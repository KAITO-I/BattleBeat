using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMove : BaseResultState
{
    int flag;
    Transform[] Moves;
    Transform[] Gole;
    float MoveTime;
    TextMove(SoundManager s) : base(s)
    {
        flag = 0;
        MoveTime = 0f;
    }

    public override bool Update()
    {
        Transform rect = Moves[flag].GetComponent<Transform>();
        Moves[flag].transform.position = Vector3.Lerp(rect.position, Gole[flag].position, MoveTime);
        MoveTime += 0.1f;
        if (Moves[flag].transform.position == rect.position)
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
