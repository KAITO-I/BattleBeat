using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BasicAttack
{
    bool DamagePassed;
    public override void PassDamage(Player player)
    {
        base.PassDamage(player);
        DamagePassed = true;
    }
    public override void Init(int row, int col, bool reverse, int root)
    {
        transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(col, row));
        transform.position += new Vector3(0, 1f, 0);
        DamagePassed = false;
        
        base.Init(row, col, reverse, root);
        
        canMakeDamage = true;//baseを上書きする
    }
    public override void TurnProcessPhase1_Main()
    {
        if (Reverse)
        {
            Col -= 1;
        }
        else
        {
            Col += 1;
        }
        //ステージ範囲外になったら
        if (Col < 0 || Col > 5)
        {
            //アニメーション
            return;
        }
        //移動処理
        var Pos = BoardManager._instance.ToWorldPos(new Vector2Int(Col, Row));
        transform.position = Pos;
        transform.position += new Vector3(0, 1f, 0);
    }
    public override bool isEnd()
    {
        return DamagePassed||Col<0||Col>5;
    }
    public override void TurnPreprocess()
    {
    }
}
