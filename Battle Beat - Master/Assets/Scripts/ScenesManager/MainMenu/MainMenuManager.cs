using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using MainMenu;

public class MainMenuManager : MonoBehaviour
{
    // 管理系
    private ControllerManager controller;

    // 状態
    private DisplayState displayState;
    private MegaphoneTreeState mtState;
    private int selectedNum;
    private bool canPushDPadX;
    private bool canPushDPadY;

    // 画面の移動時間
    [SerializeField]
    private float moveTime;

    // 背景
    [Header("Background")]
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private float backLeftPosX;
    [SerializeField]
    private float backRightPosX;

    [Header("ElectricBoard")]
    [SerializeField]
    private Transform electricBoardObj;
    [SerializeField]
    private ModeDescription[] mtLeftModeDescriptions;
    [SerializeField]
    private ModeDescription[] mtRightModeDescriptions;

    private ElectricBoard electricBoard;

    // メガホンツリー
    [Header("MegaphoneTree")]
    [SerializeField]
    private RectTransform megaphoneTree;
    [SerializeField]
    private GameObject mtLeftUI;
    [SerializeField]
    private GameObject mtRightUI;
    [SerializeField]
    private float mtLeftPosX;
    [SerializeField]
    private float mtRightPosX;
    [SerializeField]
    private float mtSideRotateX;
    [SerializeField]
    private float mtCenterRotateX;
    [SerializeField]
    private SignOverlay[] mtLeftSigns;
    [SerializeField]
    private SignOverlay[] mtRightSigns;

    [Header("Config")]
    [SerializeField]
    private GameObject configObj;
    [SerializeField]
    private AudioMixer gameAudio;
    [SerializeField]
    private GameObject[] volumeSliders;
    [SerializeField]
    private Color sliderSelectColor;
    [SerializeField]
    private Color sliderUnselectColor;
    [SerializeField]
    private float volumeSliderSpeed;

    private VolumeController[] volumeControllers;

    [Header("Credit")]
    [SerializeField]
    private MainMenuImageChanger creditImageChanger;

    [Header("Tutorial")]
    [SerializeField]
    private MainMenuImageChanger tutorialImageChanger;

    private void Start()
    {
        this.controller = ControllerManager.Instance;

        this.displayState      = DisplayState.Menu;
        this.mtState           = MegaphoneTreeState.Left;
        this.selectedNum       = 0;
        this.canPushDPadX      = true;
        this.canPushDPadY      = true;

        this.electricBoard = new ElectricBoard(this.electricBoardObj.Find("Title").GetComponent<Text>(), this.electricBoardObj.Find("Description").GetComponent<Text>());
        this.electricBoard.Set(this.mtLeftModeDescriptions[0]);

        this.megaphoneTree.anchoredPosition = new Vector2(this.mtLeftPosX, this.megaphoneTree.anchoredPosition.y);
        this.mtLeftUI.SetActive(true);
        this.mtRightUI.SetActive(false);
        for (int i = 0; i < 3; i++) {
            this.mtLeftSigns[i].Init();
            this.mtRightSigns[i].Init();
            if (i == 0) {
                this.mtLeftSigns[i].SignSelected();
                this.mtRightSigns[i].SignUnselected();
            } else {
                this.mtLeftSigns[i].SignUnselected();
                this.mtRightSigns[i].SignUnselected();
            }
        }

        this.configObj.SetActive(false);

        this.volumeControllers = new VolumeController[3];
        this.volumeControllers[0] = new VolumeController(
            "MasterVol",
            this.gameAudio,
            this.volumeSliders[0].transform.Find("MasterSlider").GetComponent<Slider>(),
            this.sliderSelectColor,
            this.sliderUnselectColor
        );

        this.volumeControllers[1] = new VolumeController(
            "BGMVol",
            this.gameAudio,
            this.volumeSliders[1].transform.Find("MasterSlider").GetComponent<Slider>(),
            this.sliderSelectColor,
            this.sliderUnselectColor
        );

        this.volumeControllers[2] = new VolumeController(
            "SEVol",
            this.gameAudio,
            this.volumeSliders[2].transform.Find("MasterSlider").GetComponent<Slider>(),
            this.sliderSelectColor,
            this.sliderUnselectColor
        );

        this.creditImageChanger.Init();
        this.creditImageChanger.parentGameObject.SetActive(false);

        this.tutorialImageChanger.Init();
        this.tutorialImageChanger.parentGameObject.SetActive(false);

        // BGM
        SoundManager.Instance.PlayBGM(BGMID.MainMenu);
    }

