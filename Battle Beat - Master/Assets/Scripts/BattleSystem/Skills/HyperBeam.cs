using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperBeam : BasicAttack
{

    enum Seq
    {
        MTB,//mid->top->bot
        MBT//mid->bot->top
    }
    Seq seq = 0;
    List<Vector2Int> SecondArea;
    List<Vector2Int> ThirdArea;
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        seq = (Seq)Random.Range(0, 2);
        SecondArea = new List<Vector2Int>();
        ThirdArea = new List<Vector2Int>();
        foreach(var p in Area)
        {
            SecondArea.Add(p + new Vector2Int(0, 1) * (seq == Seq.MTB ? -1 : 1));
            ThirdArea.Add(p + new Vector2Int(0, 1) * (seq == Seq.MTB ? 1 : -1));
        }
    }
    public override void TurnPreprocess()
    {
        NowTurn++;
        if (NowTurn >= Delay + 3)
        {
            ChangeFloorColor(fColor, 1,ThirdArea);
            canMakeDamage = false;
        }
    }
    override public void TurnProcessPhase1_Main()
    {
        if (isCancel)
        {
            return;
        }
        //ダメージ発生一ターン目
        if (NowTurn == Delay)
        {
            ChangeFloorColor(fColor, 1,ThirdArea);
            if (RootID == 1)
            {
                fColor = Floor.Colors.red;
            }

            else
            {
                fColor = Floor.Colors.blue;
            }
            ChangeFloorColor(fColor, 0);
            canMakeDamage = true;
        }
        //ダメージ発生二ターン目
        else if (NowTurn == Delay + 1)
        {
            ChangeFloorColor(fColor, 1);
            ChangeFloorColor(fColor, 0, SecondArea);
        }
        //ダメージ発生三ターン目
        else if (NowTurn == Delay + 2)
        {
            ChangeFloorColor(fColor, 1, SecondArea);
            ChangeFloorColor(fColor, 0, ThirdArea);
        }
        //終わった次のタン
        else if (NowTurn >= Delay + 3)
        {

        }
        //wait時
        else if(NowTurn == 0)
        {
            if (RootID == 1)
            {
                fColor = Floor.Colors.deeppink;
            }

            else
            {
                fColor = Floor.Colors.skyblue;
            }
            ChangeFloorColor(fColor, 0);
        }
        else if(NowTurn ==1)
        {
            ChangeFloorColor(fColor, 1);
            ChangeFloorColor(fColor, 0,SecondArea);
        }
        else if (NowTurn == 2)
        {
            ChangeFloorColor(fColor, 1,SecondArea);
            ChangeFloorColor(fColor, 0, ThirdArea);
        }
    }
    public override bool isEnd()
    {
        if (NowTurn >= Delay+3)
        {
            return true;
        }
        return false;
    }
    public override void DamegePhase()
    {
        if (CheckDamage())
        {
            List<Vector2Int> Area;
            if (NowTurn == Delay)
            {
                Area = this.Area;
            }
            else if (NowTurn == Delay + 1)
            {
                Area = SecondArea;
            }
            else if (NowTurn == Delay + 2)
            {
                Area = ThirdArea;
            }
            else
            {
                Area = new List<Vector2Int>();
            }
            foreach (var p in Area)
            {
                Vector2Int pos = AreaProcess(p);
                int OpponentID = 3 - RootID;

                Player Opponent = TurnManager._instance.GetPlayer(OpponentID);
                if (pos == Opponent.Pos)
                {
                    PassDamage(Opponent);
                }
            }
        }
    }
}
