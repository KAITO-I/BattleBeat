//---------------------------------------------------
//作成者・木原　時間信仰 編集者 金川
//---------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSetter : MonoBehaviour
{
    [SerializeField]float time = 0f;
    [SerializeField] Text showText;
    [SerializeField] bool timeFlag;

    public bool isTimeOut() { return !timeFlag; }

    private void Awake()
    {
        timeFlag = false;
    }
    private void FixedUpdate()
    {
        if (!timeFlag) return;
        time -= Time.deltaTime;
        TimeShow();
        if(time < 0f)
        {
            timeFlag = false;
        }
    }
    public void TimeSetUP(float f)
    {
        time = f;
    }
    public void startTimer()
    {
        timeFlag = true;
    }
    void TimeShow()
    {
        showText.text = time.ToString("f0");
    }

    public void stop()
    {
        timeFlag = false;
    } 
}
