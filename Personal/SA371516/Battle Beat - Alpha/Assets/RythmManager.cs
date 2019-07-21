using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//==============================
// リズム管理
//==============================
public class RythmManager : MonoBehaviour
{
    // 速度設定
    [SerializeField] private int bpm    = 100;
    [SerializeField] private int maxBpm = 150; // 最高BPM
    //public float BPM { get { return this.bpm; } }

    // 速度上昇設定
    [SerializeField] private int tempoUpValue = 10;  // テンポ上昇値
    [SerializeField] private int tempoUpCount = 10;  // テンポ上昇までのカウント

    // 測定
    private float bps;
    private float time;
    private int   tempoCount;

    // 命令先
    [SerializeField] private UnityEvent events = new UnityEvent();

    private void Start()
    {
        this.bps        = 60f / (float) bpm;
        this.time       += this.bps + 0.00001f;
        this.tempoCount = 0;
    }

    private void Update()
    {
        // リズム
        this.time += Time.deltaTime;
        if (this.time >= this.bps)
        {
            this.time -= this.bps;

            Debug.Log("Rythm");
            events.Invoke();

            //テンポ上昇
            this.tempoCount++;
            if (this.tempoCount == this.tempoUpCount)
            {
                this.tempoCount = 0;
                this.bpm += tempoUpValue;
                this.bps = 60f / (float) bpm;
                if (this.bpm >= this.maxBpm) this.tempoUpCount = -1;
            }
        }
    }
}