    private void Update()
    {
        // ロードディング画面の表示中は無効化
        if (SceneLoader.Instance.isLoading) return;

        switch (this.displayState) {
            case DisplayState.Menu:
                UpdateMenu();
                break;

            case DisplayState.Config:
                UpdateConfig();
                break;

            case DisplayState.Credit:
                UpdateCredit();
                break;

            case DisplayState.Tutorial:
                UpdateTutorial();
                break;
        }
    }

    private void UpdateMenu() {
        // 移動していないときはボタンの選択状態に合わせてUIを変化させる
        if (this.mtState != MegaphoneTreeState.Changing) {
            // 看板切り替え
            int selectedNum = this.selectedNum;

            float axisY = this.controller.GetAxis_Menu(ControllerManager.Axis.DpadY);
            if (Mathf.Abs(axisY) > 0.5) {
                if (this.canPushDPadY) {
                    this.canPushDPadY = false;
                    if (axisY < 0) {
                        selectedNum++;
                        if (selectedNum > 2) selectedNum = 2;
                    } else {
                        selectedNum--;
                        if (selectedNum < 0) selectedNum = 0;
                    }
                }
            } else this.canPushDPadY = true;

            if (this.selectedNum != selectedNum) {
                SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
                if (this.mtState == MegaphoneTreeState.Left) {
                    this.mtLeftSigns[this.selectedNum].SignUnselected();
                    this.selectedNum = selectedNum;
                    this.mtLeftSigns[this.selectedNum].SignSelected();
                } else {
                    this.mtRightSigns[this.selectedNum].SignUnselected();
                    this.selectedNum = selectedNum;
                    this.mtRightSigns[this.selectedNum].SignSelected();
                }
                this.electricBoard.Set((this.mtState == MegaphoneTreeState.Left) ? this.mtLeftModeDescriptions[this.selectedNum] : this.mtRightModeDescriptions[this.selectedNum]);
            }

            // ボタン操作
            // 左UI
            if (this.mtState == MegaphoneTreeState.Left)
            {
                // A
                if (this.controller.GetButtonDown_Menu(ControllerManager.Button.A))
                {
                    SoundManager.Instance.PlaySE(SEID.General_Controller_Decision);
                    switch (this.selectedNum)
                    {
                        case 0: StartCoroutine(LeftToRight()); break;
                        case 1:
                            this.displayState = DisplayState.Config;
                            this.configObj.SetActive(true);
                            this.selectedNum = 0;
                            this.canPushDPadY = true;
                            this.volumeControllers[0].Select(true);
                            this.volumeControllers[1].Select(false);
                            this.volumeControllers[2].Select(false);
                            break;
                        case 2:
                            this.displayState = DisplayState.Credit;
                            this.creditImageChanger.Init();
                            this.creditImageChanger.parentGameObject.SetActive(true);
                            this.canPushDPadX = true;
                            break;
                    }
                }
                // B
                else if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B))
                {
                    SoundManager.Instance.PlaySE(SEID.General_Controller_Back);
                    SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
                }
            }
            // 右UI
            else
            {
                // A
                if (this.controller.GetButtonDown_Menu(ControllerManager.Button.A))
                {
                    SoundManager.Instance.PlaySE(SEID.General_Controller_Decision);
                    switch (this.selectedNum)
                    {
                        case 0: SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect); break;
                        case 1: break;
                        case 2:
                            this.displayState = DisplayState.Tutorial;
                            this.tutorialImageChanger.Init();
                            this.tutorialImageChanger.parentGameObject.SetActive(true);
                            this.canPushDPadX = true;
                            break;
                    }
                }
                // B
                else if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B)) StartCoroutine(RightToLeft());
            }
        }
    }

    private IEnumerator LeftToRight()
    {
        // 開始
        this.mtState = MegaphoneTreeState.Changing;

        this.electricBoard.Set("", "");

        foreach (SignOverlay overlay in this.mtLeftSigns) overlay.SignUnselected();
        
        // 動作
        float timer     = 0f;
        bool  mtChanged = false;
        while (timer <= this.moveTime)
        {
            this.background.anchoredPosition = new Vector3(Mathf.Lerp(this.backRightPosX, this.backLeftPosX, timer / this.moveTime), this.background.anchoredPosition.y);
            this.megaphoneTree.anchoredPosition = new Vector3(Mathf.Lerp(this.mtLeftPosX, this.mtRightPosX, timer / this.moveTime), this.megaphoneTree.anchoredPosition.y);

            // 前半 0->90
            if (timer / this.moveTime <= 0.5)
            {
                float rotateUI = Mathf.Lerp(this.mtSideRotateX, this.mtCenterRotateX, timer / this.moveTime);
                this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, rotateUI, megaphoneTree.rotation.z);
            }
            // 後半 90->0
            else
            {
                if (!mtChanged)
                {
                    mtChanged = true;
                    this.mtLeftUI.SetActive(false);
                    this.mtRightUI.SetActive(true);
                }
                float rotateUI = Mathf.Lerp(mtCenterRotateX, mtSideRotateX, timer / this.moveTime);
                this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, rotateUI, megaphoneTree.rotation.z);
            }

            timer += Time.deltaTime;
            yield return 0;
        }

        // 修正
        this.background.anchoredPosition = new Vector3(this.backLeftPosX, this.background.anchoredPosition.y);
        this.megaphoneTree.anchoredPosition = new Vector3(this.mtRightPosX, this.megaphoneTree.anchoredPosition.y);

        this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, this.mtSideRotateX, this.megaphoneTree.rotation.z);

        // 終了      
        this.mtState = MegaphoneTreeState.Right;
        this.selectedNum = 0;

        this.electricBoard.Set(this.mtRightModeDescriptions[0]);

        for (int i = 0; i < 3; i++) {
            if (i == 0) this.mtRightSigns[i].SignSelected();
            else        this.mtRightSigns[i].SignUnselected();
        }
    }

    private IEnumerator RightToLeft()
    {
        SoundManager.Instance.PlaySE(SEID.General_Controller_Back);

        // 開始
        this.mtState = MegaphoneTreeState.Changing;

        this.electricBoard.Set("", "");

        foreach (SignOverlay overlay in this.mtRightSigns) overlay.SignUnselected();
        
        // 動作
        float timer     = 0f;
        bool  mtChanged = false;
        while (timer <= this.moveTime)
        {
            this.background.anchoredPosition = new Vector3(Mathf.Lerp(this.backLeftPosX, this.backRightPosX, timer / this.moveTime), this.background.anchoredPosition.y);
            this.megaphoneTree.anchoredPosition = new Vector3(Mathf.Lerp(this.mtRightPosX, this.mtLeftPosX, timer / this.moveTime), this.megaphoneTree.anchoredPosition.y);

            // 前半 0->90
            if (timer / this.moveTime <= 0.5)
            {
                float rotateUI = Mathf.Lerp(this.mtSideRotateX, this.mtCenterRotateX, timer / this.moveTime);
                this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, rotateUI, megaphoneTree.rotation.z);
            }
            // 後半 90->0
            else
            {
                if (!mtChanged)
                {
                    mtChanged = true;
                    this.mtRightUI.SetActive(false);
                    this.mtLeftUI.SetActive(true);
                }
                float rotateUI = Mathf.Lerp(mtCenterRotateX, mtSideRotateX, timer / this.moveTime);
                this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, rotateUI, megaphoneTree.rotation.z);
            }

            timer += Time.deltaTime;
            yield return 0;
        }

        // 修正
        this.background.anchoredPosition = new Vector3(this.backRightPosX, this.background.anchoredPosition.y);
        this.megaphoneTree.anchoredPosition = new Vector3(this.mtLeftPosX, this.megaphoneTree.anchoredPosition.y);

        this.megaphoneTree.rotation = Quaternion.Euler(this.megaphoneTree.rotation.x, this.mtSideRotateX, this.megaphoneTree.rotation.z);

        // 終了
        this.mtState = MegaphoneTreeState.Left;
        this.selectedNum = 0;

        this.electricBoard.Set(this.mtLeftModeDescriptions[0]);

        for (int i = 0; i < 3; i++) {
            if (i == 0) this.mtLeftSigns[i].SignSelected();
            else        this.mtLeftSigns[i].SignUnselected();
        }
    }

    private void UpdateConfig() {
        int selectedNum = this.selectedNum;
        float axisY = this.controller.GetAxis_Menu(ControllerManager.Axis.DpadY);
        if (Mathf.Abs(axisY) > 0.5) {
            if (this.canPushDPadY) {
                this.canPushDPadY = false;
                if (axisY < 0) {
                    selectedNum++;
                    if (selectedNum > 2) selectedNum = 2;
                } else {
                    selectedNum--;
                    if (selectedNum < 0) selectedNum = 0;
                }
            }
        } else this.canPushDPadY = true;
        
        if (this.selectedNum != selectedNum)
        {
            SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
            this.selectedNum = selectedNum;
            for (int i = 0; i < 3; i++) this.volumeControllers[i].Select(i == this.selectedNum);
        }

        float axisX = this.controller.GetAxis_Menu(ControllerManager.Axis.DpadX);
        if (Mathf.Abs(axisX) > 0.5)
        {
            if (axisX > 0) this.volumeControllers[this.selectedNum].AddVolume(this.volumeSliderSpeed);
            else           this.volumeControllers[this.selectedNum].SubVolume(this.volumeSliderSpeed);
        }

        if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B))
        {
            this.volumeControllers[0].Save();
            this.volumeControllers[1].Save();
            this.volumeControllers[2].Save();

            SoundManager.Instance.PlaySE(SEID.General_Controller_Back);
            this.displayState = DisplayState.Menu;
            this.configObj.SetActive(false);
            this.selectedNum = 1;
            this.canPushDPadY = true;
        }
    }

    private void UpdateCredit()
    {
        // キー操作
        float axisX = this.controller.GetAxis_Menu(ControllerManager.Axis.DpadX);
        if (Mathf.Abs(axisX) > 0.5)
        {
            if (this.canPushDPadX)
            {
                this.canPushDPadX = false;
                if (axisX > 0) this.creditImageChanger.Next();
                else           this.creditImageChanger.Back();
            }
        }
        else this.canPushDPadX = true;

        // ボタン操作
        if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B))
        {
            SoundManager.Instance.PlaySE(SEID.General_Controller_Back);
            this.displayState = DisplayState.Menu;
            this.creditImageChanger.parentGameObject.SetActive(false);
        }
    }

    private void UpdateTutorial()
    {
        // キー操作
        float axisX = this.controller.GetAxis_Menu(ControllerManager.Axis.DpadX);
        if (Mathf.Abs(axisX) > 0.5)
        {
            if (this.canPushDPadX)
            {
                this.canPushDPadX = false;
                if (axisX > 0) this.tutorialImageChanger.Next();
                else           this.tutorialImageChanger.Back();
            }
        }
        else this.canPushDPadX = true;

        // ボタン操作
        if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B))
        {
            SoundManager.Instance.PlaySE(SEID.General_Controller_Back);
            this.displayState = DisplayState.Menu;
            this.tutorialImageChanger.parentGameObject.SetActive(false);
        }
    }
}