using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class Uni_ZoneAnimation : BasePlayerAnimation
{
    [SerializeField]
    GameObject _onUni,_onGogle;//上のユニを非表示
    bool _specialChack = false;
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
        UniDisFunction(false);
        PlayAnim = "Trap";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Special");
        PlayAnim = "Special";
        _specialChack = true;
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

    public void UniDisFunction(bool t)
    {
        _onUni.SetActive(t);
        _onGogle.SetActive(t);
    }

    public override void Damage()
    {
        //base.Damage();
        if (_specialChack)
        {
            anim.SetTrigger("SpecialStop");
            _specialChack = false;
        }
    }
}
