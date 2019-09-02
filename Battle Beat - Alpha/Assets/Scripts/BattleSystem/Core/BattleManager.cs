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
        ShowImage._instance.ShowImages(new string[]{"3","2","1","READY","GO" });
        StartCoroutine(startGameLoop());
    }

    IEnumerator startGameLoop()
    {
        while(true){
            if (ShowImage._instance.IsEnd())
            {
                onGame = true;
                timeSetter.startTimer();
                rythmManager.StartRythm();
                break;
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
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
