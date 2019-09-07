using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;
    public delegate void commandFunction(int playerId);
    Dictionary<string, commandFunction> commandList = new Dictionary<string, commandFunction>();

    string[] playerInputLog = new string[2] { string.Empty,string.Empty};

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        bool[] playerInputFlag = { false, false };
        for (int i = 0; i < 2; i++)
        {
            foreach (ControllerManager.Button button in Enum.GetValues(typeof(ControllerManager.Button)))
            {
                if (playerInputFlag[i]) { break; }
                if (ControllerManager.Instance.Player1.GetButtonDown(button))
                {
                    playerInputFlag[i] = true;
                    playerInputLog[i] += button.ToString();
                    if (playerInputLog[i].Length > 150)
                    {
                        playerInputLog[i] = playerInputLog[i].Substring(playerInputLog[i].Length-12);
                    }
                }
            }



            foreach (var v in commandList.Keys)
            {
                bool rlt = Regex.IsMatch(playerInputLog[i], v);
                if (rlt)
                {
                    if (commandList[v] != null)
                    {
                        commandList[v](i);
                        commandList.Remove(v);
                    }
                }
            }
        }
    }
    public void registCommand(string command, commandFunction commandFunction)
    {
        commandList.Add(command, commandFunction);
    }
}
