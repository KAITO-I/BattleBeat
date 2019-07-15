using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : AttackItemBase
{
    bool Interrupted = false;
    public float Power = 1f;
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        (RootPlayer as Yunizon).Guard = true;
    }
    public override void DamegePhase()
    {
    }
    public override void OnInterruption()
    {
        Interrupted = true;
    }
    public override bool isEnd()
    {
        return Interrupted;
    }
    public override void TurnProcessPhase1_Main()
    {
        RootPlayer.SetHp(RootPlayer.GetHp() + Power);
    }
    public override void TurnPostprocess()
    {
        if (Interrupted)
        {
            (RootPlayer as Yunizon).Guard = false;
        }
    }
}
