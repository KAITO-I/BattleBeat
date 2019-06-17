using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    Rigidbody rig;
    float Gravity = -8f;
    //Characterの位置
    //ステージ座標
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
    //移動時に必要
    List<Vector3> Pos = new List<Vector3>();
    [SerializeField]
    //プレイヤーごとに違う
    GameObject Plane;

    //指定した場所に出現させるため
  　public int NowPoint_X;
    public int NowPoint_Y;

    //Jump出来るかどうか
    bool OnJump = false;
    Vector3 Movedirection;
    float Jump_Power = 30f;

    // Start is called before the first frame update
    void Start()
    {
        //リストの中にステージのポジションを入力
        foreach(Transform tra in Plane.transform)
        {
            Pos.Add(tra.position);
        }
        //ステージの真ん中からスタート
        transform.position = Pos[Stage_Pos[NowPoint_Y-1, NowPoint_X-1]];

        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //動けたら//ここにジャンプの判定が来る
        //稲福＆木原のスクリプトを入れる
        if (true)
        {
            if (gameObject.tag == "Player") PlayerMove();
            else if (gameObject.tag == "Player2") PlayerMove2();
        }
        //タイミングを計っている
        //Jump_Powerの値でジャンプの高さが変わる
        #region ======ここからは仕様書外のもの========
        if (OnJump)
        {
            Movedirection.y = Jump_Power;
        }
        Movedirection.y -= Jump_Power * Time.deltaTime;
        rig.AddForce(Movedirection, ForceMode.Force);
        #endregion
    }
    //Playerが動けるタイミングになったら活動できる
    void  PlayerMove()
    {
        int Posi_Copy =NowPoint_Y;
        int Posi_Copy2 = NowPoint_X;
        //ジャンプのタイミングを一定間隔::どの位置に飛ぶのかチェック

        float x =0f,  z = 0f;
        //左に移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            Posi_Copy2 -= 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0){
                Posi_Copy2 = NowPoint_X;
            }
        }//右に移動
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Posi_Copy2 += 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy2 = NowPoint_X;
            }
        }
        //上に移動
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Posi_Copy += 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy = NowPoint_Y;
            }
        }
        //下に移動
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Posi_Copy -= 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy = NowPoint_Y;
            }
        }
        //攻撃
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //ぐでたまのスクリプトを入れる
            Attack(Posi_Copy);
        }

        //ここでポジションを変更してる
        transform.position = Pos[Stage_Pos[Posi_Copy - 1, Posi_Copy2 - 1]];
        //Playerの位置が同じになってしまうので少し上げる
        transform.position += new Vector3(0, 1f, 0);
        //現在のポジションを入れる
        NowPoint_X = Posi_Copy2;
        NowPoint_Y = Posi_Copy;

        rig.velocity = new Vector3(x, 0, z);
    }


    void PlayerMove2()
    {
        int Posi_Copy = NowPoint_Y;
        int Posi_Copy2 = NowPoint_X;
        //ジャンプのタイミングを一定間隔::どの位置に飛ぶのかチェック

        float x = 0f, z = 0f;
        //左に移動
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Posi_Copy2 -= 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy2 = NowPoint_X;
            }
        }//右に移動
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Posi_Copy2 += 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy2 = NowPoint_X;
            }
        }
        //上に移動
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Posi_Copy += 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy = NowPoint_Y;
            }
        }
        //下に移動
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Posi_Copy -= 1;
            //ステータスが0の時は動けない
            if (Stage_Pos_Sta[Posi_Copy - 1, Posi_Copy2 - 1] == 0)
            {
                Posi_Copy = NowPoint_Y;
            }
        }
        //攻撃
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //ぐでたまのスクリプトを入れる
            Attack(Posi_Copy);
        }

        transform.position = Pos[Stage_Pos[Posi_Copy - 1, Posi_Copy2 - 1]];
        transform.position += new Vector3(0, 1f, 0);
        NowPoint_X = Posi_Copy2;
        NowPoint_Y = Posi_Copy;
        rig.velocity = new Vector3(x, 0, z);
    }

    void Attack(int c)
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Plane")
        {
            OnJump = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Plane")
        {
            OnJump = false;
        }
    }
}
