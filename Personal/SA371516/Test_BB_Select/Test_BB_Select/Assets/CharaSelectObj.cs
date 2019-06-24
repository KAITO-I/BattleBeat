using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaSelectObj : MonoBehaviour
{
    //status
    public Sprite[] Flames = new Sprite[3];
    Image image;
    public Sprite CharaSprite;
    public string Charaname;
    public Sprite GetCharaSprite
    {
        get { return CharaSprite; }
    }
    public string GetCharaname
    {
        get { return Charaname; }
    }
    private bool P1select;
    private bool P2select;

    private void Awake()
    {
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
    }
    public void Init()
    {
        P1select = false;
        P2select = false;
        image.sprite = Flames[3];
    }
    //(操作している人のID,キャラが選択されているか)
    public void charaSelect(int ID,bool OK)
    {
        //何Pが操作したかを判断
        switch (ID)
        {
            case 1:
                P1select = OK;
                break;
            case 2:
                P2select = OK;
                break;
        }
        //枠の画像を変更する
        if (P1select && P2select)
        {
            image.sprite = Flames[0];
        }
        else if (P1select&&!P2select)
        {
            image.sprite = Flames[1];
        }
        else if (P2select&&!P1select)
        {
            image.sprite = Flames[2];
        }
        else if (!P1select && !P2select)
        {
            image.sprite = Flames[3];   

        }
    }

}
