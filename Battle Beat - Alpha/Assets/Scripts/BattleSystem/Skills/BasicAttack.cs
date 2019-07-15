using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//AttackItemBaseから派生したクラス、例として書いた
public class BasicAttack : AttackItemBase
{
    public float DamageFactor;
    public float BaseDamage;
    public int Delay;
    public int CoolDown;
    public float SpCost;
    protected int NowTurn;
    protected bool canMakeDamage;
    protected bool IsInterrupted;

    protected Floor.Colors fColor;

    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        NowTurn = 0;
        canMakeDamage = false;
        IsInterrupted = false;
    }
    override public void TurnProcessPhase1_Main() {
        if (isCancel)
        {
            return;
        }
        if (NowTurn==Delay)
        {
            Step1();
        }
        else if (NowTurn == Delay+1)
        {
            Step2();
        }
        else
        {
            Step0();
        }
    }
    override public void TurnProcessPhase2_Main()
    {
        if (isCancel)
        {
            return;
        }
        if (IsInterrupted)
        {
            NowTurn = Delay;
            
            IsInterrupted = false;
        }
    }
    void Step0()
    {
        //ここに攻撃モーションを入れる

        if (RootID == 1)
        {
            fColor = Floor.Colors.deeppink;
        }

        else
        {
            fColor = Floor.Colors.skyblue;
        }
        ChangeFloorColor(fColor, 0);

        Debug.Log(string.Format("{0}ターン後、ダメージ判定する",Delay-NowTurn));
    }
    void Step1()
    {
        //ここに攻撃モーションを入れる
        if (RootID == 1)
        {
            fColor = Floor.Colors.deeppink;
        }

        else
        {
            fColor = Floor.Colors.skyblue;
        }
        ChangeFloorColor(fColor, 1);
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
        Debug.Log("ダメージ発生のターン");
    }
    void Step2()
    {
        
    }

    public override void PassDamage(Player player)
    {
        Opponent.TakeDamage(RootPlayer.DamageCalc(BaseDamage) * DamageFactor);
    }
    
    public override bool isEnd()
    {
        if (NowTurn > Delay)
        {
            return true;
        }
        return false;
    }
    public override bool CheckDamage()
    {
        return canMakeDamage;
    }
    public override void OnInterruption()
    {
        IsInterrupted = true;
        /*アニメーションの処理
         * 
         * SomeCodeHere
         * 
         *アニメーションの処理*/
    }
    public override void TurnPreprocess()
    {
        base.TurnPreprocess();
        NowTurn++;
        if (NowTurn >= Delay + 1)
        {
            ChangeFloorColor(fColor, 1);
            canMakeDamage = false;
        }
    }
}
