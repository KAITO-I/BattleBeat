using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDamage : MonoBehaviour
{
    private enum CameraMove
    {
        Vertical,
        Horizontal,
        Wobble
    }

    [SerializeField] private bool debug = false;
    [SerializeField] private CameraMove cameraMove = CameraMove.Horizontal; //揺れ方
    [SerializeField] private float speed = 0; //揺れる速度
    [SerializeField] private float amount = 0; //揺れる量
    [SerializeField] private int count = 0; //揺れる回数
    private int resetCount;
    private Vector3 cameraPos;

    private void Start()
    {
        resetCount = count;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPos = transform.position;
        if (debug)
        {
            //左クリックされた場合実行される
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            ResetCount();
        }

        switch (cameraMove)
        {
            case CameraMove.Horizontal:
                HorizontalShaking();
                break;
            case CameraMove.Vertical:
                VerticalShaking();
                break;
            case CameraMove.Wobble:
                WobbleShaking();
                break;
            default:
                break;
        }
    }

    #region 揺れ処理
    /// <summary>
    /// 横揺れ
    /// </summary>
    private void HorizontalShaking()
    {
        bool right = true;
        //指定したカウント分揺らす
        for(int i = 0; i <= count; i++)
        {
            //右揺れ
            if (right)
            {
                //設定した揺れ幅より小さい
                if (amount > cameraPos.x)
                {
                    Debug.Log("RightIN");
                    //座標を右にずらす
                    cameraPos.x += speed * Time.deltaTime;
                    PositionChange();
                }
                else //端まで来た
                {
                    Debug.Log("RightCount");
                    //フラグをleftにし、カウント
                    right = false;
                    i++;
                }
            }
            //左揺れ
            else
            {
                //設定した揺れ幅より大きい(負の数)
                if (-amount < cameraPos.x)
                {
                    Debug.Log("LeftIN");
                    //座標を左にずらす
                    cameraPos.x -= speed * Time.deltaTime;
                    PositionChange();
                }
                else
                {
                    Debug.Log("LeftCount");
                    //フラグをrightにし、カウント
                    right = true;
                    i++;
                }
            }
            Debug.Log(i);
        }
    }

    /// <summary>
    /// 縦揺れ
    /// </summary>
    private void VerticalShaking()
    {

    }

    /// <summary>
    /// ぐらつく揺れ
    /// </summary>
    private void WobbleShaking()
    {

    }

    private void ResetCount()
    {
        count = resetCount;
    }

    private void PositionChange()
    {
        transform.position = cameraPos;
    }
    #endregion
}