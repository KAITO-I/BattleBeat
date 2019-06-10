//------------------------------------
//作成者：木原　キャラクターステータス
//------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Character",menuName ="Chara")]
public class CharacterStatas : ScriptableObject
{
    public int id = 0;
    public int HP = 20;
    //攻撃方法・スキル

    public GameObject prefab;

}
