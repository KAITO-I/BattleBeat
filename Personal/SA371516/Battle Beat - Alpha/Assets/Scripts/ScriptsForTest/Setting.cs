﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public enum Chara
    {
        HOMI,
        KAGURA,
        ANA,
        YUNIZON
    }
    public static Chara p1c;
    public static Chara p2c;
    public GameObject[] charaPrefabs;

    Player.KeySets p1k;
    Player.KeySets p2k;

    public KeyCode LeftKey;
    public KeyCode RightKey;
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode Attack_1Key;
    public KeyCode Attack_2Key;
    public KeyCode Attack_3Key;
    public KeyCode Attack_4Key;

    public KeyCode LeftKey2;
    public KeyCode RightKey2;
    public KeyCode UpKey2;
    public KeyCode DownKey2;
    public KeyCode Attack_1Key2;
    public KeyCode Attack_2Key2;
    public KeyCode Attack_3Key2;
    public KeyCode Attack_4Key2;


    //debug
    public DebugText dt1;
    public DebugText dt2;
    private void Start()
    {
        p1k = new Player.KeySets(LeftKey, RightKey, UpKey, DownKey, Attack_1Key, Attack_2Key, Attack_3Key, Attack_4Key);
        p2k= new Player.KeySets(LeftKey2, RightKey2, UpKey2, DownKey2, Attack_1Key2, Attack_2Key2, Attack_3Key2, Attack_4Key2);
        GameObject p1g = Instantiate<GameObject>(charaPrefabs[(int)p1c]);
        GameObject p2g = Instantiate<GameObject>(charaPrefabs[(int)p2c]);
        Player p1p = p1g.GetComponent<Player>();
        Player p2p = p2g.GetComponent<Player>();
        p1p.keySets = p1k;
        p2p.keySets = p2k;
        p1p.PlayerID = 1;
        p2p.PlayerID = 2;
        p1p.Pos = new Vector2Int(1, 1);
        p2p.Pos = new Vector2Int(4, 1);
        p1p.Init();
        p2p.Init();
        AttackManager._instance.SetPlayers(p1p, p2p);

        //debug
        dt1.p = p1p;
        dt2.p = p2p;
    }
}
