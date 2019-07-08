using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AttackManager : MonoBehaviour
{
    static public AttackManager _instance;
    //攻撃オブジェクトが保存されるリスト
    [SerializeField]
    List<AttackItemBase> attackItems;
    //プレイヤーが保存されるリスト
    [SerializeField]
    List<Player> players;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    //プレイヤーが攻撃する際に生成された攻撃オブジェクトをリストに記載する
    public void Add(AttackItemBase attackItem)
    {
        attackItems.Add(attackItem);
    }
    //ターン処理
    public void NextTurn()
    {
        foreach (var p in players)
        {
            p.Turn_AttackPhase();
        }
        foreach (var item in attackItems)
        {
            //攻撃オブジェクトのターン処理 => 第一
            item.TurnProcessPhase0();
        }
        foreach (var p in players)
        {
            p.Turn_MovePhase();
        }
        
        foreach (var item in attackItems)
        {
      　　　//攻撃オブジェクトのターン処理 => 第一
            item.TurnProcessPhase1();
            //攻撃オブジェクトがダメージ判定が発生するか
            if (item.CheckDamage())
            {
                //ダメージ判定が発生する際に、特定のプレイヤーが判定エリア内にいるか
                foreach (var player in players)
                {
                    if (item.CheckArea(player.Pos, player.PlayerID))
                    {
                        //ダメージ発生
                        item.PassDamage(player);
                    }
                }
            }
        }
        foreach (var item in attackItems)
        {
            //攻撃オブジェクトのターン処理 => 第二
            item.TurnProcessPhase2();
            
        }
        //攻撃オブジェクトが廃棄するべきか
        foreach (var item in attackItems)
        {
            if (item.isEnd())
            {
                Destroy(item.gameObject);
            }
        }
        attackItems.RemoveAll(AttackItemIsEnd);
    }

    private void Update()
    {
        //test
        if (Input.GetKeyUp(KeyCode.Return))
        {
            NextTurn();
        }
    }
    static bool AttackItemIsEnd(AttackItemBase attackItem)
    {
        return attackItem.isEnd();
    }
    public Player GetPlayer(int pId)
    {
        return players.Find(p => p.PlayerID == pId);
    }
}
