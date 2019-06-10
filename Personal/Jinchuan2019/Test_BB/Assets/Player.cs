using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Hp;
    public Vector2Int Pos;
    public virtual void TakeDamage(float Damage) { Hp -= Damage; Debug.Log(gameObject.name + "が" + Damage.ToString() + "ダメージを受けた。"); }

    public GameObject fireAttackPrefab;
    public enum MoveComand
    {
        None,
        Left,
        Right,
        Up,
        Down,
        Attack_1
    }

    private MoveComand input = MoveComand.None;

    public KeyCode LeftKey;
    public KeyCode RightKey;
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode AttackKey;

    //移動時に必要
    List<Vector3> BoardPos = new List<Vector3>();
    
    //指定した場所に出現させるため
    public int NowPoint_X;
    public int NowPoint_Y;

    [SerializeField]
    public int PlayerID;
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
        //Test();

        if (Input.GetKeyDown(LeftKey)) input = MoveComand.Left;
        else if (Input.GetKeyDown(RightKey)) input = MoveComand.Right;
        else if (Input.GetKeyDown(UpKey)) input = MoveComand.Up;
        else if (Input.GetKeyDown(DownKey)) input = MoveComand.Down;
        else if (Input.GetKeyDown(AttackKey)) input = MoveComand.Attack_1;
    }

    public void Turn()
    {
        PlayerMove(input);
        input = MoveComand.None;
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
                Attack();
                break;
            default:
                break;
        }

        if (move >= MoveComand.Left && move <= MoveComand.Down)
        {
            //ここでポジションを変更してる
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(Posi_Copy2, Posi_Copy), PlayerID);
            //Playerの位置が同じになってしまうので少し上げる
            transform.position += new Vector3(0, 1f, 0);
            //現在のポジションを入れる
            NowPoint_X = Posi_Copy2;
            NowPoint_Y = Posi_Copy;
            Pos = new Vector2Int(NowPoint_X-2, 4-NowPoint_Y);
        }

    }


    //バグが出ないようにしている
    void Attack()
    {
        GameObject obj = Instantiate<GameObject>(fireAttackPrefab);
        var fire = obj.GetComponent<AttackItemBase>() as Fire;
        //テストの時にrowとcolを0,0にした、普段はplayerの座標はず
        bool R = PlayerID == 1 ? false : true;
        fire.Init(Pos.y, Pos.x, R, PlayerID);
        AttackManager._instance.Add(fire);
    }
}

