using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    public GameObject obj;
    public LayerMask msk;//Raycastで触る物を選択する
    void Start()
    {
        //msk = ~(1 << 2);　レイヤーの順番で触りたくないレイヤーを指定する
        msk = LayerMask.GetMask(new string[] { "Default" });//触るレイヤーを直接指定する
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;//この時点では空っぽ
        if (Physics.Raycast(ray, out hit,Mathf.Infinity,msk))
        {
            obj.transform.position = hit.point;
            //GameObject関数の中のtransformのpositionをRaycastHit関数のpoint(当たった座標)に変更する


        }

    }
}
