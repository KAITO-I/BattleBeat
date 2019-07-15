using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRangeAttack : BasicAttack
{
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(1, col, reverse, root);
    }
}
