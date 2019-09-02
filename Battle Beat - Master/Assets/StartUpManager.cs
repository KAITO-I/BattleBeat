using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpManager : MonoBehaviour
{
    int setPlnum;
    ControllerManager controller;
    void Start()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
        controller = ControllerManager.Instance;
        setPlnum = 1;
    }
    private void Update()
    {
        if(setPlnum <= 2)
        {
            if (controller.ChangeControllerData(setPlnum))
            {
                setPlnum++;
            }
        }
    }
}
