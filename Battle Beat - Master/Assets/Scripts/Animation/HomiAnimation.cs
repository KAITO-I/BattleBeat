using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class HomiAnimation : BasePlayerAnimation
{
    [SerializeField]
    GameObject Anp;
    GameObject obj;
    protected override void Start()
    {
        base.Start();
        _renderer = transform.GetChild(1).GetComponent<MeshRenderer>();
        _renderer.enabled = false;
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
        anim.SetTrigger("Buff");
        PlayAnim = "Buff";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Wait");
        Vector3 vec = gameObject.transform.position;
        if (_playerClass.PlayerID == 1)
        {
            vec += new Vector3(-2, 0, 0);
            obj = Instantiate(Anp, vec, Quaternion.identity);
        }
        else
        {
            vec += new Vector3(2, 0, 0);
            obj = Instantiate(Anp, vec, Quaternion.identity);
            obj.gameObject.transform.Rotate(0, 180, 0);
        }
        PlayAnim = "Wait";
    }
    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("MusicAttack");
                PlayAnim = "MusicAttack";
                break;
            case 3:
                anim.SetTrigger("Special");
                Destroy(obj, rythm.getbps);
                PlayAnim = "Special";
                break;
            default:
                Debug.Log("入力が違います");
                break;
        }
    }
}
