using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectCountroll : MonoBehaviour
{
    [SerializeField]
    Image Player01;
    [SerializeField]
    Image Player02;

    [SerializeField]
    Text Player01_text;
    [SerializeField]
    Text Player02_text;

    int Player1;
    int Player2;

    [SerializeField]
    GameObject Text01;
    [SerializeField]
    GameObject Text02;
    
    [SerializeField]
    List<Image> Chara_;

    public GameObject F;
    List<CharaSelectObj> CharaObj;
    int length;

    [SerializeField]
    GameObject Ready;

    string[] charaname =
    {
        "Chara1",
        "Chara2",
        "Chara3",
        "Chara4"
    };
    
    // Start is called before the first frame update
    void Start()
    {
        Ready.SetActive(false);

        Text01.SetActive(false);
        Text02.SetActive(false);
        CharaObj = new List<CharaSelectObj>();
        foreach (Transform v in F.transform)
        {
            var CObj = v.GetComponent<CharaSelectObj>();
            CharaObj.Add(CObj);
            //デバッグ用
            var CImg = v.GetComponent<Image>();
            Chara_.Add(CImg);
        }
        length = F.transform.childCount;

        foreach(var c in CharaObj)
        {
            c.Init();
        }

        Player1 = 0;
        Player2 = 0;
        CharaObj[Player1].charaSelect(1, true);
        CharaObj[Player2].charaSelect(2, true);
        #region ============デバッグ用====================
        //デバッグ用に色を取得する
        //1Pの色変え
        Color color = Chara_[Player1].color;
        Player01.color = color;
        //2Pの色かえ
        Color color2 = Chara_[Player1].color;
        Player02.color = color2;
        #endregion
        //テキスト表示
        Player01_text.text = CharaObj[Player1].GetCharaname;
        Player02_text.text = CharaObj[Player2].GetCharaname;
    }

    // Update is called once per frame
    void Update()
    {
        Test();

        //Charaが二人とも選択されたとき
        if (Text01.activeSelf && Text02.activeSelf)
        {
            Ready.SetActive(true);
            return;
        }
        Ready.SetActive(false);

        //現在のButtonがどれなのかを取得
        GameObject SelectButton = EventSystem.current.currentSelectedGameObject;
        //Buttonが変更された時
    }

    void Test()
    {
        //1P処理
        if (Input.GetKeyDown(KeyCode.S))
        {
            CharaObj[Player1].charaSelect(1, false);
            Player1++;
            Player1 = Player1 % length;
            Debug.Log(Player1);
            CharaObj[Player1].charaSelect(1, true);

            Player01.sprite = CharaObj[Player1].GetCharaSprite;
            Player01_text.text = CharaObj[Player1].GetCharaname;
            //デバッグ用1P色変え
            Color color = Chara_[Player1].color;
            Player01.color = color;
        }
        else if (Input.GetKeyDown(KeyCode.W)){
            CharaObj[Player1].charaSelect(1, false);
            Player1--;
            Player1 = Player1 % length;
            if (Player1 < 0) Player1 = 3;
            Debug.Log(Player1);
            CharaObj[Player1].charaSelect(1, true);

            Player01.sprite = CharaObj[Player1].GetCharaSprite;
            Player01_text.text = CharaObj[Player1].GetCharaname;
            //デバッグ用1P色変え
            Color color = Chara_[Player1].color;
            Player01.color = color;
        }
        //2P処理
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CharaObj[Player2].charaSelect(2, false);
            Player2++;
            Player2 = Player2 % length;
            Debug.Log(Player2);
            CharaObj[Player2].charaSelect(2, true);

            Player02.sprite = CharaObj[Player2].GetCharaSprite;
            Player02_text.text = CharaObj[Player2].GetCharaname;

            //2Pの色かえ
            Color color2 = Chara_[Player2].color;
            Player02.color = color2;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CharaObj[Player2].charaSelect(2, false);
            Player2--;
            Player2 = Player2 % length;
            if (Player2 < 0) Player2 = 3;
            Debug.Log(Player2);
            CharaObj[Player2].charaSelect(2, true);

            Player02.sprite = CharaObj[Player2].GetCharaSprite;
            Player02_text.text = CharaObj[Player2].GetCharaname;

            //2Pの色かえ
            Color color2 = Chara_[Player2].color;
            Player02.color = color2;
        }
        //if(Input.GetKeyDown())

    }
    //public void OnButton(int PID)
    //{
    //    Button ChangeButton = BeforeButton.GetComponent<Button>();
    //    //Charaが選択されたとき、ボタンが操作できないようにする
    //    if (!Click)
    //    {
    //        Navigation nav = new Navigation();
    //        nav.mode = Navigation.Mode.None;
    //        ChangeButton.navigation = nav;
    //        Text01.SetActive(true);
    //        Click = true;
    //    }
    //    else
    //    {
    //        Navigation nav = new Navigation();
    //        nav.mode = Navigation.Mode.Vertical;
    //        ChangeButton.navigation = nav;
    //        Text01.SetActive(false);
    //        Click = false;
    //    }
    //}
}
