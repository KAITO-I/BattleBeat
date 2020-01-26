using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniAnimation : BasePlayerAnimation
{
    public enum UniState
    {
        Start,
        Attack,
        Back
    }
    Vector3 _Gole;
    GameObject _uniZoneObj;
    public GameObject GetSetUniZoneObj
    {
        get { return _uniZoneObj; }
        set { _uniZoneObj = value; }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        AnimatorStateInfo _info = anim.GetCurrentAnimatorStateInfo(0);
        if (_info.IsName("naguru") || _info.IsName("Back"))//
        {
            anim.speed *= 2;//①テンポで帰らなければならないため
            if (_info.IsName("Back"))
            {
                StartCoroutine(MoveColutin(gameObject, _uniZoneObj.transform.position));
            }
        }
        else if (_info.IsName("Back0"))
        {
            StartCoroutine(MoveColutin(gameObject, _uniZoneObj.transform.position));
        }
        else if (_info.IsName("dashu0") || _info.IsName("naguru0") || _info.IsName("Back1"))//指定した場所に敵がいた場合
        {
            anim.speed *= 3;//①テンポで帰らなければならないため
            if (_info.IsName("Back1")){
                StartCoroutine(MoveColutin(gameObject, _uniZoneObj.transform.position));
            }
        }
    }
    //トラップの時はこの関数を呼んでもらう
    //(現在の状況、ユニのオブジェクト,目的地,どっちが出したか)
    public void UniAnim(UniState state,GameObject uni=null, Vector3? Gole = null, bool pID=false)
    {
        Vector3 position = Gole ?? Vector3.zero;
        _Gole = position;//戻るときにUpdate関数で行わなければならないため
        switch (state)
        {
            case UniState.Start:
                Vector3 vec = _Gole;
                if (!pID)//デバッグできないため未確認
                {
                    vec += new Vector3(1, 0, -1);
                }
                else vec += new Vector3(-1, 0, -1);
                StartCoroutine(MoveColutin(uni,vec));//移動
                break;
            case UniState.Attack://攻撃時は自動的に戻るアニメーションが再生される
                anim.SetTrigger("Attack");
                break;
            case UniState.Back:
                anim.SetTrigger("Back");
                break;
        }
    }
    protected override IEnumerator MoveColutin(GameObject obj, Vector3 Goal)
    {
        yield return StartCoroutine(base.MoveColutin(obj,Goal));
        AnimatorStateInfo _info = anim.GetCurrentAnimatorStateInfo(0);
        if (_info.IsName("Back")|| _info.IsName("Back0") || _info.IsName("Back1"))
        {
            GetSetUniZoneObj.GetComponent<Uni_ZoneAnimation>().UniDisFunction(true);
            Destroy(gameObject);
        }
        gameObject.transform.Rotate(0, 180, 0);
        yield return new WaitForFixedUpdate();
    }
}
