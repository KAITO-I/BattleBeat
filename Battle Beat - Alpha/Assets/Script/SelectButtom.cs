using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectButtom : MonoBehaviour
{

    //最初
    [SerializeField]Button PlayMode; 
    //二個目
    [SerializeField] Button PlayGame; 
    //キャンバス内グループオブジェクト
    [SerializeField]
    GameObject Grup1;
    [SerializeField]
    GameObject Grup2;
    //看板
    [SerializeField]
    RectTransform ImageChange;



    //初期消し用
    bool Set = true;
    //表示用
    bool GrupEnter = false;
    bool GrupEnter2 = false;

    //最初の選択
    //Playを決定時(二番目へ)
    public void OnClickPlayMode()
    {
        Grup1.gameObject.SetActive(false);
        //okのフラグ
        GrupEnter = true;
        

        //カーソル指定とローテーション・背景操作
        Debug.Log("mode選択へ");

    }
    //看板右側でBackを決定時(一番目へ)
    public void OnclickBackSlect()
    {
        Grup2.gameObject.SetActive(false);

        GrupEnter2 = true;
        Debug.Log("menu選択へ");

    }

    private void Start()
    {
        //最初のボタンをセット
        PlayMode = GameObject.Find("/Canvas/Select1/PlayMode").GetComponent<Button>();
 
        //二つ目
        PlayGame = GameObject.Find("/Canvas/Select2/Battle").GetComponent<Button>();
        //ボタン初期選択
        PlayMode.Select();

        //SelectImage(看板)の取得
        ImageChange = GameObject.Find("SelectImage").GetComponent<RectTransform>();



        ////グループをセット
        //Grup1 = GameObject.Find("Canvas/Select1").GetComponent<GameObject>();
        //Grup2 = GameObject.Find("Canvas/Select2").GetComponent<GameObject>();
    }

    private void Update()
    {
        if(Set == true)//認識のため
        {
            Grup2.gameObject.SetActive(false);
            //次読まない為
            Set = false;
        }
        if(GrupEnter == true)//Play決定時の移動処理
        {
            ImageChange.localScale -= new Vector3(0.01f,0,0);
            //既定の位置(1 -> -1へ)
            if (ImageChange.localScale.x <= -1f)
            {
                ImageChange.localScale = new Vector3(-1f, 0.8f, 1f);
                Grup2.gameObject.SetActive(true);
                PlayGame.Select();
                GrupEnter = false;

            }
        }
        if(GrupEnter2 == true)//back決定時の移動処理(看板右の時)
        {
            ImageChange.localScale += new Vector3(0.01f,0,0);
            if(ImageChange.localScale.x >= 1)
            {
                ImageChange.localScale = new Vector3(1f, 0.8f, 1f);
                Grup1.gameObject.SetActive(true);
                PlayMode.Select();
                GrupEnter2 = false;
            }
        }
    }

}
