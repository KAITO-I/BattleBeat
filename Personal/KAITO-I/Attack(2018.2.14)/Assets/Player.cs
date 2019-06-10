using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仮のPlayerクラス
public class Player : MonoBehaviour
{
    public float Hp;
    public Vector2Int Pos;
    public int id;
    public virtual void TakeDamage(float Damage) { Hp -= Damage; Debug.Log(gameObject.name+"が"+Damage.ToString()+"ダメージを受けた。"); }

    public GameObject fireAttackPrefab;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //攻撃する際
            GameObject obj = Instantiate<GameObject>(fireAttackPrefab);
            var fire = obj.GetComponent<AttackItemBase>() as Fire;
            //テストの時にrowとcolを0,0にした、普段はplayerの座標はず
            fire.Init(0, 0, false, id);
            AttackManager._instance.Add(fire);
        }
    }
}
