using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    Transform target;
    float distance=30;
    float time=0;
    bool Rysum;
    [SerializeField]//0.6
    float interval;
    // Start is called before the first frame update
    void Start()
    {
        Rysum = true;
    }

    // Update is called once per frame
    void Update()
    {
        interval = GameObject.Find("Manager").GetComponent<RythmManager>().getbps;
        interval /= 2;

        time += Time.deltaTime;
        if (interval < time)
        {
            time = 0;
            if (Rysum)
            {
                distance -= 1;
                Rysum = false;
            }
            else
            {
                distance += 1;
                Rysum = true;
            }
            SetCamera();
        }
    }
    void SetCamera()
    {
        var v = Quaternion.Euler(29f,0f,0f);
        var vec = v * Vector3.forward * distance;
        transform.position = target.position - vec;
        transform.LookAt(target);
    }

}
