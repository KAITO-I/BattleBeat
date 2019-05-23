//======================================//
/*******作成者:金川　2019-05-23**********/
//======================================//
/*ノーツの表示 */
//======================================//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesManager : MonoBehaviour
{
    //ノーツの移動の向き
    [SerializeField]
    bool fromRightToLeft;
    int moveFactor;

    //次の判定タイミング来るまでの秒数
    [SerializeField]
    public float nextDuration;
    //ノートの個数
    [SerializeField]
    int totalNotes;
    //表示させたいノートのプリハブ
    [SerializeField]
    GameObject notePrefab;
    //instance化されたnotesのGameObject
    List<GameObject> notes;
    //表示させたいゴールのプリハブ
    [SerializeField]
    GameObject goalPrefab;
    //instance化されたgoalのGameObject
    GameObject goal;
    //ノートの親のpanel
    [SerializeField]
    GameObject panel;

    float intervalSize;

    //テスト用
    float Timer;
    //初期化
    public void Init() {
        //表示領域の大きさ
        var panelRectT = panel.GetComponent<RectTransform>();
        var Width = panelRectT.rect.width;
        var Height = panelRectT.rect.height;

        //ノートとゴールのinstance化の保存領域を確保する
        if (notes.Count > 0)
        {
            foreach(var GO in notes)
            {
                if (GO != null)
                {
                    Destroy(GO);
                }
            }
            notes.Clear();
        }

        if (goal != null)
        {
            Destroy(goal);
            goal = null;
        }

        //ノートとゴールのinstance化
        intervalSize = Width / (float) totalNotes;
        
        for(int i = 0; i < totalNotes - 1; i++)
        {
            var GO = Instantiate<GameObject>(notePrefab, panel.transform);
            var goRectT = GO.GetComponent<RectTransform>();
            if (fromRightToLeft) {
                goRectT.anchorMax = new Vector2(0f, 0.5f);
                goRectT.anchorMin = new Vector2(0f, 0.5f);
            }
            else
            {
                goRectT.anchorMax = new Vector2(1f, 0.5f);
                goRectT.anchorMin = new Vector2(1f, 0.5f);
                
            }
            goRectT.anchoredPosition = new Vector2((int)((i + 1) * intervalSize) * -1 * moveFactor ,0);
            notes.Add(GO);
        }
        var goalGO = Instantiate<GameObject>(goalPrefab, panel.transform);
        var goalGoRectT = goalGO.GetComponent<RectTransform>();
        if (fromRightToLeft)
        {
            goalGoRectT.anchorMax = new Vector2(0f, 0.5f);
            goalGoRectT.anchorMin = new Vector2(0f, 0.5f);
        }
        else
        {
            goalGoRectT.anchorMax = new Vector2(1f, 0.5f);
            goalGoRectT.anchorMin = new Vector2(1f, 0.5f);
        }
        goalGoRectT.anchoredPosition = Vector2.zero;
        goal = goalGO;
    }
    //ノートを移動させる
    public void MoveNotes(float timeOffset)
    {
        int i = 0;
        foreach(var GO in notes)
        {
            var goRectT = GO.GetComponent<RectTransform>();
            var pos = new Vector2((int)((i + 1) * intervalSize) * -1 * moveFactor, 0);
            pos.x = pos.x + moveFactor * Mathf.Lerp(0, intervalSize, timeOffset / nextDuration);
            goRectT.anchoredPosition = pos;
            i++;
        }
    }
    //rhythmManagerと同期する
    public void Synchronism2()
    {
        //DoSomeThing
    }
    private void Awake()
    {
        goal = null;
        notes = new List<GameObject>();
        if (fromRightToLeft)
        {
            moveFactor = -1;
        }
        else
        {
            moveFactor = 1;
        }
    }
    //テスト用
    private void Update()
    {
        Timer += Time.deltaTime;
        if (nextDuration <= 0)
        {
            throw new System.Exception("nextDuration <= 0");
        }
        while (Timer > nextDuration)
        {
            Timer -= nextDuration;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Init();
            Timer = 0;
        }
        MoveNotes(Timer);
    }
}