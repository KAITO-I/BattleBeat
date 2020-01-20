using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDamage : MonoBehaviour
{
    [SerializeField]
    private ShakeCamera shake = null;

    private void Update()
    {
        //ダメージを受けた
        if (Input.GetMouseButtonDown(0))
        {
            shake.Shake();
        }
    }
}