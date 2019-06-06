using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MoveComand
    {
        Left,
        Right,
        Up,
        Down,
        Attack_1
    }

    //移動時に必要
    List<Vector3> Pos = new List<Vector3>();
    
    //指定した場所に出現させるため
    public int NowPoint_X;
    public int NowPoint_Y;

    [SerializeField]
    int PlayerID;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(NowPoint_X,NowPoint_Y),PlayerID);
        //Playerの位置が同じになってしまうので少し上げる
        transform.position += new Vector3(0, 1f, 0);

    }

    // Update is called once per frame
    void Update()
    {
        Test();
    }

    private void Test()
    {
        if (true)
        {
            //動けたら//ここにジャンプの判定が来る
            //稲福＆木原のスクリプトを入れる

            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerMove(MoveComand.Left);
            }//右に移動
            else if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerMove(MoveComand.Right);
            }
            //上に移動
            else if (Input.GetKeyDown(KeyCode.W))
            {
                PlayerMove(MoveComand.Up);
            }
            //下に移動
            else if (Input.GetKeyDown(KeyCode.S))
            {
                PlayerMove(MoveComand.Down);
            }
        }
    }



    //Playerが動けるタイミングになったら活動できる
    void  PlayerMove(MoveComand move)
    {
        int Posi_Copy =NowPoint_Y;
        int Posi_Copy2 = NowPoint_X;

        //ジャンプのタイミングを一定間隔::どの位置に飛ぶのかチェック
        //行動を操作する
        switch (move)
        {
            //左移動
            case MoveComand.Left:
                Posi_Copy2 -= 1;
                if (!BoardManager._instance.Is_In_Stage(Posi_Copy2 - 1, Posi_Copy - 1,PlayerID))
                {
                    Posi_Copy2 = NowPoint_X;
                }
                break;
            //右移動
            case MoveComand.Right:
                Posi_Copy2 += 1;
                if (!BoardManager._instance.Is_In_Stage(Posi_Copy2 - 1, Posi_Copy - 1, PlayerID))
                {
                    Posi_Copy2 = NowPoint_X;
                }
                break;
            //上移動
            case MoveComand.Up:
                Posi_Copy += 1;
                if (!BoardManager._instance.Is_In_Stage(Posi_Copy2 - 1, Posi_Copy - 1, PlayerID))
                {
                    Posi_Copy = NowPoint_Y;
                }
                break;
            //下移動
            case MoveComand.Down:
                Posi_Copy -= 1;
                if (!BoardManager._instance.Is_In_Stage(Posi_Copy2 - 1, Posi_Copy - 1, PlayerID))
                {
                    Posi_Copy = NowPoint_Y;
                }
                break;
            //攻撃移動
            case MoveComand.Attack_1:
                Attack(Posi_Copy);
                break;
            default:
                break;
        }

        
        //ここでポジションを変更してる
        transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(Posi_Copy2, Posi_Copy), PlayerID);
        //Playerの位置が同じになってしまうので少し上げる
        transform.position += new Vector3(0, 1f, 0);
        //現在のポジションを入れる
        NowPoint_X = Posi_Copy2;
        NowPoint_Y = Posi_Copy;

    }


    //バグが出ないようにしている
    void Attack(int c)
    {

    }
}

