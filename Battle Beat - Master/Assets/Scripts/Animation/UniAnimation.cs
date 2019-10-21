using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniAnimation : BasePlayerAnimation
{
    public enum UniState
    {
        Start,
        Wait,
        Attack,
        Back
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void UniAnim(GameObject player, Vector3 Gole,UniState state)
    {
        switch (state)
        {
            case UniState.Start:
                anim.SetTrigger("Start");

                StartCoroutine(enumerator(player,Gole));//移動
                break;
            case UniState.Wait:
                anim.SetTrigger("Wait");
                break;
            case UniState.Attack:
                anim.SetTrigger("Attack");
                break;
            case UniState.Back:
                anim.SetTrigger("Back");
                break;
        }
    }

}
