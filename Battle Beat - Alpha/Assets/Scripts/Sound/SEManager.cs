using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================
// SE専用クラス
//==============================
public class SEManager : Sound
{
    //------------------------------
    // 再生
    //------------------------------
    // [引数]
    // AudioClip sound : 再生する音
    //------------------------------
    public override void Play(AudioClip sound)
    {
        this.audio.clip = sound;
        this.audio
    }
}
