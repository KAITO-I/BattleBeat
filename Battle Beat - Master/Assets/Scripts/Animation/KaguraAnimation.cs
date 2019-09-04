using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class KaguraAnimation :BasePlayerAnimation
{
    Animator _kusariAnim;
    Animator _toguroAnim;
    protected override void Start()
    {
        base.Start();
        _kusariAnim = gameObject.transform.GetChild(2).GetComponent<Animator>();
    }
    protected override void Attack1()
    {
        anim.SetTrigger("Attack1");
        PlayAnim = "Attack1";
    }
    protected override void Attack2()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    protected override void Attack3()
    {
        anim.SetTrigger("PullAttack");
        KusariAnim("Kusari");
        PlayAnim = "PullAttack";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    //鎖のアニメーションを再生する関数（鎖攻撃が当たった,敵のオブジェクト）
    public void KusariAnim(string _triggerName, bool _attack = false,GameObject _enemyObj=null)
    {
        if (_attack)//相手につけている鎖のアニメーションを再生させる
        {
            _toguroAnim = _enemyObj.transform.GetChild(1).GetComponent<Animator>();
            _enemyObj.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;//表示
            _toguroAnim.SetTrigger("Toguro");
        }
        else　if(_enemyObj!=null)//引き寄せ後の処理
        {
            _enemyObj.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;//表示
            _kusariAnim.SetTrigger(_triggerName);
        }
        else//当たらない場合の処理
        {
            _kusariAnim.SetTrigger(_triggerName);
        }
    }

    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("HorizontalAttack");
                PlayAnim = "HorizontalAttack";
                break;
            case 3:
                anim.SetTrigger("Special");
                PlayAnim = "Special";
                break;
            default:
                Debug.Log("入力が違います");
                break;
        }
    }

}
