using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectButtom : MonoBehaviour
{
    //選択取っているので入力はできるよ

    //ボタン認識->リスト化？構造化？
    //最初
    [SerializeField]Button PlayMode; [SerializeField] Button Cfg; [SerializeField] Button Credit; [SerializeField] Button Back;
    //二個目
    [SerializeField] Button PlayGame; [SerializeField] Button Training; [SerializeField] Button BackSecond;
    //キャンバス内グループオブジェクト
    GameObject Grup2;

    //最初の選択
    //二番目の選択へ
    public void OnClickPlayMode()
    {
        //カーソル指定とローテーション・背景操作
        Debug.Log("mode選択へ");
        PlayGame.Select();
    }
//設定移動
    public void OnClickCfg()
    {
        Debug.Log("Config");
    }
//クレジット・スタッフロール
    public void OnClickCredit()
    {
        Debug.Log("Credit");
    }
//タイトルへ戻る
    public void OnClickBack()
    {
        Debug.Log("GoMainTitle");
    }

    //二番目の選択
    //ゲーム開始ボタン
    public void OnClickPlayGame()
    {
        Debug.Log("Play Game!");
        SceneManager.LoadScene("PlayGame");
    }
    //練習・訓練ボタン
    public void OnClickTraining()
    {
        //Debug.Log("GoTrainig");
        SceneManager.LoadScene("PlayGame");
    }
    //最初の選択に戻る
    public void OnCkickBackSecond()
    {
        //カーソル指定とローテーション・背景操作
        Debug.Log("最初の選択へ");
        PlayMode.Select();
    }

    private void Start()
    {
        //最初のボタンをセット
        PlayMode = GameObject.Find("/Canvas/PlayMode").GetComponent<Button>();
        Cfg = GameObject.Find("/Canvas/Config").GetComponent<Button>();
        Credit = GameObject.Find("/Canvas/Credit").GetComponent<Button>();
        Back = GameObject.Find("/Canvas/Back(MainTitle)").GetComponent<Button>();
        //二つ目
        PlayGame = GameObject.Find("/Canvas/PlaySelect/Battle").GetComponent<Button>();
        Training = GameObject.Find("/Canvas/PlaySelect/Training").GetComponent<Button>();
        BackSecond = GameObject.Find("/Canvas/PlaySelect/Back(Select)").GetComponent<Button>();
        //ボタン初期選択
        PlayMode.Select();

        //グループをセット
        Grup2 = GameObject.Find("Canvas/PlaySelect").GetComponent<GameObject>();
    }

}
