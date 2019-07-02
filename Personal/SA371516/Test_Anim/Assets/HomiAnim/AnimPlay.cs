using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlay : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo info_;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        info_ = anim.GetCurrentAnimatorStateInfo(0);
        //連打処理未対応
        //アニメーションがidolに代わる前に位置をずらすとちょうどいい
        if (info_.IsName("idol"))
        {
            TestAnim();
        }
    }
    void TestAnim()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("LeftT");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("RightT");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger("BackT");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("FlontT");
        }
    }
}
