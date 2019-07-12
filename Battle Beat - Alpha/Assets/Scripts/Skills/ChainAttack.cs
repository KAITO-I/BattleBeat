using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAttack : AttackItemBase
{
    public int CoolDown;
    public float SpCost;

    int turn;
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        turn = -1;
    }
    public override void TurnProcessPhase0_Prediction_Request()
    {
        turn++;
        
        if (turn == 0)
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
    }
    public override void TurnProcessPhase0_Main()
    {
        
        if (turn == 0)
        {
            var RootPlayer = AttackManager._instance.GetPlayer(RootID) as Kagura;

            int OpponentID = 3 - RootID;

            Player Opponent = AttackManager._instance.GetPlayer(OpponentID);

            if (RootPlayer.ChainAttackHit == true && RootPlayer.IsStuned != true)
            {
                RootPlayer.StunTurn += 2;
                Opponent.StunTurn += 2;
                Opponent.IsStuned = true;
                RootPlayer.IsStuned = true;
            }
            //鎖攻撃の相打ち
            else if (RootPlayer.ChainAttackHit == true && RootPlayer.IsStuned == true)
            {
                RootPlayer.ChainAttackHit = false;
                RootPlayer.IsStuned = false;

            }
        }
    }
    public override void TurnProcessPhase1_Main()
    {
        

        if(turn ==1)
        {
            var RootPlayer = AttackManager._instance.GetPlayer(RootID) as Kagura;

            int OpponentID = 3 - RootID;

            Player Opponent = AttackManager._instance.GetPlayer(OpponentID);

            //Hitした場合
            if (RootPlayer.ChainAttackHit == true)
            {
                Opponent.ForcedMovement(Reverse ? new Vector2Int(Col - 1, Row) : new Vector2Int(Col + 1, Row));
            }
            RootPlayer.ChainAttackHit = false;
        }
    }
    public override bool isEnd()
    {
        if (turn == 0)
        {
            return false;
        }
        return true;
    }
}
