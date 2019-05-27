using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//==============================
// リズム管理
//==============================
public class RythmManager : MonoBehaviour
{
    // 設定
    [SerializeField] private float bpm = 100f;
    public float BPM { get { return this.bpm; } }

    // 測定
    private float bps;
    private float time;

    // 命令先
    [SerializeField] private UnityEvent events = new UnityEvent();

    private void Start()
    {
        this.bps = 60f / bpm;
        this.time = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Execute()
    {

    }

    private void InvokeEvent()
    {
        events.Invoke();
    }
}