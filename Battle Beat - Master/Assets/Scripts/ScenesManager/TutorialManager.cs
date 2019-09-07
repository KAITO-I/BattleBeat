using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    [SerializeField] Sprite[] TutorialImages;
    [SerializeField]Image TutorialPanel;
    [SerializeField]int pageCount;
    ControllerManager controller;
    // Start is called before the first frame update
    void Start()
    {
        pageCount = 0;
        TutorialPanel.sprite = TutorialImages[pageCount];
        controller = ControllerManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetButtonDown_Menu(ControllerManager.Button.A)|| Input.GetKeyDown(KeyCode.A))
        {
            ImageChange(1);
        }
        else if (controller.GetButtonDown_Menu(ControllerManager.Button.B) || Input.GetKeyDown(KeyCode.S))
        {
            ImageChange(-1);
        }
    }
    //画像入れ替え
    void ImageChange(int num)
    {
        if (pageCount == 0 && num == -1)
        {
            //メニューに戻る
            SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
        }
        else if ((pageCount + num) < TutorialImages.Length)
        {
            TutorialPanel.sprite = TutorialImages[pageCount += num];
        }
    }
}
