using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ControllerManager : MonoBehaviour
{
    public static PlayerController Player1 { get; private set; }
    public static PlayerController Player2;
    PlayerController SelectPlayer;
    int SelectID=1;

    public enum InputKey
    {
        CrossKeyH,
        CrossKeyV,
        A,
        B,
        X,
        Y
    }

    //これでInput.Axesの名前を取得している
    private string[] axes = {
        "HorizontalAlt_",
        "VerticalAlt_",
        "A_",
        "B_",
        "X_",
        "Y_"
    };

    private void Update()
    {
        //入力されたものが何なのか
        foreach (InputKey code in Enum.GetValues(typeof(InputKey)))
        {
            if (Input.GetButtonDown(axes[(int)code]+"1P"))
            {
                //処理を書く
                Debug.Log(code);
                break;
            }
            else if (Input.GetButtonDown(axes[(int)code] + "2P"))
            {
                //処理を書く
                Debug.Log(code);
                break;
            }
        }
    }

    //コントローラー操作
    void InputCheck(InputKey key,PlayerController player)
    {
        switch (player.controllerNum)
        {
            case 1:
                switch (key)
                {
                    case InputKey.A:
                        Debug.Log("A");
                        break;
                    case InputKey.B:
                        Debug.Log("B");
                        break;
                    case InputKey.CrossKeyH:
                        float h = Input.GetAxisRaw(player.axes[(int)key]);
                        if(h>0) Debug.Log("←");
                        else Debug.Log("→");
                        break;
                    case InputKey.CrossKeyV:
                        float v = Input.GetAxisRaw(player.axes[(int)key]);
                        if (v > 0) Debug.Log("↑");
                        else Debug.Log("↓");
                        break;
                    case InputKey.X:
                        Debug.Log("X");
                        break;
                    case InputKey.Y:
                        Debug.Log("Y");
                        break;
                }
                break;
            case 2:
                switch (key)
                {
                    case InputKey.A:
                        Debug.Log("A");
                        break;
                    case InputKey.B:
                        Debug.Log("B");
                        break;
                    case InputKey.CrossKeyH:
                        float h = Input.GetAxisRaw(player.axes[(int)key]);
                        if (h > 0) Debug.Log("←");
                        else Debug.Log("→");
                        break;
                    case InputKey.CrossKeyV:
                        float v = Input.GetAxisRaw(player.axes[(int)key]);
                        if (v > 0) Debug.Log("↑");
                        else Debug.Log("↓");
                        break;
                    case InputKey.X:
                        Debug.Log("X");
                        break;
                    case InputKey.Y:
                        Debug.Log("Y");
                        break;
                }
                break;
        }
    }
    //Playerの紐づけ
    void ControllerChange()
    {
        switch (Input.GetKeyDown("A_1P"))
        {
            case true:
                Player1 = new PlayerController(1,axes);
                Player2 = new PlayerController(2, axes);
                break;
            case false:
                Player1 = new PlayerController(2,axes);
                Player2 = new PlayerController(1, axes);
                break;
        }
    }

    public class PlayerController
    {
        public int controllerNum;
        public string[] axes;

        public PlayerController(int controllerNum, string[] axes)
        {
            this.controllerNum = controllerNum;

            this.axes = axes;
            for (int i = 0; i < this.axes.Length; i++) this.axes[i] += this.controllerNum + "P";
        }
    }
}
