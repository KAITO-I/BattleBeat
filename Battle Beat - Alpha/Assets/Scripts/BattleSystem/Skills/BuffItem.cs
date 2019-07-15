using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : AttackItemBase
{
    public int CoolDown;
    public float Power;
    public int Duration;
    int lifeTime;
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        lifeTime = Duration;
    }
    public override void TurnProcessPhase2_Main()
    {
        lifeTime--;
    }
    public override bool isEnd()
    {
        if (lifeTime > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
