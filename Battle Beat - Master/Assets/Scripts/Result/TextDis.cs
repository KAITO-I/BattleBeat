using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDis : BaseResultState
{
    GameObject WordPos;
    float intervalForCharacterDisplay = 0.05f;  // 1文字の表示にかかる時間
    Text Word;

    private float timeUntilDisplay = 0;     // 表示にかかる時間
    private float timeElapsed = 1;          // 文字列の表示を開始した時間
    private int lastUpdateCharacter = -1;       // 表示中の文字数
    private string WordText;


    public TextDis(SoundManager s) : base(s)
    {
        WordPos.SetActive(true);
        // 想定表示時間と現在の時刻をキャッシュ
        timeUntilDisplay = WordText.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        // 文字カウントを初期化
        lastUpdateCharacter = -1;

    }

    public override bool Update()
    {
        // クリックから経過した時間が想定表示時間の何%か確認し、表示文字数を出す
        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * WordText.Length);
        // 表示文字数が前回の表示文字数と異なるならテキストを更新する
        if (displayCharacterCount != lastUpdateCharacter)
        {
            Word.text = WordText.Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
        //すべて表示したら飛べるようにする
        if (displayCharacterCount == WordText.Length)
        {
            _finish = true;
        }
        return _finish;
    }
}
