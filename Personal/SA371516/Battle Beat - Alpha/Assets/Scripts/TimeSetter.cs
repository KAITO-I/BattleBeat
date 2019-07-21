//---------------------------------------------------
//作成者・木原　時間信仰
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

    private void Start()
    {
        timeFlag = true;
    }
    private void FixedUpdate()
    {
        if (!timeFlag) return;
        time -= Time.deltaTime;
        TimeShow();
        if(time < 0f)
        {
            timeFlag = false;
            Debug.Log("戦闘終了");
        }
    }
    public void TimeSetUP(float f)
    {
        time = f;
    }
    void TimeShow()
    {
        showText.text = time.ToString("f0");
    }
}
