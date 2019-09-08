using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect
{
    const string EffectPathRoot = "Effekseer/";
    const string damageEffectPath = "S_Hit_damege1";
    const string waitEffectPath = "S_Weight_Beat_maru";
    const string musicwaveEffectPath = "musicwave";

    public enum Effect
    {
        DAMAGE,
        WAIT,
        MUSICWAVE
    }
    Dictionary<Effect, string> effectPath = new Dictionary<Effect, string> {
        { Effect.DAMAGE,damageEffectPath},
        { Effect.WAIT,waitEffectPath},
        { Effect.MUSICWAVE,musicwaveEffectPath}
    };


    List<GameObject> emitterObjs = new List<GameObject>();

    public GameObject NewAndPlay(GameObject gameObject, Effect effect, bool loop = false,float scale = 2f,float speed = 7f)
    {
        Effekseer.EffekseerEffectAsset effectAsset;
        GameObject EffectObj = new GameObject();
        EffectObj.transform.SetParent(gameObject.transform);
        EffectObj.transform.localPosition = Vector3.zero;
        EffectObj.transform.localRotation = Quaternion.identity;
        EffectObj.transform.localScale *= scale;
        Effekseer.EffekseerEmitter effectEmitter = EffectObj.AddComponent<Effekseer.EffekseerEmitter>();
        effectAsset = Resources.Load<Effekseer.EffekseerEffectAsset>(EffectPathRoot + effectPath[effect]);
        effectEmitter.effectAsset = effectAsset;
        effectEmitter.speed *= speed;
        effectEmitter.isLooping = loop;
        effectEmitter.Play();
        emitterObjs.Add(EffectObj);
        return EffectObj;
    }
    public void CheckAndDestroy()
    {
        foreach (var obj in emitterObjs)
        {
            if (obj == null)
            {
                continue;
            }
            if (!obj.GetComponent<Effekseer.EffekseerEmitter>().exists)
            {
                GameObject.Destroy(obj);
            }
        }
    }
    public void CheckAndDestroy(GameObject obj)
    {
        if (!obj.GetComponent<Effekseer.EffekseerEmitter>().exists)
        {
            emitterObjs.Remove(obj);
            GameObject.Destroy(obj);
        }
    }
}
