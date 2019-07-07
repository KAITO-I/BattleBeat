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
    private int NowTurn;
    private bool canMakeDamage;
    private bool IsInterrupted;

    Floor.Colors fColor;

    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        NowTurn = -1;
        canMakeDamage = false;
        IsInterrupted = false;
    }
    override public void TurnProcessPhase1() {
        NowTurn++;
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
    override public void TurnProcessPhase2()
    {
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
        ChangeFloorColor(fColor, 1);
        canMakeDamage = false;
    }
    private void ChangeFloorColor(Floor.Colors color,int mode=0)
    {
        foreach (var Grid in Area)
        {
            Vector2Int pos;
            if (Reverse)
            {
                pos = new Vector2Int(Col - Grid.x, Row + Grid.y - 1);
            }
            else
            {
                pos = new Vector2Int(Col + Grid.x, Row + Grid.y - 1);
            }
            var floor = BoardManager._instance.GetGameObjectAt(pos, RootID);
            if (floor != null)
            {
                var floorObj = floor.GetComponent<Floor>();
                if (mode == 0)
                {
                    floorObj.AddColor(color);
                }
                else
                {
                    floorObj.SubColor(color);
                }
            }
        }
    }

    public override void PassDamage(Player player)
    {
        Player playerRoot = AttackManager._instance.GetPlayer(RootID);
        float buff_const=playerRoot.GetSpecialParameter("Buff");
        player.TakeDamage((BaseDamage+buff_const) * DamageFactor);
    }
    public override bool CheckArea(Vector2Int pos,int rootId)
    {
        Vector2Int _Area;
        for (int i = 0; i < Area.Count; i++)
        {
            if (Reverse)
            {
                _Area = new Vector2Int(Col - Area[i].x, Row + Area[i].y - 1);
            }
            else
            {
                _Area = new Vector2Int(Col + Area[i].x, Row + Area[i].y - 1);
            }
            if (pos ==_Area)
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
}
