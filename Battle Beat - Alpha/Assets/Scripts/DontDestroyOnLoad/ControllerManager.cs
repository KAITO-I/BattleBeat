//==============================
// Created by akiirohappa.
// Customized by KAITO-I and SA371516
//==============================
using UnityEngine;
using System;

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
    public Controller Player1 { get; private set; }
    public Controller Player2 { get; private set; }

    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (ControllerManager.instance != null) return;
        ControllerManager.instance = this;
    }

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
        if (!himoduke)
        {
            ControllerChange();
            return;
        }

        if (Input.anyKeyDown)
        {
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                if (Player1 != null && !Mathf.Approximately(Player1.GetAxis(axis), 0f)) Debug.Log("P1 " + axis.ToString() + ":" + Player1.GetAxis(axis));
                if (Player2 != null && !Mathf.Approximately(Player2.GetAxis(axis), 0f)) Debug.Log("P2 " + axis.ToString() + ":" + Player2.GetAxis(axis));
            }

            foreach (Button button in Enum.GetValues(typeof(Button)))
            {
                if (Player1 != null && Player1.GetButton(button) != false) Debug.Log("P1 " + button.ToString() + ":" + Player1.GetButton(button));
                if (Player2 != null && Player2.GetButton(button) != false) Debug.Log("P2 " + button.ToString() + ":" + Player2.GetButton(button));
            }
        }
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
    bool himoduke = false;
    void ControllerChange()
    {
        if (Input.anyKeyDown)
        {
            switch (Input.GetButtonDown("A_1"))
            {
                case true:
                    Player1 = new Controller(1);
                    //Player2 = new Controller(2);
                    himoduke = true;
                    break;

                case false:
                    //Player1 = new Controller(2);
                    Player2 = new Controller(1);
                    himoduke = true;
                    break;
            }
            Debug.Log("ControllerChange");
        }
    }

    public float GetAxis(Axis axis)
    {
        float player1Axis = this.Player1.GetAxis(axis);
        float player2Axis = this.Player2.GetAxis(axis);
        return player1Axis > player2Axis ? player1Axis : player2Axis;
    }

    public bool GetButton(Button button)
    {
        return this.Player1.GetButton(button) || this.Player2.GetButton(button);
    }

    public bool GetButtonDown(Button button)
    {
        return this.Player1.GetButtonDown(button) || this.Player2.GetButtonDown(button);
    }

    public bool GetButtonUp(Button button)
    {
        return this.Player1.GetButtonUp(button) || this.Player2.GetButtonUp(button);
    }

    //==============================
    // プレイヤーのコントローラーのクラス
    //==============================
    public class Controller
    {
        public int controllerNum { get; private set; }

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

        public Controller(int controllerNum)
        {
            this.controllerNum = controllerNum;

            for (int i = 0; i < this.axisAxes.Length; i++) this.axisAxes[i] += this.controllerNum.ToString();
            for (int i = 0; i < this.buttonAxes.Length; i++) this.buttonAxes[i] += this.controllerNum.ToString();
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
    }
}
