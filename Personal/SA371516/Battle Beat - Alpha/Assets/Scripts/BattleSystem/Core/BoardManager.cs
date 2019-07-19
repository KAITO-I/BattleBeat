//スキル仕様変更により修正 by　金川 2019-07-07
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager _instance;
    const uint COLMax = 6;
    const uint ROWMax = 3;
    //移動時に必要
    List<Vector3> Pos = new List<Vector3>();
    [SerializeField]
    //プレイヤーごとに違う
    GameObject Player_Plane;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        //リストの中にステージのポジションを入力
        foreach (Transform tra in Player_Plane.transform)
        {
            Pos.Add(tra.position);
        }
    }
    //Is_In_Stage(x座標,y座標,何Playerなのか)
    public bool Is_In_Stage(int Col, int Row, int pID)
    {
        switch (pID)
        {
            case 1:
                if (Col >= 0 && Row >= 0 && Col < (COLMax / 2) && Row < ROWMax)
                {
                    return true;
                }
                else return false;
            case 2:
                if (Col >= (COLMax / 2) && Row >= 0 && Col < COLMax  && Row < ROWMax)
                {
                    return true;
                }
                else return false;
            default:
                return false;
        }
    }
    //(vector(現在のX,現在のY),何プレイヤーなのか)
    public Vector3 ToWorldPos(Vector2Int BoardPos)
    {
         return Pos[_2DArrayIdx_To_1DArrayIdx(BoardPos.x,BoardPos.y,(int) COLMax)];
    }
    public GameObject GetGameObjectAt(Vector2Int BoardPos,int id)
    {
        int idx = _2DArrayIdx_To_1DArrayIdx(BoardPos.x, BoardPos.y, (int) COLMax);
        GameObject Rlt = null;
        if (idx < Player_Plane.transform.childCount&&idx>=0)
        {
            Rlt = Player_Plane.transform.GetChild(idx).gameObject;
        }
        return Rlt;
    }

     public int _2DArrayIdx_To_1DArrayIdx(int p1,int p2,int p1Max)
    {
        if (p1 >= p1Max||p1<0)
        {
            return -1;
        }
        return p1 + p1Max * p2;
    }
}
