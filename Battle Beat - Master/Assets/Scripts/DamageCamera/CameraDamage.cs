using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDamage : MonoBehaviour
{
    private ShakeCamera shake = new ShakeCamera();

    private void Update()
    {
        //ダメージを受けた
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(shake.ShakeCoroutine());
        }
    }
}