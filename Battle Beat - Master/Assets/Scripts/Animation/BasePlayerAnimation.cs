using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BasePlayerAnimation:MonoBehaviour
{
    public enum _KusariAnimList
    {
        Toguro,
        Start,
        Finish
    }
    [SerializeField]
    protected GameObject PlayerObj;
    protected Player _playerClass;
    AnimatorStateInfo info_;

    //自分のとぐろ
    public MeshRenderer _renderer;
    float interval;
    float oldbps;

    protected int PosID;
    protected Animator anim;
    protected string PlayAnim;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        if (PlayerObj != null)//ユニアニメーション時には必要ないため
        {
            _playerClass = PlayerObj.GetComponent<Player>();
            RythmManager.instance.Init();
        }
        _renderer.enabled = false;
        interval = RythmManager.instance.getbps /2;
        oldbps = RythmManager.instance.getbps;
    }

    protected virtual void Update()
    {
        //rythm:0.8->0.5
        interval = RythmManager.instance.getbps / 2;
        //リズムの値が大きくなるため、ずれる
        float dif = oldbps - RythmManager.instance.getbps;
        anim.speed = (oldbps + dif) * 2.0f;
    }
    //（プレイヤーの場所,目的地,コマンド）
    public virtual void Move(GameObject Player,Vector3 Goal, Player.MoveComand comand)
    {
        AnimFunc(comand);
        StartCoroutine(MoveColutin(Player, Goal));
    }
    //（コマンド）
    public void Attack(Player.MoveComand comand)
    {
        AnimFunc(comand);
    }
    //この関数をダメージを受けたときに呼ぶ
    public virtual void Damage()
    {
        MoveDamage();
    }

    //タイミングを同じにするため(動かすもの,到達点)
    protected virtual　IEnumerator MoveColutin(GameObject obj, Vector3 Goal)
    {
        float time = 0;
        Vector3 Origin = obj.transform.position;
        Vector3 pos = Origin;
        obj.transform.position = pos;
        //移動
        do
        {
            time += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(Origin, Goal, time / interval);
            Debug.Log(interval);
            yield return new WaitForFixedUpdate();
        } while (time < interval);
    }

    //アニメーション再生
    void AnimFunc(Player.MoveComand comand)
    {
        info_ = anim.GetCurrentAnimatorStateInfo(0);
        //いつでも来てしまうため
        if (!info_.IsName("idol")) return;
        switch (comand)
        {
            case Player.MoveComand.None:
                break;
            case Player.MoveComand.Left:
                if (_playerClass.PlayerID == 1)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Right:
                if (_playerClass.PlayerID == 2)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Up:
                if (_playerClass.PlayerID == 1)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Down:
                if (_playerClass.PlayerID == 2)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Attack_1:
                Attack1();
                break;
            case Player.MoveComand.Attack_2:
                Attack2();
                break;
            case Player.MoveComand.Attack_3:
                Attack3();
                break;
            case Player.MoveComand.Attack_4:
                Attack4();
                break;
        }
    }

    protected virtual void Attack1()
    {
    }
    protected virtual void Attack2()
    {
    }
    protected virtual void Attack3()
    {
    }
    protected virtual void Attack4()
    {
    }

    public virtual void KusariAnim(_KusariAnimList _triggerName, GameObject _enemyObj = null, bool _attack = false)
    {

    }

    //移動は同じなので継承しない
    private void MoveFront()
    {
        anim.SetTrigger("FrontT");
        PlayAnim = "Front";
    }

    private void MoveBack()
    {
        anim.SetTrigger("BackT");
        PlayAnim = "Back";
    }

    private void MoveRight()
    {
        anim.SetTrigger("RightT");
        PlayAnim = "Right";
    }

    private void MoveLeft()
    {
        anim.SetTrigger("LeftT");
        PlayAnim = "left";
    }
    private void MoveDamage()
    {
        anim.SetTrigger("Damage");
        PlayAnim = "Damage";
    }

    //（再生する技のID）
    public virtual void AttackWaitEnd(int waitAttackId)
    {

    }
}
