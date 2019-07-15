using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpManager : MonoBehaviour
{
    void Start()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
    }
}
