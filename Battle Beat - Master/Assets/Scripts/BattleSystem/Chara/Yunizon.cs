using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yunizon : Player
{
    public bool Guard;

    protected override void IStart()
    {
        Guard = false;
    }

    private bool ClassicAttackProcess(int i)
    {
        var Skill = SkillPrefabs[i].GetComponent<AttackItemBase>() as BasicAttack;
        if (CoolDownCount[i] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[i]);
            if (i == 2)//ユニのトラップ
            {
                obj.transform.GetChild(0).GetComponent<UniAnimation>().GetSetUniZoneObj = this.gameObject;
            }
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
        else
        {
            return false;
        }
    }

    protected override void Attack_1()
    {
        var rlt = ClassicAttackProcess(0);
        if (rlt)
        {
            base.Attack_1();
        }
    }
    protected override void Attack_2()
    {
        var rlt = ClassicAttackProcess(1);
        if (rlt)
        {
            base.Attack_2();
        }
    }
    protected override void Attack_3()
    {
        var rlt = ClassicAttackProcess(2);
        if (rlt)
        {
            base.Attack_3();
        }
    }
    protected override void Attack_4()
    {
        var Skill = SkillPrefabs[3].GetComponent<AttackItemBase>();
        if (CoolDownCount[3] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[3]);
            Skill = obj.GetComponent<AttackItemBase>();
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            Sp -= Skill.SpCost;
            nowAttack = Skill;
            wait = int.MaxValue;
            if (wait > 0)
            {
                waitAttackId = 3;
            }
            TurnManager._instance.Add(Skill);
            base.Attack_4();
        }
    }
    public override void Turn_AttackPhase()
    {
        base.Turn_AttackPhase();
    }
    public override void TakeDamage(float Damage)
    {
        AnimationController.Damage();
        if (!Guard)
        {
            base.TakeDamage(Damage);
        }
        else
        {
            base.TakeDamage(0);
        }
    }
}
