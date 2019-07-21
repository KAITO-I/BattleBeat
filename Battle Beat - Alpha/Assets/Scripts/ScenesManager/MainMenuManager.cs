using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
    }

    public void LoadCharacterSelect()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect);
    }

    public void LoadConfig()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Config);
    }
}
