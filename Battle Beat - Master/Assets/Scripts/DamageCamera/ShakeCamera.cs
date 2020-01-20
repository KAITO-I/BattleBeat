using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private const float shakeTime = 0.2f;
    private const float shakeWidth = 0.15f;

    /// <summary>
    /// 外部呼出しメソッド
    /// </summary>
    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    /// <summary>
    /// 揺れ処理を行うコルーチン
    /// </summary>
    private IEnumerator ShakeCoroutine()
    {
        float measure = 0; //時間計測用
        Vector3 defaultCameraPos = Camera.main.transform.position; //元々のカメラ位置

        while (measure < shakeTime)
        {
            Vector3 nowCameraPos = Camera.main.transform.position;

            //揺れ
            float y = Random.Range(-shakeWidth, shakeWidth);
            nowCameraPos.y = defaultCameraPos.y + y;
            Camera.main.transform.position = nowCameraPos;
            
            measure += Time.deltaTime;
            yield return null;
        }
        //元の位置に戻す
        Camera.main.transform.position = defaultCameraPos;
    }
}
