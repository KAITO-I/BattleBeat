//スキル仕様変更により修正 by　金川 2019-07-07
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public delegate void OnValueChange(float value);
    public OnValueChange HPChange;
    public OnValueChange SPChange;
    //
    public float OneGameTime=60f;

    public int PlayerID;
    //HP関係
    private float Hp;
    public float HpMax;
    public float GetHp() { return Hp; }
    public void SetHp(float hp) { Hp = Mathf.Clamp(hp, 0, HpMax);if (HPChange != null) { HPChange(Hp); } }
    
    //SP関係
    public float Sp;
    public float SpMax;
    public float GetSp() { return Sp; }
    public void SetSp(float sp) { Sp = Mathf.Clamp(sp, 0, SpMax); if (SPChange != null) { SPChange(Sp); } }
    public float DamageToSPFactor = 2f;

    //ボード上の座標（col、row）
    public Vector2Int Pos;

    //エフェクト
    BaseEffect baseEffect = new BaseEffect();
    //溜め攻撃が自分で中断できないので、そのカウンター
    [SerializeField]
    protected int wait;
    protected int waitAttackId;
    public int getWaitingAttack()
    {
        return wait > 0 ? waitAttackId : -1;
    }
    //最後プレーヤーが生成したAttackItemオブジェクト
    protected AttackItemBase nowAttack;
    //スタンされているかどうか
    public bool IsStuned;
    //スタンTurn数
    public int StunTurn;

    public GameObject[] SkillPrefabs;

    public BasePlayerAnimation AnimationController;
    bool AnimFlag;

    //コントローラクラス
    public ControllerManager.Controller controller;

    //ダメージ受ける関数
    public virtual void TakeDamage(float Damage) {
        SetHp(GetHp()-Damage);
        Debug.Log(gameObject.name + "が" + Damage.ToString() + "ダメージを受けた。");
        if (nowAttack != null)
        {
            nowAttack.OnInterruption();
        }

        //ここにやられたアニメーション再生
        //タメのリセット
        wait = 0;
        SetSp(Damage* DamageToSPFactor+GetSp());

        //エフェクト
        baseEffect.NewAndPlay(gameObject, BaseEffect.Effect.DAMAGE);
    }
    //ダメージ計算関数群（プレーヤーが他のプレーヤーにダメージを与え時にバフやらを考慮して攻撃力の計算）
    public virtual float DamageCalc(float p1) { return p1; }
    public virtual float DamageCalc(float p1, float p2) { return p1; }
    public virtual float DamageCalc(float p1, float p2, float p3) { return p1; }
    public virtual float DamageCalc(float p1, float p2, float p3, float p4) { return p1; }

    
    public int[] CoolDownCount = new int[4];


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

    public bool onGame = false;

    //入力キーの射影
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
    public virtual void TurnPreprocess() {
        if (wait > 0)
        {
            wait--;
            if (wait == 0)
            {
                AnimationController.AttackWaitEnd(waitAttackId);
            }
        }
        if (StunTurn > 0)
        {
            StunTurn--;
            if (StunTurn <= 0)
            { IsStuned = false;
            }
        }
    }
    public virtual void TurnPostprocess() {  canInput = true; input = MoveComand.None;  }
    void Update()
    {
        if (!onGame)
        {
            return;
        }
        //プレーヤー入力
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
            //if (input != MoveComand.None) canInput = false;
            //else if (controller.GetAxis(ControllerManager.Axis.DpadX) < 0) input = MoveComand.Left;
            //else if (controller.GetAxis(ControllerManager.Axis.DpadX) > 0) input = MoveComand.Right;
            //else if (controller.GetAxis(ControllerManager.Axis.DpadY) > 0) input = MoveComand.Up;
            //else if (controller.GetAxis(ControllerManager.Axis.DpadY) < 0) input = MoveComand.Down;
            //else if (controller.GetButtonDown(ControllerManager.Button.Y)) input = MoveComand.Attack_1;
            //else if (controller.GetButtonDown(ControllerManager.Button.X)) input = MoveComand.Attack_2;
            //else if (controller.GetButtonDown(ControllerManager.Button.A)) input = MoveComand.Attack_3;
            //else if (controller.GetButtonDown(ControllerManager.Button.B)) input = MoveComand.Attack_4;
        }

        //Spが時間経過で増やす
        SetSp(GetSp()+Time.deltaTime / OneGameTime * SpMax);

        //effect終わり判定
        baseEffect.CheckAndDestroy();
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

        //wait Effect
        if (wait > 0)
        {
            baseEffect.NewAndPlay(gameObject, BaseEffect.Effect.WAIT);
        }
    }

    void  PlayerMove(MoveComand move)
    {
        int TempY = Pos.y;
        int TempX = Pos.x;
        Debug.Log(move);
        switch (move)
        {
            case MoveComand.Left:
                TempX -= 1;
                if (!BoardManager.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempX = Pos.x;
                }
                break;
            case MoveComand.Right:
                TempX += 1;
                if (!BoardManager.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempX = Pos.x;
                }
                break;
            case MoveComand.Up:
                TempY -= 1;
                if (!BoardManager.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempY = Pos.y;
                }
                break;
            case MoveComand.Down:
                TempY += 1;
                if (!BoardManager.Is_In_Stage(TempX, TempY, PlayerID))
                {
                    TempY = Pos.y;
                }
                break;
            
            default:
                break;
        }

        if (move >= MoveComand.Left && move <= MoveComand.Down)
        {
            var pos= BoardManager._instance.ToWorldPos(new Vector2Int(TempX, TempY));
            pos += new Vector3(0, 1f, 0);
            AnimationController.Move(gameObject, pos, move);
            Pos.x = TempX;
            Pos.y = TempY;
        }

    }

    protected virtual void Attack_1()
    {
        AnimationController.Attack(MoveComand.Attack_1);
    }
    protected virtual void Attack_2()
    {
        AnimationController.Attack(MoveComand.Attack_2);
    }
    protected virtual void Attack_3()
    {
        AnimationController.Attack(MoveComand.Attack_3);
    }
    protected virtual void Attack_4()
    {
        AnimationController.Attack(MoveComand.Attack_4);
    }

    //強制移動関数
    public virtual void ForcedMovement(Vector2Int targetPos)
    {

        Pos=VectorPlusUnderBoardLimit(Pos,targetPos-Pos);
        float y = transform.position.y;
        var p = BoardManager._instance.ToWorldPos(Pos);
        p.y = y;
        transform.position = p;
    }
    //ボード範囲判定
    private Vector2Int VectorPlusUnderBoardLimit(Vector2Int Pos,Vector2Int vector)
    {
        Vector2Int temp = Pos;
        Vector2Int temptemp = temp;
        int sign = vector.x > 0 ? 1 : -1;
        for (int i = vector.x * sign; i > 0; i--)
        {
            temptemp = temp;
            temp.x += sign;
            if (!BoardManager.Is_In_Stage(temp.x, temp.y, PlayerID))
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
            if (!BoardManager.Is_In_Stage(temp.x, temp.y, PlayerID))
            {
                temp = temptemp;
                break;
            }
        }
        return temp;
    }
}

