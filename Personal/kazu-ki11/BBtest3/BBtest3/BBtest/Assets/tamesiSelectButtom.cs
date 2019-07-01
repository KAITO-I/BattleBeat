using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tamesiSelectButtom : MonoBehaviour
{
    private string LoadName;

    public void ClickPlayGame(int i)
    {
        switch (i)
        {
            case 1://ゲーム開始ボタン
                LoadName = "PlayGame";
                break;

            case 2://練習・訓練ボタン
                LoadName = "Training";
                break;

            case 3://設定移動
                LoadName = "Config";
                break;

            case 4://クレジット・スタッフロール
                LoadName = "Credit";
                break;

            case 5://タイトルへ戻る
                LoadName = "Title";
                break;

            default:
                break;
        }
    
    }
    private void Update()
    {
        if(LoadName != null)//シーン外を指定するとエラーもしくは存在しない場合
        {
            Debug.Log(LoadName);
            SceneManager.LoadScene(LoadName);
        }
    }
}
