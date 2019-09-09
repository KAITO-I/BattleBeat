using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    RythmManager rythmManager;

    [SerializeField]
    TimeSetter timeSetter;
    [SerializeField]
    float TotalTime = 60f;

    bool onGame;

    bool readyFlag = false;
    bool readyEndFlag = false;
    static int winPlayerId;
    static int LosePlayerId;
    private void Start()
    {
        onGame = false;
        rythmManager.Init();
        timeSetter.TimeSetUP(TotalTime);

        startGame();
    }
    public void startGame()
    {
        StartCoroutine(startGameLoop());
        //MainGameCamera._instance.GameStart();
        //onGame = true;
        //AttackManager._instance.GetPlayer(1).onGame = true;
        //AttackManager._instance.GetPlayer(2).onGame = true;
        //timeSetter.startTimer();
        //rythmManager.StartRythm();
    }

    IEnumerator startGameLoop()
    {
        SoundManager.Instance.PlayBGM(BGMID.InGame);
        MainGameCamera._instance.GameStart();
        ShowImage._instance.ShowImages(new string[] {  "void", "void", "void", "void" }, 0.8f, 0.0f);
        while (true){
            if (ShowImage._instance.IsEnd())
            {
                if (readyFlag == false)
                {
                    SoundManager.Instance.PlaySE(SEID.Game_Ready);
                    ShowImage._instance.ShowImages(new string[] { "READY" }, 3.2f, 0f);
                    readyFlag = true;
                }
                else{
                    if (readyEndFlag == false)
                    {
                        SoundManager.Instance.PlaySE(SEID.Game_Countdown);
                        ShowImage._instance.ShowImages(new string[] { "3", "2", "1", "GO" }, 0.8f, 0f);
                        readyEndFlag = true;
                    }
                    else
                    {
                        onGame = true;
                        AttackManager._instance.GetPlayer(1).onGame = true ;
                        AttackManager._instance.GetPlayer(2).onGame = true;
                        timeSetter.startTimer();
                        rythmManager.StartRythm();
                        break;
                    }
                }
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private class CountUp
    {
        private int time;
        private int bpm;
    }

    [SerializeField]
    private CountUp[] countUp;
    int index = 0;
    float timer = 0f;
    
    int rltwinner = 0;
    private void Update()
    {
        if (onGame)
        {
            uint winner = 0;
            if (timeSetter.isTimeOut())
            {

                winner = AttackManager._instance.CheckWinnerTimeOut();
            }
            else
            {
                winner = AttackManager._instance.GetWinner();
            }
            if (winner != 0)
            {
                SoundManager.Instance.StopBGM();
                switch (winner)
                {
                    case 1:
                        //P1Win
                        rltwinner = 1;
                        ShowImage._instance.ShowImages(new string[] { "GAME" }, 4f, 0f);
                        StartCoroutine(WaitAndJumpScene());
                        SoundManager.Instance.PlaySE(SEID.Game_Character_General_Finish);
                        break;
                    case 2:
                        //P2Win
                        rltwinner = 2;
                        ShowImage._instance.ShowImages(new string[] { "GAME" }, 4f, 0f);
                        StartCoroutine(WaitAndJumpScene());
                        SoundManager.Instance.PlaySE(SEID.Game_Character_General_Finish);
                        break;
                    case 3:
                        //DRAW
                        rltwinner = 3;
                        ShowImage._instance.ShowImages(new string[] { "Draw" }, 4f, 0f);
                        StartCoroutine(WaitAndJumpScene());
                        SoundManager.Instance.PlaySE(SEID.General_Siren);
                        break;
                }

                //playerの動きを停止させる
                StopPlayer();
                onGame = false;
            }
        }
    }

    private static void StopPlayer()
    {
        Behaviour[] pauseBehavs = null;
        GameObject p1g = AttackManager._instance.GetPlayer(1).gameObject;
        GameObject p2g = AttackManager._instance.GetPlayer(2).gameObject;
        pauseBehavs = Array.FindAll(p1g.GetComponentsInChildren<Behaviour>(), (obj) =>
        {
            if (obj == null)
            {
                return false;
            }
            return obj.enabled;
        });

        foreach (var com in pauseBehavs)
        {
            com.enabled = false;
        }
        pauseBehavs = Array.FindAll(p2g.GetComponentsInChildren<Behaviour>(), (obj) =>
        {
            if (obj == null)
            {
                return false;
            }
            return obj.enabled;
        });

        foreach (var com in pauseBehavs)
        {
            com.enabled = false;
        }
    }

    //Debug
    [SerializeField]
    GameObject textObj;
    void TextDisplayForTest(string str)
    {
        textObj.SetActive(true);
        textObj.GetComponent<Text>().text = str;
    }
    IEnumerator WaitAndJumpScene()
    {
        rythmManager.StopRythm();
        timeSetter.stop();
        if (rltwinner != 3) {
            MainGameCamera._instance.ChangeAndZoomUp(3 - rltwinner);
        }
        while (true)
        {
            if (ShowImage._instance.IsEnd())
            {
                break;
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
        if (AttackManager._instance.GetWinner() == 3)
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
        }
        else
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Result);
        }
    }
}
