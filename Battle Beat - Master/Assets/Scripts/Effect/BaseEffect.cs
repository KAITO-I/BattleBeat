using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect
{
    const string EffectPathRoot = "Effekseer/";
    const string damageEffectPath = "S_Hit_damege1";
    const string waitEffectPath = "S_Weight_Beat_maru";
    const string musicwaveEffectPath = "musicwave";
    const string homei1EffectPath = "homie_attack";
    const string homei2EffectPath = "C_fome_S2";
    const string homei4EffectPath = "C_fome_SP";
    const string kagura1EffectPath = "Kagura1";
    const string kagura2EffectPath = "Kagura2";
    const string kagura4EffectPath = "Kagura4";
    public enum Effect
    {
        DAMAGE,
        WAIT,
        MUSICWAVE,
        HOMEI1,
        HOMEI2,
        HOMEI4,
        KAGURA1,
        KAGURA2,
        KAGURA4
    }
    Dictionary<Effect, string> effectPath = new Dictionary<Effect, string> {
        { Effect.DAMAGE,damageEffectPath},
        { Effect.WAIT,waitEffectPath},
        { Effect.MUSICWAVE,musicwaveEffectPath},
        { Effect.HOMEI1,homei1EffectPath},
        { Effect.HOMEI2,homei2EffectPath},
        { Effect.HOMEI4,homei4EffectPath},
        { Effect.KAGURA1,kagura1EffectPath},
        { Effect.KAGURA2,kagura2EffectPath},
        { Effect.KAGURA4,kagura4EffectPath}
    };


    List<GameObject> emitterObjs = new List<GameObject>();
    List<GameObject> removeList = new List<GameObject>();

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
    public GameObject NewAndPlay(Vector3 Pos,Quaternion rotation,Effect effect, bool loop = false, float scale = 2f, float speed = 7f)
    {
        Effekseer.EffekseerEffectAsset effectAsset;
        GameObject EffectObj = new GameObject();
        EffectObj.transform.position = Pos;
        EffectObj.transform.rotation = rotation;
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
                removeList.Add(obj);
            }
        }
        if (removeList.Count > 0)
        {
            foreach (var obj in removeList)
            {
                emitterObjs.Remove(obj);
            }
            removeList.Clear();
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
