//==============================
// Created by KAITO-I (稲福)
//==============================
using UnityEngine;

//==============================
// BGMクラス
//==============================
public class BackgroundMusic : Sound
{
    //------------------------------
    // 再生
    //------------------------------
    public override Sound Play(AudioClip clip)
    {
        // 既に同じ音源を再生している場合は実行しない
        if (this.AudioSource.clip != clip)
        {
            if (this.AudioSource.isPlaying) Stop();
            this.AudioSource.clip = clip;
            this.AudioSource.Play();
        }
        return this;
    }

    //------------------------------
    // 停止
    //------------------------------
    public override void Stop()
    {
        this.AudioSource.Stop();
    }
}
