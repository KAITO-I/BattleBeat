using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダメージを受けた時にカメラを揺らす処理
/// 【注意点】
/// ・コルーチンを使用している為、ポーズを実装する場合は処理を書き直す必要がある
/// ・Startの時点でMainCameraのPositionを保存している為、Cameraが移動をする場合は書き直す必要がある
/// </summary>
public class CameraDamage : MonoBehaviour
{
    private enum ShakeMode
    {
        Vertical,
        Horizontal,
        Wobble,
        ConstantWobble,
        VerHorMix,
        AllMix_W,
        AllMix_CW
    }

    [SerializeField] private ShakeMode shakeMode = ShakeMode.Horizontal; //揺れ方
    [Range(0, 10)][SerializeField] private float shakeTime = 0; //揺れる時間
    [Range(0, 5)][SerializeField] private float shakeWidth = 0; //揺れ幅
    
    private Vector3 camPos; //カメラの初期位置
    private Quaternion camRot; //カメラの初期角度

    private void Start()
    {
        camPos = transform.position;
        camRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Shake());
        }   
    }

    private IEnumerator Shake()
    {
        float measure = 0; //時間計測用
        float saveRotZ = 0; //1ループ前のRotZの値を保存する用の変数
        while(measure < shakeTime)
        {
            float x = camPos.x;
            float y = camPos.y;
            float rotZ = camRot.z;

            switch (shakeMode)
            {
                //横揺れ
                case ShakeMode.Horizontal:
                    x = RandomX();
                    break;
                //縦揺れ
                case ShakeMode.Vertical:
                    y = RandomY();
                    break;
                //ぐらつき揺れ(ランダム)
                case ShakeMode.Wobble:
                    rotZ = ShakeRotationZ(saveRotZ, true);
                    break;
                //ぐらつき揺れ(定数)
                case ShakeMode.ConstantWobble:
                    rotZ = ShakeRotationZ(saveRotZ, false);
                    break;
                 //縦横複合揺れ
                case ShakeMode.VerHorMix:
                    x = RandomX();
                    y = RandomY();
                    break;
                //全ミックス(ぐらつきランダム)
                case ShakeMode.AllMix_W:
                    x = RandomX();
                    y = RandomY();
                    rotZ = ShakeRotationZ(saveRotZ, true);
                    break;
                 //全ミックス(ぐらつき定数揺れ)
                case ShakeMode.AllMix_CW:
                    x = RandomX();
                    y = RandomY();
                    rotZ = ShakeRotationZ(saveRotZ, false);
                    break;
                default:
                    break;
            }

            //このフレームのRotZの数値を保存
            saveRotZ = rotZ;
            //回転(ぐらつき)処理
            transform.Rotate(0, 0, rotZ);
            //ポジションの移動(揺れ)
            transform.localPosition = new Vector3(x, y, camPos.z);
            //タイムの加算
            measure += Time.deltaTime;
            yield return null;
        }

        //揺れ後は元の位置、角度に戻す
        transform.localPosition = camPos;
        transform.rotation = camRot;
    }

    /// <summary>
    /// Shakeする時のランダムな数値を返す
    /// </summary>
    /// <returns>ShakeWidthの±範囲内のランダムな数値</returns>
    private float RandomShakeNumber()
    {
        return Random.Range(-shakeWidth, shakeWidth);
    }

    /// <summary>
    /// X軸の揺れ
    /// </summary>
    /// <returns>揺れる大きさ(数値)</returns>
    private float RandomX()
    {
        return camPos.x + RandomShakeNumber();
    }

    /// <summary>
    /// Y軸の揺れ
    /// </summary>
    /// <returns>揺れる大きさ(数値)</returns>
    private float RandomY()
    {
        return camPos.y + RandomShakeNumber();
    }

    /// <summary>
    /// ぐらつき揺れの回転値を返す
    /// </summary>
    /// <param name="save">１フレーム前の回転値</param>
    /// <param name="random">ランダムか定数か</param>
    /// <returns>回転する値</returns>
    private float ShakeRotationZ(float save, bool random)
    {
        float shakeNum = 0;
        if (random)
        {
            shakeNum = camRot.z + RandomShakeNumber();
        }
        else
        {
            shakeNum = shakeWidth;
        }

        Debug.Log(shakeNum);

        //RotZの値をplusとminusで交互に入れ替える為の条件分岐
        if (save <= 0)
        {
            return camRot.z + shakeNum;
        }
        else
        {
            return camRot.z + -shakeNum;
        }
    }
}