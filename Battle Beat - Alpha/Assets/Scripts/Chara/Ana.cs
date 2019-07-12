using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ana : Player
{
    public GameObject[] SkillPrefabs;

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
    private void AttackProcess_HyperBeam(int i)
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
        AttackProcess_HyperBeam(3);
    }
    public override void Turn_AttackPhase()
    {
        base.Turn_AttackPhase();
    }
}
