using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Load_Scr : MonoBehaviour
{
    private AsyncOperation async;

    public Slider SliderObj;

    public Image image;
    Color color;

    private void Start()
    {
        //color = image.color;
        SliderObj.value = 0;
        LoadNextScene();
    }
    public void LoadNextScene()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("SampleScene");

        while (!async.isDone)
        {
            SliderObj.value = async.progress;
            Debug.Log(async.progress);
            //color.a = async.progress;
            //image.color = color;
            yield return null;
        }
    }
}
