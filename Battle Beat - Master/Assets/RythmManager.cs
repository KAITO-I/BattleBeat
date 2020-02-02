//Create by Inafuku, Edit by KinSen 2019/07/25
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//==============================
// リズム管理
//==============================
public class RythmManager : MonoBehaviour
{
    public static RythmManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
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

    [SerializeField]
    NotesManager notesManager;
    [SerializeField]
    NotesManager notesManager2;
    public float getbps
    {
        get { return bps; }
    }


    // 命令先
    [SerializeField] private UnityEvent events = new UnityEvent();

    //
    bool Running;

    public  void Init()
    {
        this.bps        = 60f / (float) bpm;
        this.time       += this.bps + 0.00001f;
        this.tempoCount = 0;
        Running = false;

        notesManager.nextDuration = bps;
        notesManager2.nextDuration = bps;
    }
    public void StartRythm()
    {
        Running = true;
        notesManager.Init();
        notesManager2.Init();
    }
    public void StopRythm()
    {
        Running = false;
    }

    private void Update()
    {
        if (Running)
        {
            // リズム
            this.time += Time.deltaTime;
            if (this.time >= this.bps)
            {
                this.time -= this.bps;

                Debug.Log("Rythm");
                events.Invoke();

                //テンポ上昇
                /*this.tempoCount++;
                if (this.tempoCount == this.tempoUpCount)
                {
                    this.tempoCount = 0;
                    this.bpm += tempoUpValue;
                    this.bps = 60f / (float)bpm;
                    if (this.bpm >= this.maxBpm) this.tempoUpCount = -1;
                }*/
            }
            notesManager.MoveNotes(time+0.1f);
            notesManager2.MoveNotes(time + 0.1f);
        }
    }

    public void TempoUp(int tempo)
    {
        this.bpm = tempo;
        this.bps = 60 / (float)bpm;
        notesManager.nextDuration = bps;
        notesManager2.nextDuration = bps;
    }

    public bool IsTiming()
    {
        //return true;
        return (this.time > this.bps - 0.32f);
    }
}