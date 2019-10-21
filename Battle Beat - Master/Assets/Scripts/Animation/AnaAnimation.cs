using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnaAnimation : BasePlayerAnimation
{
    [SerializeField]
    GameObject[] AnaAnimObj = new GameObject[4];
    protected override void Attack1()
    {
        //anim.SetTrigger("Attack1");
        gameObject.SetActive(false);
        GameObject obj=  Instantiate(AnaAnimObj[0], transform.position, transform.rotation);
        obj.transform.parent = gameObject.transform;//アナprefabの子にする
        PlayAnim = "Attack1";
    }
    protected override void Attack2()
    {
        //anim.SetTrigger("Wait");
        gameObject.SetActive(false);
        GameObject obj = Instantiate(AnaAnimObj[0], transform.position, transform.rotation);
        obj.transform.parent = gameObject.transform;//アナprefabの子にする
        PlayAnim = "Wait";
    }
    protected override void Attack3()
    {
        //anim.SetTrigger("Wait");
        gameObject.SetActive(false);
        GameObject obj = Instantiate(AnaAnimObj[0], transform.position, transform.rotation);
        obj.transform.parent = gameObject.transform;//アナprefabの子にする
        PlayAnim = "Wait";
    }
    protected override void Attack4()
    {
        //anim.SetTrigger("Wait");
        gameObject.SetActive(false);
        GameObject obj = Instantiate(AnaAnimObj[0], transform.position, transform.rotation);
        obj.transform.parent = gameObject.transform;//アナprefabの子にする
        PlayAnim = "Wait";
    }
    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("LongDistanceAttack");
                PlayAnim = "LongDistanceAttack";
                break;
            case 2:
                anim.SetTrigger("ShortDistanceAttack");
                PlayAnim = "ShortDistanceAttack";
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
