using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BasePlayerAnimation:MonoBehaviour
{
    public float interval;
    [SerializeField]
    GameObject PlayerObj;
    Player player;
    public bool AnimCheck;
    protected int PosID;
    Animator anim;
    AnimatorStateInfo info_;

    public string PlayAnim;
    void Start()
    {
        anim = GetComponent<Animator>();
        player = PlayerObj.GetComponent<Player>();
    }
    public virtual void Move(GameObject Player,Vector3 Goal, Player.MoveComand comand)
    {
        AnimFunc(comand);
        StartCoroutine(enumerator(Player, Goal));
    }
    //タイミングを同じにするため（未完成）(動かすもの,到達点)
    IEnumerator enumerator(GameObject obj, Vector3 Goal)
    {
        float time = 0;

        Vector3 Oragin = obj.transform.position;
        //移動距離を測る
        Vector3 pos = Vector3.Lerp(Oragin, Goal,time/interval);
        obj.transform.position = pos;
        //距離
        while (time<interval)
        {
            time += Time.deltaTime;
            obj.transform.position  = Vector3.Lerp(Oragin, Goal, time / interval);
            yield return new WaitForFixedUpdate();
        }
    }
    public void AnimFunc(Player.MoveComand comand)
    {
        info_ = anim.GetCurrentAnimatorStateInfo(0);
        //いつでも来てしまうため
        if (!info_.IsName("idol")) return;
        AnimCheck = true;
        switch (comand)
        {
            case Player.MoveComand.None:
                break;
            case Player.MoveComand.Left:
                if (player.PlayerID == 1)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Right:
                if (player.PlayerID == 2)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Up:
                if (player.PlayerID == 1)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Down:
                if (player.PlayerID == 2)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Attack_1:
                break;
            case Player.MoveComand.Attack_2:
                break;
            case Player.MoveComand.Attack_3:
                break;
            case Player.MoveComand.Attack_4:
                break;
        }
    }

    //2P視点
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
}
