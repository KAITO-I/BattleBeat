using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//AttackItemBaseから派生したクラス、例として書いた
public class BasicAttack : AttackItemBase
{
    public float DamageFactor;
    public float BaseDamage;
    public int Delay;
    [SerializeField]
    SEID attackSE;
    [SerializeField]
    SEID hitSE;
    [SerializeField]
    SEID waitSE;
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
            SoundManager.Instance.PlaySE(attackSE);
            Debug.Log("ダメージ発生のターン");
        }
        else if (NowTurn == Delay+1)
        {
            
        }
        else
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
            SoundManager.Instance.PlaySE(waitSE);
            Debug.Log(string.Format("{0}ターン後、ダメージ判定する", Delay - NowTurn));
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

    public override void PassDamage(Player player)
    {
        Opponent.TakeDamage(RootPlayer.DamageCalc(BaseDamage) * DamageFactor);
        SoundManager.Instance.PlaySE(hitSE);
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
