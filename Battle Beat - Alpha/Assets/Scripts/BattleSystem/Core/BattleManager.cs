using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    RythmManager rythmManager;

    [SerializeField]
    TimeSetter timeSetter;
    [SerializeField]
    float TotalTime = 60f;
    bool onGame;

    private void Start()
    {
        onGame = false;
        rythmManager.Init();
        timeSetter.TimeSetUP(TotalTime);

        startGame();
    }
    public void startGame()
    {
        onGame = true;
        
        timeSetter.startTimer();
        rythmManager.StartRythm();
    }
    private void Update()
    {
        if (onGame)
        {
            if (timeSetter.isTimeOut())
            {
                rythmManager.StopRythm();

                //TimeOutTextDisplay
                TimeOutTextDisplayTest();

            }
            if (AttackManager._instance.GetWinner() != 0)
            {
                switch (AttackManager._instance.GetWinner())
                {
                    case 1:
                        //P1Win
                        print("p1win");
                        break;
                    case 2:
                        //P2Win
                        print("p2win");
                        break;
                    case 3:
                        //DRAW
                        print("draw");
                        break;
                }
                rythmManager.StopRythm();
            }
        }
    }
    //Debug
    [SerializeField]
    GameObject textObj;
    void TimeOutTextDisplayTest()
    {
        textObj.SetActive(true);
    }
}
