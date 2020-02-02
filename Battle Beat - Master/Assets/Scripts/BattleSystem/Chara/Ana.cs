using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ana : Player
{
    int bullet;
    [SerializeField]
    int bulletMax;
    [SerializeField]
    GameObject bulletCounterPrefab;

    Ana_BulletCounter bulletCounter;
    private void Start()
    {
        IStart();

    }
    protected override void IStart()
    {
        base.IStart();
        bulletCounter = Instantiate<GameObject>(bulletCounterPrefab).GetComponent<Ana_BulletCounter>();
        bulletCounter.Init(PlayerID, 32, bulletMax);
        bullet = bulletMax;
    }
    void SetBullet(bool Sub)
    {
        if (Sub)
        {
            bullet--;
            bulletCounter.SubBullet();
        }
        else
        {
            for(int i = 0; i < bulletMax - bullet; i++)
            {
                bulletCounter.AddBullet();
            }
            bullet = bulletMax;
        }
    } 

    private bool ClassicAttackProcess(int i)
    {
        var Skill = SkillPrefabs[i].GetComponent<AttackItemBase>() as BasicAttack;
        if (CoolDownCount[i] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[i]);
            Skill = obj.GetComponent<AttackItemBase>() as BasicAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[i] += Skill.CoolDown + Skill.Delay;
            Sp -= Skill.SpCost;
            wait = Skill.Delay;
            if (wait > 0)
            {
                waitAttackId = i;
            }
            nowAttack = Skill;
            TurnManager._instance.Add(Skill);
            return true;
        }
        return false;
    }
    private bool AttackProcess_HyperBeam(int i)
    {
        var Skill = SkillPrefabs[i].GetComponent<AttackItemBase>() as BasicAttack;
        if (CoolDownCount[i] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[i]);
            Skill = obj.GetComponent<AttackItemBase>() as BasicAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[i] += Skill.CoolDown + Skill.Delay+2;
            Sp -= Skill.SpCost;
            wait = Skill.Delay+2;
            if (wait > 0)
            {
                waitAttackId = i;
            }
            nowAttack = Skill;
            TurnManager._instance.Add(Skill);
            return true;
        }
        return false;
    }

    protected override void Attack_1()
    {
        if (bullet > 0)
        {
            if (ClassicAttackProcess(0))
            {
                SetBullet(true);
            }
        }
    }
    protected override void Attack_2()
    {
        if (bullet > 0)
        {
            if (ClassicAttackProcess(1))
            {
                SetBullet(true);
            }
        }
    }
    protected override void Attack_3()
    {
        if (ClassicAttackProcess(2))
        {
            SetBullet(false);
        }
    }
    protected override void Attack_4()
    {
        AttackProcess_HyperBeam(3);
    }
    public override void Turn_AttackPhase()
    {
        base.Turn_AttackPhase();
    }
}
