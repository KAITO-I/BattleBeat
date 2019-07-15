//スキル仕様変更により修正 by　金川 2019-07-07
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float OneGameTime=60f;
    private float Hp;
    public float HpMax;
    public float GetHp() { return Hp; }
    public void SetHp(float hp) { Hp = Mathf.Clamp(hp, 0, HpMax); }
    
    public float Sp;
    public float SpMax;

    public float GetSp() { return Sp; }
    public void SetSp(float sp) { Sp = Mathf.Clamp(sp, 0, SpMax); }

    public Vector2Int Pos;
    protected int wait;
    protected AttackItemBase nowAttack;
    public bool IsStuned;
    public int StunTurn;
    public float DamageToSPFactor = 2f;

    public virtual void TakeDamage(float Damage) {
        SetHp(GetHp()-Damage); Debug.Log(gameObject.name + "が" + Damage.ToString() + "ダメージを受けた。");
        if (nowAttack != null)
        {
            nowAttack.OnInterruption();
        }
        wait = 0;
        SetSp(Damage* DamageToSPFactor+GetSp());
    }
    public virtual float DamageCalc(float p1) { return p1; }
    public virtual float DamageCalc(float p1, float p2) { return p1; }
    public virtual float DamageCalc(float p1, float p2, float p3) { return p1; }
    public virtual float DamageCalc(float p1, float p2, float p3, float p4) { return p1; }
    public virtual void TurnPreprocess() { if (StunTurn > 0) { StunTurn--; if (StunTurn <= 0) { IsStuned = false; } }  }

    public enum MoveComand
    {
        None,
        Left,
        Right,
        Up,
        Down,
        Attack_1,
        Attack_2,
        Attack_3,
        Attack_4
    }

    private MoveComand input = MoveComand.None;
    private bool canInput = true;
    public class KeySets
    {
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode UpKey;
        public KeyCode DownKey;
        public KeyCode Attack_1Key;
        public KeyCode Attack_2Key;
        public KeyCode Attack_3Key;
        public KeyCode Attack_4Key;

        public KeySets(KeyCode leftKey, KeyCode rightKey, KeyCode upKey, KeyCode downKey, KeyCode attack_1Key, KeyCode attack_2Key, KeyCode attack_3Key, KeyCode attack_4Key)
        {
            LeftKey = leftKey;
            RightKey = rightKey;
            UpKey = upKey;
            DownKey = downKey;
            Attack_1Key = attack_1Key;
            Attack_2Key = attack_2Key;
            Attack_3Key = attack_3Key;
            Attack_4Key = attack_4Key;
        }
    }
    public KeySets keySets;
    //指定した場所に出現させるため
    

    [SerializeField]
    public int PlayerID;
    [SerializeField]
    public int[] CoolDownCount = new int[4];

    void Start()
    {
        
        IStart();

    }
    protected virtual void IStart()
    {

    }
    public void Init()
    {
        transform.position = BoardManager._instance.ToWorldPos(Pos);
        //Playerの位置が同じになってしまうので少し上げる
        transform.position += new Vector3(0, 1f, 0);
        Hp = HpMax;
        Sp = 0;
        for (int i = 0; i < 4; i++)
        {
            CoolDownCount[i] = 0;
        }
        wait = 0;
        nowAttack = null;
        StunTurn = 0;
    }
    public virtual void TurnPostprocess() {  canInput = true; input = MoveComand.None; if (wait > 0) { wait--; } }
    // Update is called once per frame
    void Update()
    {
        //Test();
        if (canInput&&wait==0)
        {
            if (Input.GetKeyDown(keySets.LeftKey)) input = MoveComand.Left;
            else if (Input.GetKeyDown(keySets.RightKey)) input = MoveComand.Right;
            else if (Input.GetKeyDown(keySets.UpKey)) input = MoveComand.Up;
            else if (Input.GetKeyDown(keySets.DownKey)) input = MoveComand.Down;
            else if (Input.GetKeyDown(keySets.Attack_1Key)) input = MoveComand.Attack_1;
            else if (Input.GetKeyDown(keySets.Attack_2Key)) input = MoveComand.Attack_2;
            else if (Input.GetKeyDown(keySets.Attack_3Key)) input = MoveComand.Attack_3;
            else if (Input.GetKeyDown(keySets.Attack_4Key)) input = MoveComand.Attack_4;
            if (input != MoveComand.None) canInput = false;
        }
        SetSp(GetSp()+Time.deltaTime / OneGameTime * SpMax);
    }

    public virtual void Turn_MovePhase()
    {
        if (IsStuned)
        {
            if(input >= MoveComand.Attack_1 && input <= MoveComand.Attack_4 && nowAttack != null)
            {
                nowAttack.Cancel();
            }
            return;
        }

        if(input >= MoveComand.Left && input <= MoveComand.Down) {
            PlayerMove(input);
        }

    }
    public virtual void Turn_AttackPhase()
    {
        for (int i = 0; i < 4; i++)
        {
            if (CoolDownCount[i] > 0)
            {
                CoolDownCount[i]--;
            }
        }
        if (IsStuned)
        {
            return;
        }

        if (input >= MoveComand.Attack_1 && input <= MoveComand.Attack_4)
        {
            switch (input)
            {
                case MoveComand.Attack_1:
                    Attack_1();
                    break;
                case MoveComand.Attack_2:
                    Attack_2();
                    break;
                case MoveComand.Attack_3:
                    Attack_3();
                    break;
                case MoveComand.Attack_4:
                    Attack_4();
                    break;
                case MoveComand.None:
                    break;
            }
        }
    }

    //Playerが動けるタイミングになったら活動できる
    void  PlayerMove(MoveComand move)
    {
        int TempY = Pos.y;
        int TempX = Pos.x;

        //ジャンプのタイミングを一定間隔::どの位置に飛ぶのかチェック
        //行動を操作する
        switch (move)
        {
            //左移動
            case MoveComand.Left:
                TempX -= 1;
                if (!BoardManager._instance.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempX = Pos.x;
                }
                break;
            //右移動
            case MoveComand.Right:
                TempX += 1;
                if (!BoardManager._instance.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempX = Pos.x;
                }
                break;
            //上移動
            case MoveComand.Up:
                TempY -= 1;
                if (!BoardManager._instance.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempY = Pos.y;
                }
                break;
            //下移動
            case MoveComand.Down:
                TempY += 1;
                if (!BoardManager._instance.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempY = Pos.y;
                }
                break;
            //攻撃移動
            
            default:
                break;
        }

        if (move >= MoveComand.Left && move <= MoveComand.Down)
        {
            //ここでポジションを変更してる
            transform.position = BoardManager._instance.ToWorldPos(new Vector2Int(TempX, TempY));
            //Playerの位置が同じになってしまうので少し上げる
            transform.position += new Vector3(0, 1f, 0);
            //現在のポジションを入れる
            Pos.x = TempX;
            Pos.y = TempY;
        }

    }

    protected virtual void Attack_1()
    {
    }
    protected virtual void Attack_2()
    {
    }
    protected virtual void Attack_3()
    {
    }
    protected virtual void Attack_4()
    {
    }

    public virtual void ForcedMovement(Vector2Int targetPos)
    {

        Pos=VectorPlusUnderBoardLimit(Pos,targetPos-Pos);
        transform.position = BoardManager._instance.ToWorldPos(Pos);

    }

    private Vector2Int VectorPlusUnderBoardLimit(Vector2Int Pos,Vector2Int vector)
    {
        Vector2Int temp = Pos;
        Vector2Int temptemp = temp;
        int sign = vector.x > 0 ? 1 : -1;
        for (int i = vector.x * sign; i > 0; i--)
        {
            temptemp = temp;
            temp.x += sign;
            if (!BoardManager._instance.Is_In_Stage(temp.x, temp.y, PlayerID))
            {
                temp = temptemp;
                break;
            }
        }
        sign = vector.y > 0 ? 1 : -1;
        for (int i = vector.y * sign; i > 0; i--)
        {
            temptemp = temp;
            temp.y += sign;
            if (!BoardManager._instance.Is_In_Stage(temp.x, temp.y, PlayerID))
            {
                temp = temptemp;
                break;
            }
        }
        return temp;
    }
}

