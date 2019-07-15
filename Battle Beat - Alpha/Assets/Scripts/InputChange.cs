//==============================
// Created by akiirohappa
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==============================
// 入力切替用
//==============================
public class InputChange : MonoBehaviour
{
    public Text uitex;
    PlayerNum pn;
    public PlayerControllerr[] players; 
    // Start is called before the first frame update
    void Start()
    {
        pn = PlayerNum.oneP;
    }

    // Update is called once per frame
    void Update()
    {
        switch (pn)
        {
            case PlayerNum.oneP:
                if (Input.GetButtonDown("LB_1P") && Input.GetButtonDown("RB_1P"))
                {
                    Debug.Log("SetPlayer 1 1");
                    pn = PlayerNum.twoP;
                }
                if (Input.GetButtonDown("LB_2P") && Input.GetButtonDown("RB_2P"))
                {
                    Debug.Log("SetPlayer 1 2");
                    players[0].myPl = PlayerNum.twoP;
                    pn = PlayerNum.twoP;
                }
                break;
            case PlayerNum.twoP:
                if (Input.GetButtonDown("LB_2P") && Input.GetButtonDown("RB_2P"))
                {
                    Debug.Log("SetPlayer 2 2");
                }
                if (Input.GetButtonDown("LB_1P") && Input.GetButtonDown("RB_1P"))
                {
                    Debug.Log("SetPlayer 2 1");
                    players[1].myPl = PlayerNum.oneP;
                }
                break;
        }

    }

}
