//------------------------------------
//作成者：木原　プレイヤーステータス
//------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
struct PlData
{
    public int id;//キャラID
    public int hp;//現在HP
    public int sp;//現在SP
    public CharacterStatas mychara;
}
public class PlayerStatas : MonoBehaviour
{
    [SerializeField] int pnum;//プレイヤー番号
    private static PlData[] playerDatas = new PlData[2];

    // Start is called before the first frame update
    void Start()
    {
        Init();
        playerDatas[0].id = 1;
        playerDatas[1].id = 3;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init()
    {
        for(int i = 0;i < playerDatas.Length; i++)
        {
            playerDatas[i].id = 0;
            playerDatas[i].hp = 20;
            playerDatas[i].sp = 0;
            playerDatas[i].mychara = null;
        }
       
    }

    public int GetId(int num)
    {
        return playerDatas[num-1].id;
    }
    public void SetId(int num,int i)
    {
        playerDatas[num - 1].id = i;
    }
    public int GetHP(int num)
    {
        return playerDatas[num - 1].hp;
    }
    //HP減少(回復するならvalueを-にして)
    public void SetHP(int num,int value)
    {
        playerDatas[num - 1].hp -= value;
    }
    public int GetSP(int num)
    {
        return playerDatas[num - 1].sp;
    }
    public void SetSP(int num,int value)
    {
        playerDatas[num - 1].sp += value;
    }
    public CharacterStatas GetChara(int num)
    {
        return playerDatas[num - 1].mychara;
    }
    public void SetChara(int num,CharacterStatas chara)
    {
        playerDatas[num - 1].mychara = chara;
    }
}
