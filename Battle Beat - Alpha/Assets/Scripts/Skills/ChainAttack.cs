using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAttack : AttackItemBase
{
    public int CoolDown;
    public float SpCost;

    public override void TurnProcessPhase0()
    {
        var RootPlayer = AttackManager._instance.GetPlayer(RootID) as Kagura;

        int OpponentID = 3 - RootID;

        Player Opponent = AttackManager._instance.GetPlayer(OpponentID);
        foreach (var Grid in Area)
        {
            Vector2Int pos;
            pos = AreaProcess(Grid);
            if (pos == Opponent.Pos)
            {
                Opponent.IsStuned = true;
                RootPlayer.ChainAttackHit = true;
            }
        }
    }
    public override void TurnProcessPhase1()
    {
        var RootPlayer = AttackManager._instance.GetPlayer(RootID) as Kagura;

        int OpponentID = 3 - RootID;

        Player Opponent = AttackManager._instance.GetPlayer(OpponentID);

        //Hitした場合
        if (RootPlayer.ChainAttackHit == true && RootPlayer.IsStuned != true)
        {
            Opponent.ForcedMovement(Reverse ? new Vector2Int(Col - 1, Row) : new Vector2Int(Col + 1, Row));
        }
        //鎖攻撃の相打ち
        else if (RootPlayer.ChainAttackHit == true && RootPlayer.IsStuned == true)
        {

        }
    }
    public override bool isEnd()
    {
        return true;
    }
}
