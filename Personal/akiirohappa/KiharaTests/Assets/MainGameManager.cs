using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] PlayerStatas PlSt;
    [SerializeField] GameObject P1Ob;
    [SerializeField] GameObject P2Ob;
    // Start is called before the first frame update
    void Start()
    {
        CharaSet();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //キャラ生成（試すときはResources/Charaの下にCharacterStatasのアセットをつくってね）
    void CharaSet()
    {
        CharacterStatas[] charas;
        charas = Resources.LoadAll<CharacterStatas>("Chara");
        PlSt = GetComponent<PlayerStatas>();
        for(int i = 0;i < charas.Length; i++)
        {
            if(PlSt.GetId(1) == charas[i].id)
            {
                PlSt.SetChara(1, charas[i]);
            }
            if (PlSt.GetId(2) == charas[i].id)
            {
                PlSt.SetChara(2, charas[i]);
            }
        }
        P1Ob =  Instantiate(PlSt.GetChara(1).Charaprefab);
        P2Ob = Instantiate(PlSt.GetChara(2).Charaprefab);
        //キャラの座標とかはこの後になんやかんやしてね
    }
}
