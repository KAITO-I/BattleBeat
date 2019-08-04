using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create CharacterData")]
public class CharaData : ScriptableObject
{
    public  string Name;
    public Sprite Avatar;
    public Sprite[] SkillIcons = new Sprite[4];
    public GameObject prefab;
}
