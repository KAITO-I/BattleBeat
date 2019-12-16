using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class Uni_ZoneAnimation : BasePlayerAnimation
{
    [SerializeField]
    GameObject _onUni;//上のユニを非表示
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
        anim.SetTrigger("Trap");
        _onUni.SetActive(false);
        PlayAnim = "Trap";
        //ユニ単体出現
        //Instantiate(_uniObj,transform);
        //UniAnimation _unianim = _uniObj.GetComponent<UniAnimation>();
        //_unianim.UniAnim(UniAnimation.UniState.Start,gameObject,);
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
