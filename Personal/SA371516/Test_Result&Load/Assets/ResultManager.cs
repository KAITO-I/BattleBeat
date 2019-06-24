using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    Text PlayerName,Charaname,Word;
    //キャラクターの名前
    string[] Name=
    {
        "カグラ",
        "ホーミー",
        "アナ",
        "ユニ＆ゾーン"
    };

    //キャラクター台詞
    string[] Words ={
        "Chara1,これはテストの表示です",
        "Chara2,これはテストの表示です",
        "Chara3,これはテストの表示です",
        "Chara4,これはテストの表示です "
    };

    [SerializeField]
    [Range(0.001f, 0.3f)]
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間

    private float timeUntilDisplay = 0;     // 表示にかかる時間
    private float timeElapsed = 1;          // 文字列の表示を開始した時間
    private int lastUpdateCharacter = -1;       // 表示中の文字数

    //勝利したほうの情報を受け取る
    int WinPlayerID;
    [SerializeField]
    int WinCharaID;
    // Start is called before the first frame update
    void Start()
    {
        //勝利したほうを読み込む

        //勝利したキャラの台詞を読み込む
        SetText();
    }

    // Update is called once per frame
    void Update()
    {

        // クリックから経過した時間が想定表示時間の何%か確認し、表示文字数を出す
        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * Words[WinCharaID-1].Length);

        // 表示文字数が前回の表示文字数と異なるならテキストを更新する
        if (displayCharacterCount != lastUpdateCharacter)
        {
            Word.text = Words[WinCharaID-1].Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
    }
    //シーン移動
    void OnClick()
    {

    }
    //テキスト変更
    void SetText()
    {
        // 想定表示時間と現在の時刻をキャッシュ
        timeUntilDisplay = Words.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        // 文字カウントを初期化
        lastUpdateCharacter = -1;

        Charaname.text = Name[WinCharaID - 1];
    }

}
