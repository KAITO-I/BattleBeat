using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : AttackItemBase
{
    public float Power;
    public int Duration;
    int lifeTime;

    Effekseer.EffekseerEmitter emitter;
    public override void Init(int row, int col, bool reverse, int root)
    {
        base.Init(row, col, reverse, root);
        lifeTime = Duration+1;
        emitter = gameObject.AddComponent<Effekseer.EffekseerEmitter>();
        emitter.effectAsset = EffekseerEffectAsset;
        emitter.isLooping = true;
        emitter.speed *= 1.5f;
        emitter.Play();
    }
    public override void TurnProcessPhase2_Main()
    {
        lifeTime--;
    }
    public override bool isEnd()
    {
        if (lifeTime > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
