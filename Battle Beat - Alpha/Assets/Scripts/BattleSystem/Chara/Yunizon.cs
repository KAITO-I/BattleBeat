using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yunizon : Player
{
    public GameObject[] SkillPrefabs;
    public bool Guard;

    protected override void IStart()
    {
        Guard = false;
    }

    private void ClassicAttackProcess(int i)
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
            nowAttack = Skill;
            AttackManager._instance.Add(Skill);
        }
    }

    protected override void Attack_1()
    {
        ClassicAttackProcess(0);
    }
    protected override void Attack_2()
    {
        ClassicAttackProcess(1);
    }
    protected override void Attack_3()
    {
        ClassicAttackProcess(2);
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
            AttackManager._instance.Add(Skill);
        }
    }
    public override void Turn_AttackPhase()
    {
        base.Turn_AttackPhase();
    }
    public override void TakeDamage(float Damage)
    {
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
