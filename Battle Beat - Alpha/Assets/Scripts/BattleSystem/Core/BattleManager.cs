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
                TextDisplayForTest("Time Out");

            }
            if (AttackManager._instance.GetWinner() != 0)
            {
                switch (AttackManager._instance.GetWinner())
                {
                    case 1:
                        //P1Win
                        TextDisplayForTest("p1win");
                        break;
                    case 2:
                        //P2Win
                        TextDisplayForTest("p2win");
                        break;
                    case 3:
                        //DRAW
                        TextDisplayForTest("draw");
                        break;
                }
                rythmManager.StopRythm();
                timeSetter.stop();
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
}
