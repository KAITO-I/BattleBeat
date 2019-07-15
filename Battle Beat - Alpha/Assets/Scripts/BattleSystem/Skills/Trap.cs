using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BasicAttack 
{
    bool DamagePassed;
    public int LiftTime;
    public override void PassDamage(Player player)
    {
        base.PassDamage(player);
        DamagePassed = true;
    }
    public override void Init(int row, int col, bool reverse, int root)
    {
        if (!reverse)
        {
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(col+ Area[0].x, row));
        }
        else
        {
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(col- Area[0].x, row));
        }
        transform.position += new Vector3(0, 1f, 0);
        DamagePassed = false;

        base.Init(row, col, reverse, root);

        canMakeDamage = true;//baseを上書きする
    }
    public override bool isEnd()
    {
        return DamagePassed || NowTurn > LiftTime;
    }
    public override void TurnPreprocess()
    {
        NowTurn++;
    }
    public override void TurnProcessPhase1_Main()
    {
    }
}
