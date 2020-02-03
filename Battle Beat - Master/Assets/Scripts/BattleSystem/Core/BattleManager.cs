using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    float lagTime1 = 0.1f;
    [SerializeField]
    float lagTime2 = 0.1f;
    [SerializeField]
    protected RythmManager rythmManager;

    [SerializeField]
    protected TimeSetter timeSetter;
    [SerializeField]
    float TotalTime = 90f;

    protected bool onGame;

    protected uint winner = 0;

    public bool readyFlag = false;
    public bool readyEndFlag = false;
    static int winPlayerId;
    static int LosePlayerId;
    float SceneStartTime;
    protected GameState gameState;
    private void Awake()
    {
        SceneStartTime = Time.time;
    }

    private void Start()
    {
        onGame = false;
        rythmManager.Init();
        timeSetter.TimeSetUP(TotalTime);

        gameState = new CountDownState(this);

    }
    private IEnumerator startGameLoop()
    {
        
        MainGameCamera._instance.GameStart();
        SoundManager.Instance.PlayBGM(BGMID.InGame0);
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
                        SoundManager.Instance.PlaySE(SEID.Game_Countdown_First_Only);
                        ShowImage._instance.ShowImages(new string[] { "3", "2", "1", "GO" }, 0.8f, 0f);
                        readyEndFlag = true;
                    }
                    else
                    {
                        onGame = true;
                        TurnManager._instance.GetPlayer(1).onGame = true ;
                        TurnManager._instance.GetPlayer(2).onGame = true;
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
        onGame = true;
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
        gameState = gameState?.Run();
    }

    private bool CheckGameOver()
    {
        
        if (timeSetter.isTimeOut())
        {

            winner = TurnManager._instance.CheckWinnerTimeOut();
        }
        else
        {
            winner = TurnManager._instance.GetWinner();
        }
        if (winner != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GameOverProcess()
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
        RythmManager.instance.PrintStatistics();
        //playerの動きを停止させる
        StopPlayer();
        onGame = false;
    }

    private static void StopPlayer()
    {
        Behaviour[] pauseBehavs = null;
        GameObject p1g = TurnManager._instance.GetPlayer(1).gameObject;
        GameObject p2g = TurnManager._instance.GetPlayer(2).gameObject;
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

    public virtual void TurnProcess()
    {
        Debug.Log(TurnManager._instance.totalTurn);
        switch (TurnManager._instance.totalTurn)
        {
            case 24+2:

                rythmManager.TempoUp(113);
                

                break;
            case 24 - 4+2:
                
                SoundManager.Instance.PlaySE(SEID.Game_Countdown);
                ShowImage._instance.ShowImages(new string[] { "3", "2", "1", "TempoUp" }, 0.8f, 0.0f);
                break;

            case 88+2:
                rythmManager.TempoUp(150);
                
                break;
            case 88 - 4+2:
                SoundManager.Instance.PlaySE(SEID.Game_Countdown_113);
                ShowImage._instance.ShowImages(new string[] { "3", "2", "1", "TempoUp" }, 60f / 113f, 0.0f);
                break;

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
        if (TurnManager._instance.GetWinner() == 3)
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
        }
        else
        {
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Result);
        }
    }
    protected class GameState
    {
        protected BattleManager manager;
        public GameState(BattleManager manager) { this.manager = manager; }
        public virtual GameState Run() { return null; }
    }
    protected class CountDownState : GameState
    {
        bool isStarted = false;
        public CountDownState(BattleManager manager) : base(manager)
        {

        }
        public override GameState Run()
        {
            if (!isStarted)
            {
                manager.StartCoroutine(manager.startGameLoop());
                isStarted = true;
            }
            if (manager.onGame)
            {
                return new OnGameState(manager);
            }
            else
            {
                return this;
            }
        }
    }
    protected class OnGameState : GameState
    {
        int bgmId = 0;
        public OnGameState(BattleManager manager) : base(manager)
        {

        }
        public override GameState Run()
        {
            if(manager.CheckGameOver())
            {
                return new EndGameState(manager);
            }
            else
            {
                switch (bgmId)
                {
                    case 0:
                        if (Time.time - manager.SceneStartTime > 19.2f + 9.6f-manager.lagTime1)
                        {
                            bgmId = 1;
                            SoundManager.Instance.PlayBGM(BGMID.InGame1);
                        }
                        break;
                    case 1:
                        if (Time.time - manager.SceneStartTime > 90f-36.8f + 9.6f - manager.lagTime2)
                        {
                            bgmId = 2;
                            SoundManager.Instance.PlayBGM(BGMID.InGame2);
                        }
                        break;
                }
                return this;
            }
        }
    }
    protected class EndGameState : GameState
    {
        public EndGameState(BattleManager manager) : base(manager)
        {

        }

        public override GameState Run()
        {
            if (manager.onGame)
            {
                manager.GameOverProcess();
            }
            return null;
        }
    }
}
