using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour
{

    public Player p;
    Text t;
    private void Start()
    {
        t = GetComponent<Text>();
    }
    private void Update()
    {
        t.text = string.Format("HP:{0:G}/{1:G}\n" +
            "SP:{2:G}/{3:G}\n" +
            "Skill1:{4:G}\n" +
            "Skill2:{5:G}\n" +
            "Skill3:{6:G}\n" +
            "Skill4:{7:G}\n" +
            "Stun:{8:G}\n", p.GetHp(),p.HpMax,p.GetSp(),p.SpMax,p.CoolDownCount[0], p.CoolDownCount[1], p.CoolDownCount[2], p.CoolDownCount[3],p.StunTurn);
        if (p.GetType() == typeof(Homi))
        {
            t.text += string.Format("Buff:{0:G}\n" +
            "BuffPower:{1:G}", ((Homi)p).onBuff, ((Homi)p).buffPower);
        }
    }
}
