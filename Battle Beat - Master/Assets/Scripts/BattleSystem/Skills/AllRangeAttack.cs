using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRangeAttack : BasicAttack
{
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(1, col, reverse, root);
    }
    public override void PassDamage(Player player)
    {
        Homi homi = RootPlayer as Homi;
        if (homi!=null)
        {
            Opponent.TakeDamage(RootPlayer.DamageCalc(BaseDamage- homi.buffPower) * DamageFactor * Mathf.Max(homi.buffPower,1f));
        }
        else
        {
            Opponent.TakeDamage(RootPlayer.DamageCalc(BaseDamage) * DamageFactor);
        }
    }
}
