using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    static PlayerController player1;
    static PlayerController player2;

    public enum Input
    {
        CrossKeyH,
        CrossKeyV,
        A,
        B,
        X,
        Y
    }

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
        
    }

    class PlayerController
    {
        private int controllerNum;
        private string[] axes;

        PlayerController(int controllerNum, string[] axes)
        {
            this.controllerNum = controllerNum;

            this.axes = axes;
            for (int i = 0; i < this.axes.Length; i++) this.axes[i] += this.controllerNum + "P";
        }
    }
}
