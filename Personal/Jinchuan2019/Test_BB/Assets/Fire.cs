using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//AttackItemBaseから派生したクラス、例として書いた
public class Fire : AttackItemBase
{
    public float DamageFactor;
    public float BaseDamage;
    public int Delay;
    private int NowTurn;
    private bool canMakeDamage;

    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(0, 0, reverse, root);
        NowTurn = -1;
        canMakeDamage = false;
    }
    override public void TurnProcess() {
        NowTurn++;
        if (NowTurn==Delay)
        {
            Step1();
        }
        else
        {
            Step0();
        }
    }
    void Step0()
    {
        //ここに攻撃モーションを入れる



        Debug.Log(string.Format("{0}ターン後、ダメージ判定する",Delay-NowTurn));
    }
    void Step1()
    {
        //ここに攻撃モーションを入れる



        canMakeDamage = true;
        Debug.Log("ダメージ発生のターン");
    }
    public override void PassDamage(Player player)
    {
        player.TakeDamage(BaseDamage * DamageFactor);
    }
    public override bool CheckArea(Vector2Int pos,int rootId)
    {
        if (Reverse)
        {
            pos.x = 2 - pos.x;
        }
        for(int i = 0; i < Area.Count; i++)
        {
            if (pos + new Vector2Int(Col,Row) == Area[i])
            {
                if (rootId != RootID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public override bool isEnd()
    {
        if (NowTurn >= Delay)
        {
            return true;
        }
        return false;
    }
    public override bool CheckDamage()
    {
        return canMakeDamage;
    }
}
