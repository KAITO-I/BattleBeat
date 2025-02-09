﻿//==============================
// Created by akiirohappa.
// Customized by KAITO-I and SA371516
//==============================
using UnityEngine;

//==============================
// プレイヤーのコントローラーを管理するクラス
//==============================
public class ControllerManager : MonoBehaviour
{
    //==============================
    // static class
    //==============================
    private static ControllerManager instance;
    public static ControllerManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("ControllerManagerが読み込まれていません");
            return instance;
        }
    }

    //==============================
    // コントローラー入力リスト
    //==============================
    public enum Axis
    {
        DpadX,
        DpadY,
    }

    public enum Button
    {
        A,
        B,
        X,
        Y,
        L,
        R,
        Select,
        Start
    }

    //==============================
    // class
    //==============================
    public static PlayerController Player1 { get; private set; }
    public static PlayerController Player2 { get; private set; }

    PlayerController SelectPlayer;

    //これでInput.Axesの名前を取得している
    private string[] axisAxes = {
        "D-padX_",
        "D-padY_"
    };

    private string[] buttonAxes = {
        "A_",
        "B_",
        "X_",
        "Y_",
        "L_",
        "R_",
        "Select_",
        "Start_"
    };

    private void Update()
    {
        //入力されたものが何なのか
        /*foreach (Controller code in Enum.GetValues(typeof(Controller)))
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
        }*/
    }

    //コントローラー操作
    /*void InputCheck(Controller key,PlayerController player)
    {
        switch (player.controllerNum)
        {
            case 1:
                switch (key)
                {
                    case Controller.A:
                        Debug.Log("A");
                        break;
                    case Controller.B:
                        Debug.Log("B");
                        break;
                    case Controller.DpadX:
                        float h = Input.GetAxisRaw(player.axes[(int)key]);
                        if(h>0) Debug.Log("←");
                        else Debug.Log("→");
                        break;
                    case Controller.DpadY:
                        float v = Input.GetAxisRaw(player.axes[(int)key]);
                        if (v > 0) Debug.Log("↑");
                        else Debug.Log("↓");
                        break;
                    case Controller.X:
                        Debug.Log("X");
                        break;
                    case Controller.Y:
                        Debug.Log("Y");
                        break;
                }
                break;
            case 2:
                switch (key)
                {
                    case Controller.A:
                        Debug.Log("A");
                        break;
                    case Controller.B:
                        Debug.Log("B");
                        break;
                    case Controller.DpadX:
                        float h = Input.GetAxisRaw(player.axes[(int)key]);
                        if (h > 0) Debug.Log("←");
                        else Debug.Log("→");
                        break;
                    case Controller.DpadY:
                        float v = Input.GetAxisRaw(player.axes[(int)key]);
                        if (v > 0) Debug.Log("↑");
                        else Debug.Log("↓");
                        break;
                    case Controller.X:
                        Debug.Log("X");
                        break;
                    case Controller.Y:
                        Debug.Log("Y");
                        break;
                }
                break;
        }
    }*/

    //Playerの紐づけ
    void ControllerChange()
    {
        switch (Input.GetKeyDown("A_1P"))
        {
            case true:
                Player1 = new PlayerController(1, axisAxes, buttonAxes);
                Player2 = new PlayerController(2, axisAxes, buttonAxes);
                break;

            case false:
                Player1 = new PlayerController(2, axisAxes, buttonAxes);
                Player2 = new PlayerController(1, axisAxes, buttonAxes);
                break;
        }
    }

    //==============================
    // プレイヤーのコントローラーのクラス
    //==============================
    public class PlayerController
    {
        public int controllerNum { get; private set; }
        private string[] axisAxes;
        private string[] buttonAxes;

        public PlayerController(int controllerNum, string[] axisAxes, string[] buttonAxes)
        {
            this.controllerNum = controllerNum;

            this.axisAxes   = axisAxes;
            this.buttonAxes = buttonAxes;

            for (int i = 0; i < this.axisAxes.Length; i++) this.axisAxes[i] += this.controllerNum + "P";
            for (int i = 0; i < this.buttonAxes.Length; i++) this.buttonAxes[i] += this.controllerNum + "P";
        }

        public float GetAxis(Axis axis)
        {
            return Input.GetAxis(this.axisAxes[(int)axis]);
        }

        public bool GetButton(Button button)
        {
            return Input.GetButton(this.buttonAxes[(int)button]);
        }

        public bool GetButtonDown(Button button)
        {
            return Input.GetButtonDown(this.buttonAxes[(int)button]);
        }

        public bool GetButtonUp(Button button)
        {
            return Input.GetButtonUp(this.buttonAxes[(int)button]);
        }

        public bool AnyButtonDown()
        {
            for (int i = 0; i < this.buttonAxes.Length; i++)
                if (Input.GetButtonDown(this.buttonAxes[i])) return true;
            return false;
        }
    }
}
