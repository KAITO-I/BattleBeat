using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MainMenu;

public class MainMenuManager : MonoBehaviour
{
    // 管理系
    private ControllerManager controller;
    [SerializeField]
    private EventSystem es;

    // 状態
    private DisplayState displayState;
    private MegaphoneTreeState mtState;
    private int selectedButtonNum;

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

    private void Start()
    {
        this.controller = ControllerManager.Instance;
        this.es.enabled = true;

        this.displayState      = DisplayState.Menu;
        this.mtState           = MegaphoneTreeState.Left;
        this.selectedButtonNum = 0;

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
    }

    private void Update()
    {
        // ロードディング画面の表示中は無効化
        if (SceneLoader.Instance.isLoading) return;

        switch (this.displayState) {
            case DisplayState.Menu:
                UpdateMenu();
                break;
        }
    }

    private void UpdateMenu() {
        // 移動していないときはボタンの選択状態に合わせてUIを変化させる
        if (this.mtState != MegaphoneTreeState.Changing) {
            // 看板切り替え
            int selectedButtonNum = int.Parse(es.currentSelectedGameObject.name);
            if (this.selectedButtonNum != selectedButtonNum) {
                if (this.mtState == MegaphoneTreeState.Left) {
                    this.mtLeftSigns[this.selectedButtonNum].SignUnselected();
                    this.selectedButtonNum = selectedButtonNum;
                    this.mtLeftSigns[this.selectedButtonNum].SignSelected();
                } else {
                    this.mtRightSigns[this.selectedButtonNum].SignUnselected();
                    this.selectedButtonNum = selectedButtonNum;
                    this.mtRightSigns[this.selectedButtonNum].SignSelected();
                }
            }

            // ボタン操作
            // 左UI
            if (this.mtState == MegaphoneTreeState.Left)
            {
                // A
                if (this.controller.GetButtonDown_Menu(ControllerManager.Button.A)) {
                    switch (this.selectedButtonNum) {
                        case 0: StartCoroutine(LeftToRight()); break;
                        case 1: break;
                        case 2: break;
                    }
                }
                // B
                else if (this.controller.GetButtonDown_Menu(ControllerManager.Button.B)) SceneLoader.Instance.LoadScene(SceneLoader.Scenes.Title);
            }
            // 右UI
            else {
                // A
                if (this.controller.GetButtonDown_Menu(ControllerManager.Button.A)) {
                    switch (this.selectedButtonNum) {
                        case 0: SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect); break;
                        case 1: break;
                        case 2: break;
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
        this.es.enabled = false;

        this.mtState = MegaphoneTreeState.Changing;

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
        this.es.enabled = true;

        this.mtState = MegaphoneTreeState.Right;
        this.selectedButtonNum = 0;

        for (int i = 0; i < 3; i++) {
            if (i == 0) this.mtRightSigns[i].SignSelected();
            else        this.mtRightSigns[i].SignUnselected();
        }
    }

    private IEnumerator RightToLeft()
    {
        // 開始
        this.es.enabled = false;

        this.mtState = MegaphoneTreeState.Changing;

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
        this.es.enabled = true;

        this.mtState = MegaphoneTreeState.Left;
        this.selectedButtonNum = 0;

        for (int i = 0; i < 3; i++) {
            if (i == 0) this.mtLeftSigns[i].SignSelected();
            else        this.mtLeftSigns[i].SignUnselected();
        }
    }
}