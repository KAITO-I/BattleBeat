﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    static public TurnManager _instance;
    //攻撃オブジェクトが保存されるリスト
    [SerializeField]
    List<AttackItemBase> attackItems;
    //プレイヤーが保存されるリスト
    [SerializeField]
    List<Player> players;

    [SerializeField]
    BattleManager battleManager;

    [SerializeField]
    List<GameObject> stages;
    BaseEffect baseEffect = new BaseEffect();
    public RythmManager rythmManager;
    public static uint winner;

    public int totalTurn=0;

    public void SetPlayers(Player p1,Player p2) {
        if (players == null) {
            players = new List<Player>();
        }
        if (players != null) {
            players.Clear();
            players.Add(p1);
            players.Add(p2);
        }
    }
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
    //
    public void AddEffect(Vector3 Pos, bool Reverse, BaseEffect.Effect effect,Quaternion rotation, bool loop = false, float scale = 2f, float speed = 7f)
    {
        Quaternion rotation0 = Reverse ? players[1].transform.rotation : players[0].transform.rotation;
        rotation0 *= rotation;
        baseEffect.NewAndPlay(Pos, rotation0, effect, loop, scale, speed);
    }
    //ターン処理
    public void NextTurn()
    {
        //ステージエフェクト
        foreach(var obj in stages)
        {
            baseEffect.NewAndPlay(obj, BaseEffect.Effect.MUSICWAVE,false,40f,1f);
        }
        baseEffect.CheckAndDestroy();

        //ターン始まる時の処理(playerやattackitemなどのオブジェクトのカウンターなどの処理をする)
        foreach (var item in attackItems)
        {
            item.TurnPreprocess();
        }
        foreach (var p in players)
        {
            p.TurnPreprocess();
        }

        //（A）このプレイヤー（ユーザー）の指令を処理する
        //A：その一（攻撃指示する場合）：攻撃itemを生成
        foreach (var p in players)
        {
            p.Turn_AttackPhase();
        }
        //（B）移動する前に判定する攻撃
        //B：二人のプレイヤーが優先順位無し、同時に処理するために攻撃の予測をする
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase0_Prediction_Request();
        }
        //B：前記の予測を基づいて、疑似の同時処理をする。以下に似たような処理もいくつかある。
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase0_Main();
        }
        //A：その二（移動指示する場合）：移動させる
        foreach (var p in players)
        {
            p.Turn_MovePhase();
        }
        //ProcessPhase1=>DamagePhase=>ProcessPhase2の順でターンの処理をする
        //基本処理
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase1_Prediction_Request();

        }
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase1_Main();

        }
        //攻撃オブジェクトのダメージ判定
        foreach (var item in attackItems)
        {
            item.DamegePhase();

        }
        //ダメージ後処理
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase2_Prediction_Request();

        }
        foreach (var item in attackItems)
        {
            item.TurnProcessPhase2_Main();

        }
        //ターンおわる時の処理
        foreach (var item in attackItems)
        {
            item.TurnPostprocess();
        }
        foreach (var p in players)
        {
            p.TurnPostprocess();
        }
        //勝敗判定
        CheckWinner();

        //攻撃オブジェクトが廃棄するべきか
        foreach (var item in attackItems)
        {
            if (item.isEnd())
            {
                Destroy(item.gameObject);
            }
        }
        attackItems.RemoveAll(AttackItemIsEnd);

        battleManager.TurnProcess();

        totalTurn++;

        
    }

    private void CheckWinner()
    {
        winner = 0;
        foreach (var p in players)
        {
            winner = winner << 1;
            if (p.GetHp() == 0)
            {
                winner += 1;
            }
        }
    }
    public uint CheckWinnerTimeOut()
    {
        uint rlt = 0;
        var hp1 = players[0].GetHp();
        var hp2 = players[1].GetHp();
        if (hp1 > hp2)
        {
            rlt = 1;
        }
        else if (hp1 < hp2)
        {
            rlt = 2;
        }
        else
        {
            rlt = 3;
        }
        winner = rlt;
        return rlt;
    }
    public uint GetWinner()
    {
        return winner;
    }
    private void Update()
    {
        ////test
        //if (Input.GetKeyUp(KeyCode.Return))
        //{
        //    NextTurn();
        //}
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
