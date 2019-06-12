using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectCountroll : MonoBehaviour
{
    Button button_;
    [SerializeField]
    Image Player01;
    [SerializeField]
    Text Player01_text;
    [SerializeField]
    Text Player02_text;
    [SerializeField]
    Image Player02;
    [SerializeField]
    GameObject Text01;
    [SerializeField]
    GameObject Text02;

    bool Click;

    [SerializeField]
    GameObject Ready;
    GameObject BeforeButton;
    GameObject[] ButtonObj;
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
        //キーボードで選択できるようにする
        button_ =GameObject.Find("Chara1").GetComponent<Button>();
        button_.Select();
        BeforeButton= EventSystem.current.currentSelectedGameObject;
        Ready.SetActive(false);

        Text01.SetActive(false);
        Text02.SetActive(false);

        Click = false;
        //色を取得する
        Color color = BeforeButton.GetComponent<Image>().color;
        Player01.color = color;


    }

    // Update is called once per frame
    void Update()
    {
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
        if (BeforeButton != SelectButton)
        {
            BeforeButton = SelectButton;
            //ここでPlayerに合わせてTextの表示を変える
            switch (SelectButton.name)
            {
                case "Chara1":
                    Player01_text.text = charaname[0];
                    break;
                case "Chara2":
                    Player01_text.text = charaname[1];
                    break;
                case "Chara3":
                    Player01_text.text = charaname[2];
                    break;
                case "Chara4":
                    Player01_text.text = charaname[3];
                    break;
            }
            //色を取得する
            Color color = SelectButton.GetComponent<Image>().color;

            Player01.color = color;
        }
    }
    public void OnButton(int PID)
    {
        Button ChangeButton = BeforeButton.GetComponent<Button>();
        //Charaが選択されたとき、ボタンが操作できないようにする
        if (!Click)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            ChangeButton.navigation = nav;
            Text01.SetActive(true);
            Click = true;
        }
        else
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Vertical;
            ChangeButton.navigation = nav;
            Text01.SetActive(false);
            Click = false;
        }
    }
}
