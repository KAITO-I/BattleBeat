using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager _instance;
    int[,] Stage_Pos = {
            {0,1,2,3,4,},
            { 5,6,7,8,9,},
            {10,11,12,13,14,},
            {15,16,17,18,19,},
            { 20,21,22,23,24,},
    };
    //ステージのステータス：0→踏めない:1→踏める:2→罠
    int[,] Stage_Pos_Sta = {
            { 0, 0, 0, 0, 0, },
            { 0,1, 1, 1, 0, },
            { 0,1, 1, 1, 0, },
            { 0,1, 1, 1, 0, },
            { 0, 0, 0, 0, 0 },
    };
    //ステージのステータス：0→踏めない:1→踏める:2→罠 Player2用
    int[,] Stage_Pos_Sta2 = {
            { 0, 0, 0, 0, 0, },
            { 0,1, 1, 1, 0, },
            { 0,1, 1, 1, 0, },
            { 0,1, 1, 1, 0, },
            { 0, 0, 0, 0, 0 },
    };
    //移動時に必要
    List<Vector3> Pos = new List<Vector3>();
    List<Vector3> Pos2 = new List<Vector3>();
    [SerializeField]
    //プレイヤーごとに違う
    GameObject Player_Plane;
    [SerializeField]
    //プレイヤーごとに違う
    GameObject Player_Plane2;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //リストの中にステージのポジションを入力
        foreach (Transform tra in Player_Plane.transform)
        {
            Pos.Add(tra.position);
        }
        foreach (Transform tra in Player_Plane2.transform)
        {
            Pos2.Add(tra.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Is_In_Stage(x座標,y座標,何Playerなのか)
    public bool Is_In_Stage(int x, int y,int pID)
    {
        try
        {
            switch (pID)
            {
                case 1:
                    if (Stage_Pos_Sta[y, x] == 0)
                    {
                        return false;
                    }
                    else return true;
                case 2:
                    if (Stage_Pos_Sta2[y, x] == 0)
                    {
                        return false;
                    }
                    else return true;
                default:
                    return true;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }
    }
    //(vector(現在のX,現在のY),何プレイヤーなのか)
    public Vector3 ToWorldPos(Vector2Int BoardPos, int pID)
    {
        switch (pID)
        {
            case 1:
                return Pos[Stage_Pos[BoardPos.y - 1, BoardPos.x - 1]];
            case 2:
               return Pos2[Stage_Pos[BoardPos.y - 1, BoardPos.x - 1]];
            default:
                return Pos[Stage_Pos[BoardPos.y - 1, BoardPos.x - 1]];
        }

    }
}
