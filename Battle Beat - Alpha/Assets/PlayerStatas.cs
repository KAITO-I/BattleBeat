//------------------------------------
//作成者：木原　プレイヤーステータス
//------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatas : MonoBehaviour
{
    [SerializeField] int pnum;//プレイヤー番号
    [SerializeField] int id;//キャラID
    [SerializeField] int hp;//現在HP
    [SerializeField] int sp;//現在SP

//    public void Init(int pnum, )

    public int GetId()
    {
        return id;
    }
    public void SetId(int i)
    {
        id = i;
    }
    public int GetHP()
    {
        return hp;
    }
    //HP減少(回復するならvalueを-にして)
    public void SetHP(int value)
    {
        hp -= value;
    }
    public int GetSP()
    {
        return sp;
    }
    public void SetSP(int value)
    {
        sp += value;
    }
}
