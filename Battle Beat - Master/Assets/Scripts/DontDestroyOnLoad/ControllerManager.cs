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

    [SerializeField]
    private bool forcedSpecificationed;
    [SerializeField]
    private int player1ID;
    [SerializeField]
    private int player2ID;


    const float inputThreshold = 0.9f;
    //------------------------------
    // 初期化
    //------------------------------
    public void Init()
    {
        if (ControllerManager.instance != null) return;
        ControllerManager.instance = this;

        if (this.forcedSpecificationed)
        {
            Player1 = new Controller(this.player1ID);
            Player2 = new Controller(this.player2ID);
        }
    }

    private void LateUpdate()
    {
        Player1.UpdateAxes();
        Player2.UpdateAxes();
    }

    public int GetAxis_Menu(Axis axis)
    {
        int player1Axis = this.Player1.GetAxis(axis);
        int player2Axis = this.Player2.GetAxis(axis);
        if      (player1Axis!=0) return player1Axis;
        else  return player2Axis;
    }

    public bool GetButton_Menu(Button button)
    {
        return this.Player1.GetButton(button) || this.Player2.GetButton(button);
    }

    public bool GetButtonDown_Menu(Button button)
    {
        return this.Player1.GetButtonDown(button) || this.Player2.GetButtonDown(button);
    }

    public bool GetButtonUp_Menu(Button button)
    {
        return this.Player1.GetButtonUp(button) || this.Player2.GetButtonUp(button);
    }

    public bool ChangeControllerData(int num)
    {
        Debug.Log("プレイヤー" + num +"のAボタンを押してください");
        if (Input.anyKeyDown)
        {
            if (Input.GetButtonDown("A_1"))
            {
                if(num == 1)Player1 = new Controller(1);
                else Player2 = new Controller(1);
                return true;
            }
            else if(Input.GetButtonDown("A_2"))
            {
                if (num == 1) Player1 = new Controller(2);
                else Player2 = new Controller(2);
                return true;
            }
        }
        return false;
    }

    private void OnGUI()
    {
        Button p1b = Button.Select;
        Button p2b = Button.Select;
        foreach (Button button in Enum.GetValues(typeof(Button)))
        {
            if (Player1.GetButton(button)) p1b = button;
            if (Player2.GetButton(button)) p2b = button;
        }

        GUI.Label(new Rect(50, 50, 1000, 100), "P1: " + p1b.ToString());
        GUI.Label(new Rect(50, 100, 1000, 100), "P2: " + p2b.ToString());
    }

    //==============================
    // プレイヤーのコントローラーのクラス
    //==============================
    public class Controller
    {
        public int controllerNum { get; private set; }

        private Vector2Int preAxes=new Vector2Int(0,0);

        public void UpdateAxes()
        {
            preAxes = new Vector2Int(GetAxis(Axis.DpadX), GetAxis(Axis.DpadY));
        }
        //これでInput.Axesの名前を取得している
        private string[] axisAxes = {
            "D-padX_",
            "D-padY_"
        };

        private string[] buttonAxes;

        public Controller(int controllerNum)
        {
            this.controllerNum = controllerNum;

#if UNITY_EDITOR && UNITY_STANDALONE
            buttonAxes = new string[]{
                "MacOS_A_",
                "MacOS_B_",
                "MacOS_X_",
                "MacOS_Y_",
                "MacOS_L_",
                "MacOS_R_",
                "MacOS_Select_",
                "MacOS_Start_"
            };
#endif
#if UNITY_EDITOR_OSX && UNITY_STANDALONE_OSX
            buttonAxes = new string[]{
                "MacOS_A_",
                "MacOS_B_",
                "MacOS_X_",
                "MacOS_Y_",
                "MacOS_L_",
                "MacOS_R_",
                "MacOS_Select_",
                "MacOS_Start_"
            };
#endif

            for (int i = 0; i < this.axisAxes.Length; i++) this.axisAxes[i] += this.controllerNum.ToString();
            for (int i = 0; i < this.buttonAxes.Length; i++) this.buttonAxes[i] += this.controllerNum.ToString();
        }

        public int GetAxis(Axis axis)
        {
            var temp = Input.GetAxis(this.axisAxes[(int)axis]);
            int result = 0;
            if (temp > inputThreshold)
            {
                result = 1;
            }
            else if (temp < -inputThreshold)
            {
                result = -1;
            }
            return result;
        }
        public int GetAxisUp(Axis axis) //1フレームずつ呼び出すこと
        {
            var temp = Input.GetAxis(this.axisAxes[(int)axis]);
            int result = 0;
            if (temp > inputThreshold)
            {
                result = 1;
            }
            else if (temp < -inputThreshold)
            {
                result = -1;
            }
            if (result == (axis==Axis.DpadX?preAxes.x:preAxes.y))
            {
                result = 0;
            }
            return result;
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
