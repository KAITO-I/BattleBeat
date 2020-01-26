using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class Uni_ZoneAnimation : BasePlayerAnimation
{
    [SerializeField]
    GameObject _onUni;//上のユニを非表示
    [SerializeField]
    protected override void Attack1()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    protected override void Attack2()
    {
        anim.SetTrigger("RocketAttack");
        PlayAnim = "RocketAttack";
    }
    protected override void Attack3()
    {
        _onUni.SetActive(false);
        PlayAnim = "Trap";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Special");
        PlayAnim = "Special";
    }
    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("BombAttack");
                PlayAnim = "BombAttack";
                break;
            default:
                break;
        }
    }
}
