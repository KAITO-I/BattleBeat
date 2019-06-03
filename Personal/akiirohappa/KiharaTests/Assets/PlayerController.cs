//------------------------------------------------
//作成者・木原　コントローラー操作
//------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNum
{
    oneP,
    twoP,
}
public class PlayerController : MonoBehaviour
{
    //Numで実際のプレイヤー番号。myplでコントローラーの番号
    [SerializeField]PlayerNum Num;
    public PlayerNum myPl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetButtonDown("A_1P") && myPl == PlayerNum.oneP)
        {
            Debug.Log("plessd" + ((Num == PlayerNum.oneP) ? "1P" : "2P"));
        }
        if (Input.GetButtonDown("A_2P") && myPl == PlayerNum.twoP)
        {
            Debug.Log("plessd" + ((Num == PlayerNum.oneP) ? "1P" : "2P"));
        }
        */
        InputCheck();

    }

    //ボタン入力用。switchの下の対応するボタンのif文の下に使いたい関数を入れてね。
    //oneP、twoPどっちにも入れといて。
    //プレイヤー番号が欲しいならPlayerNumを引数にするかGetPlayerNumを使っとくれ
    void InputCheck()
    {
        switch (myPl)
        {
            case PlayerNum.oneP:
                //入力1番コントローラーver
                #region
                if (Input.GetAxisRaw("Horizontal_1P") > 0 || Input.GetAxisRaw("HorizontalAlt_1P") > 0)
                {
                    Debug.Log("←");
                }
                else if (Input.GetAxisRaw("Horizontal_1P") < 0 || Input.GetAxisRaw("HorizontalAlt_1P") < 0)
                {
                    Debug.Log("→");
                }
                else if (Input.GetAxisRaw("Vertical_1P") > 0 || Input.GetAxisRaw("VerticalAlt_1P") > 0)
                {
                    Debug.Log("↑");
                }
                else if (Input.GetAxisRaw("Vertical_1P") < 0 || Input.GetAxisRaw("VerticalAlt_1P") < 0)
                {
                    Debug.Log("↓");
                }
                else if (Input.GetButtonDown("A_1P"))
                {
                    Debug.Log("A");
                }
                else if (Input.GetButtonDown("B_1P"))
                {
                    Debug.Log("B");
                }
                else if (Input.GetButtonDown("X_1P"))
                {
                    Debug.Log("X");
                }
                else if (Input.GetButtonDown("Y_1P"))
                {
                    Debug.Log("Y");
                }
                #endregion
                break;
            case PlayerNum.twoP:
                //入力2番コントローラーver
                #region
                if (Input.GetAxisRaw("Horizontal_2P") > 0 || Input.GetAxisRaw("HorizontalAlt_2P") > 0)
                {
                    Debug.Log("←");
                }
                else if (Input.GetAxisRaw("Horizontal_2P") < 0 || Input.GetAxisRaw("HorizontalAlt_2P") < 0)
                {
                    Debug.Log("→");
                }
                else if (Input.GetAxisRaw("Vertical_2P") > 0 || Input.GetAxisRaw("VerticalAlt_2P") > 0)
                {
                    Debug.Log("↑");
                }
                else if (Input.GetAxisRaw("Vertical_2P") < 0 || Input.GetAxisRaw("VerticalAlt_2P") < 0)
                {
                    Debug.Log("↓");
                }
                else if (Input.GetButton("A_2P"))
                {
                    Debug.Log("A");
                }
                else if (Input.GetButton("B_2P"))
                {
                    Debug.Log("B");
                }
                else if (Input.GetButton("X_2P"))
                {
                    Debug.Log("X");
                }
                else if (Input.GetButton("Y_2P"))
                {
                    Debug.Log("Y");
                }
                #endregion
                break;
        }
    }

    //Numを返す
    public PlayerNum GetPlayerNum()
    {
        return Num;
    }
    public void SetPlayerNum(PlayerNum pn)
    {
        myPl = pn;
    }
}
