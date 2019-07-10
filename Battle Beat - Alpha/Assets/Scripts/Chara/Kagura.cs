using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kagura : Player
{
    public GameObject[] SkillPrefabs;

    public bool ChainAttackHit;
    private void ClassicAttackProcess(int i)
    {
        if (wait > 0)
        {
            return;
        }
        var Skill = SkillPrefabs[i].GetComponent<AttackItemBase>() as BasicAttack;
        if (CoolDownCount[i] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[i]);
            Skill = obj.GetComponent<AttackItemBase>() as BasicAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[i] += Skill.CoolDown;
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
        var Skill = SkillPrefabs[2].GetComponent<AttackItemBase>() as ChainAttack;
        if (CoolDownCount[2] == 0)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[2]);
            Skill = obj.GetComponent<AttackItemBase>() as ChainAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[2] += Skill.CoolDown;
            nowAttack = Skill;
            AttackManager._instance.Add(Skill);
        }
    }
    protected override void Attack_4()
    {
        ClassicAttackProcess(3);
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
