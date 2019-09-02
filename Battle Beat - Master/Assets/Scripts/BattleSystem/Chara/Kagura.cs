using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kagura : Player
{

    public bool ChainAttackHit;
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
            AttackManager._instance.Add(Skill);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void Attack_1()
    {
        ClassicAttackProcess(0);
        var rlt = ClassicAttackProcess(0);

        if (rlt)
        {
            base.Attack_1();
        }

    }
    protected override void Attack_2()
    {
        var rit=ClassicAttackProcess(1);
        if (rit)
        {
            base.Attack_2();
        }
    }
    protected override void Attack_3()
    {
        var Skill = SkillPrefabs[2].GetComponent<AttackItemBase>() as ChainAttack;
        if (CoolDownCount[2] == 0)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[2]);
            Skill = obj.GetComponent<AttackItemBase>() as ChainAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[2] += Skill.CoolDown;
            wait = 1;
            waitAttackId = 2;
            nowAttack = Skill;
            AttackManager._instance.Add(Skill);
            base.Attack_3();
        }
    }
    protected override void Attack_4()
    {
        var rit = ClassicAttackProcess(3);
        if (rit)
        {
            base.Attack_4();
        }
    }
    public override void Turn_AttackPhase()
    {
        base.Turn_AttackPhase();
    }
    protected override void IStart()
    {
        ChainAttackHit = false;
    }
}
