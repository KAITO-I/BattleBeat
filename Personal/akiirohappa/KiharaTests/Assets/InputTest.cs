using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            Debug.Log("plessdany");
        }
        if (Input.GetButton("A_1P"))
        {
            Debug.Log("plessd");
            Debug.Log(Input.GetJoystickNames());
        }

    }
}
