using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField]
    SKillGrid[] sKillGrids = new SKillGrid[4];
    [SerializeField]
    Player p;
    [SerializeField]
    UltimateSkillMask skillMask;

    public void Init(Setting.Chara chara, Player p)
    {
        CharaData data = Resources.Load<CharaData>("CharacterData/" + chara.ToString());
        Player player = data.prefab.GetComponent<Player>();
        GameObject[] SkillPrefabs = player.SkillPrefabs;
        int[] turns = new int[4];

        for (int i = 0; i < 4; i++)
        {
            turns[i] = SkillPrefabs[i].GetComponent<AttackItemBase>().CoolDown;
        }

        for (int i = 0; i < 4; i++)
        {
            sKillGrids[i].Init(turns[i], data.SkillIcons[i]);
        }
        this.p = p;
    }

    public void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            sKillGrids[i].SetTurn(p.CoolDownCount[i]);
            sKillGrids[i].SetOnUse(p.getWaitingAttack() == i);
        }
        if (p.Sp == 100f)
        {
            skillMask.setAvailable(false);
        }
        else
        {
            skillMask.setAvailable(true);
        }
    }
}
