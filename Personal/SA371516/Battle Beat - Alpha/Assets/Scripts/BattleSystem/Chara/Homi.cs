using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homi : Player
{
    public GameObject[] SkillPrefabs;

    public float buffPower = 0f;

    public int onBuff;

    private void ClassicAttackProcess(int i)
    {
        var Skill = SkillPrefabs[i].GetComponent<AttackItemBase>() as BasicAttack;
        if (CoolDownCount[i] == 0 && Skill.SpCost <= Sp)
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[i]);
            Skill = obj.GetComponent<AttackItemBase>() as BasicAttack;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[i] += Skill.CoolDown+Skill.Delay;
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
        var Skill = SkillPrefabs[2].GetComponent<AttackItemBase>() as BuffItem;
        if (CoolDownCount[2] == 0 )
        {
            GameObject obj = Instantiate<GameObject>(SkillPrefabs[2]);
            Skill = obj.GetComponent<AttackItemBase>() as BuffItem;
            Skill.Init(Pos.y, Pos.x, PlayerID == 1 ? false : true, PlayerID);
            CoolDownCount[2] += Skill.CoolDown;
            onBuff += Skill.Duration;
            buffPower += Skill.Power;
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
        if (onBuff > 0)
        {
            onBuff--;
            if (onBuff == 0)
            {
                buffPower = 0;
            }
        }
        base.Turn_AttackPhase(); 
    }
    protected override void IStart()
    {
        onBuff = 0;
    }
    public override float DamageCalc(float p1)
    {
        return p1+buffPower;
    }
}
