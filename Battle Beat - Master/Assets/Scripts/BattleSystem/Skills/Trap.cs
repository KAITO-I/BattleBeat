﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BasicAttack 
{
    bool DamagePassed;
    public int LiftTime;
    //=====アニメーション=======//
    UniAnimation _uniAnim ;
    public override void PassDamage(Player player)
    {
        base.PassDamage(player);
        DamagePassed = true;
        //======アニメーション処理==========//
        _uniAnim.UniAnim(UniAnimation.UniState.Attack);
    }
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        Vector3 vec = transform.parent.transform.position;                              //アニメーションに必要
        transform.parent = null;
        if (!reverse)
        {
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(col+ Area[0].x, row));
        }
        else
        {
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(col- Area[0].x, row));
        }
        transform.position += new Vector3(0, 1f, 0);
        DamagePassed = false;

        canMakeDamage = true;//baseを上書きする
        //==========アニメーション処理==========//
        //トラップのある位置にプレイヤーがいた場合にバグ発生
        GameObject _uniObj = transform.GetChild(0).gameObject;
        _uniObj.transform.position = vec;//ユニゾンの位置にする
         _uniAnim = _uniObj.GetComponent<UniAnimation>();
        if (!reverse)
        {
            _uniAnim.UniAnim(UniAnimation.UniState.Start, _uniObj, BoardManager._instance.ToWorldPos(new Vector2Int(col + Area[0].x, row)),reverse);
        }
        else
        {
            _uniAnim.UniAnim(UniAnimation.UniState.Start, _uniObj, BoardManager._instance.ToWorldPos(new Vector2Int(col - Area[0].x, row)),reverse);
        }
        _uniObj.transform.parent = null;
    }
    public override bool isEnd()
    {
        if (NowTurn > LiftTime)//罠にかからなかった場合
        {
            _uniAnim.UniAnim(UniAnimation.UniState.Back);
            return true; 
        }
        return DamagePassed;
    }
    public override void TurnPreprocess()
    {
        NowTurn++;
    }
    public override void TurnProcessPhase1_Main()
    {
    }
}
