using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
    }
}
