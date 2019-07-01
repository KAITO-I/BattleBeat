using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Jump(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

}
