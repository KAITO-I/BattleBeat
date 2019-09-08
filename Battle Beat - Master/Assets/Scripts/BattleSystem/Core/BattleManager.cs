using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        SoundManager.Instance.PlaySE(SEID.Game_Ready);
        MainGameCamera._instance.GameStart();
        ShowImage._instance.ShowImages(new string[] {  "void", "void", "void", "void" }, 0.8f, 0.0f);
        while (true){
            if (ShowImage._instance.IsEnd())
            {
                if (readyFlag == false)
                {
                    ShowImage._instance.ShowImages(new string[] { "READY" }, 0.8f, 0f);
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
    private void Update()
    {
        if (onGame)
        {
            if (timeSetter.isTimeOut())
            {
                rythmManager.StopRythm();

                ////TimeOutTextDisplay
                //TextDisplayForTest("Time Out");

            }
            if (AttackManager._instance.GetWinner() != 0)
            {
                switch (AttackManager._instance.GetWinner())
                {
                    case 1:
                        //P1Win
                        ShowImage._instance.ShowImages(new string[] { "GAME" });
                        StartCoroutine(WaitAndJumpScene());
                        break;
                    case 2:
                        //P2Win
                        ShowImage._instance.ShowImages(new string[] { "GAME" });
                        StartCoroutine(WaitAndJumpScene());
                        break;
                    case 3:
                        //DRAW
                        ShowImage._instance.ShowImages(new string[] { "Draw" });
                        StartCoroutine(WaitAndJumpScene());
                        break;
                }
            }
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

        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Result);
    }
}
