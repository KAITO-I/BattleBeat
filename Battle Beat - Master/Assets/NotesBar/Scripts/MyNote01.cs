using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNote01 : NoteBase
{
    [SerializeField]
    public float Speed;

    public override void DoNextNote()
    {
        throw new System.NotImplementedException();
    }

    public override void SetId(int id)
    {
        throw new System.NotImplementedException();
    }

    public override void SetNextDuration(int duration)
    {
        if (duration > 0)
        {
            this.duration = duration;
        }
        else
        {
            throw new System.Exception("間隔が短過ぎる或いは負数");
        }
    }
}
