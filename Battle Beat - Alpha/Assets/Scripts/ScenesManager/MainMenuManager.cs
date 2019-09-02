using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private enum State
    {
        Left,
        Right,
        Changing
    }

    private ControllerManager controller;
    private State state;
    private int selected;
    private bool canPressDpadY;

    [SerializeField]
    private float moveTime;

    [Header("Background")]
    [SerializeField]
    private Transform background;
    [SerializeField]
    private float backLeftPosX;
    [SerializeField]
    private float backRightPosX;

    [Header("MegaphoneTree")]
    [SerializeField]
    private Transform megaphoneTree;
    [SerializeField]
    private Transform megaphoneTreeUI;
    [SerializeField]
    private Transform megaphoneTreeOverlay;
    [SerializeField]
    private int numberOfSigns;
    [SerializeField]
    private GameObject[] mtOverlays; // 上から覆うUI
    [SerializeField]
    private Sprite mtLeftSprite;
    [SerializeField]
    private Sprite mtRightSprite;
    [SerializeField]
    private float mtLeftPosX;
    [SerializeField]
    private float mtRightPosX;
    [SerializeField]
    private float mtSideRotateX;
    [SerializeField]
    private float mtCenterRotateX;
    [SerializeField]
    private float mtOverrayLeftSideRotateX;
    [SerializeField]
    private float mtOverrayRightSideRotateX;

    private void Start()
    {
        this.controller = ControllerManager.Instance;
        this.state = State.Left;
        this.selected = 0;
        this.canPressDpadY = true;
        this.megaphoneTreeUI.GetComponent<Image>().sprite = this.mtLeftSprite;
        ChangeSign();
    }

    private void Update()
    {
        // ロードディング画面の表示中は無効化
        if (SceneLoader.Instance.isLoading) return;

        if (this.state != State.Changing)
        {
            // 十字キー処理
            float axisY = this.controller.GetAxis(ControllerManager.Axis.DpadY);
            if (!Mathf.Approximately(axisY, 0f))
            {
                if (this.canPressDpadY)
                {
                    if (axisY < 0)
                    {
                        this.selected++;
                        if (this.selected > this.numberOfSigns - 1) this.selected = this.numberOfSigns - 1;
                    }
                    else
                    {
                        this.selected--;
                        if (this.selected < 0) this.selected = 0;
                    }
                    this.canPressDpadY = false;
                }
            }
            else this.canPressDpadY = true;

            ChangeSign();

            // ボタン処理
            if (this.state == State.Left)
            {
                if (this.controller.GetButtonDown(ControllerManager.Button.A))
                {
                    switch (this.selected)
                    {
                        case 0: StartCoroutine(LeftToRight()); break;
                    }
                }
            } else
            {
                // A
                if (this.controller.GetButtonDown(ControllerManager.Button.A))
                {
                    switch (this.selected)
                    {
                        case 0: SceneLoader.Instance.LoadScene(SceneLoader.Scenes.CharacterSelect); break;
                    }
                }
                //B
                else if (this.controller.GetButtonDown(ControllerManager.Button.B)) StartCoroutine(RightToLeft());
            }
        }
    }

    private void ChangeSign()
    {
        for (int i = 0; i < this.numberOfSigns; i++)
        {
            if (i == this.selected) this.mtOverlays[i].SetActive(false);
            else                    this.mtOverlays[i].SetActive(true);
        }
    }

    private void ChangeSignToAllBlack()
    {
        for (int i = 0; i < this.numberOfSigns; i++) this.mtOverlays[i].SetActive(true);
    }

    private IEnumerator LeftToRight()
    {
        this.state = State.Changing;
        bool mtSpriteChanged = false;
        ChangeSignToAllBlack();

        float timer = 0f;
        while (true)
        {
            this.background.position = new Vector3(Mathf.Lerp(this.backLeftPosX, this.backRightPosX, timer / this.moveTime), this.background.position.y);
            this.megaphoneTree.position = new Vector3(Mathf.Lerp(this.mtLeftPosX, this.mtRightPosX, timer / this.moveTime), this.megaphoneTree.position.y);

            // 前半 0->90
            if (timer / this.moveTime <= 0.5)
            {
                float rotateUI = Mathf.Lerp(this.mtSideRotateX, this.mtCenterRotateX, timer / this.moveTime);
                this.megaphoneTreeUI.rotation = Quaternion.Euler(this.megaphoneTreeUI.rotation.x, rotateUI, megaphoneTreeUI.rotation.z);
            }
            // 後半 90->0
            else
            {
                if (!mtSpriteChanged)
                {
                    mtSpriteChanged = true;
                    this.megaphoneTreeUI.GetComponent<Image>().sprite = this.mtRightSprite;
                }
                float rotateUI = Mathf.Lerp(mtCenterRotateX, mtSideRotateX, timer / this.moveTime);
                this.megaphoneTreeUI.rotation = Quaternion.Euler(this.megaphoneTreeUI.rotation.x, rotateUI, megaphoneTreeUI.rotation.z);
            }

            float rotateOverlay = Mathf.Lerp(this.mtOverrayLeftSideRotateX, this.mtOverrayRightSideRotateX, timer / this.moveTime);
            this.megaphoneTreeOverlay.rotation = Quaternion.Euler(this.megaphoneTreeOverlay.rotation.x, rotateOverlay, this.megaphoneTreeOverlay.rotation.z);

            timer += Time.deltaTime;
            if (timer >= this.moveTime) break;

            yield return 0;
        }

        this.background.position = new Vector3(this.backRightPosX, this.background.position.y);
        this.megaphoneTree.position = new Vector3(this.mtRightPosX, this.megaphoneTree.position.y);

        this.megaphoneTreeUI.rotation = Quaternion.Euler(this.megaphoneTreeUI.rotation.x, this.mtSideRotateX, this.megaphoneTreeUI.rotation.z);

        this.state = State.Right;
        this.selected = 0;
        ChangeSign();
    }

    private IEnumerator RightToLeft()
    {
        this.state = State.Changing;
        bool mtSpriteChanged = false;
        ChangeSignToAllBlack();

        float timer = 0f;
        while (true)
        {
            background.position = new Vector3(Mathf.Lerp(backRightPosX, backLeftPosX, timer / this.moveTime), background.position.y);
            megaphoneTree.position = new Vector3(Mathf.Lerp(mtRightPosX, mtLeftPosX, timer / this.moveTime), megaphoneTree.position.y);

            // 前半 0->90
            if (timer / this.moveTime <= 0.5)
            {
                float rotate = Mathf.Lerp(mtSideRotateX, mtCenterRotateX, timer / this.moveTime);
                megaphoneTreeUI.rotation = Quaternion.Euler(megaphoneTreeUI.rotation.x, rotate, megaphoneTreeUI.rotation.z);
            }
            // 後半 90->0
            else
            {
                if (!mtSpriteChanged)
                {
                    mtSpriteChanged = true;
                    this.megaphoneTreeUI.GetComponent<Image>().sprite = this.mtLeftSprite;
                }
                float rotate = Mathf.Lerp(mtCenterRotateX, mtSideRotateX, timer / this.moveTime);
                megaphoneTreeUI.rotation = Quaternion.Euler(megaphoneTreeUI.rotation.x, rotate, megaphoneTreeUI.rotation.z);
            }

            float rotateOverlay = Mathf.Lerp(mtOverrayRightSideRotateX, mtOverrayLeftSideRotateX, timer / this.moveTime);
            megaphoneTreeOverlay.rotation = Quaternion.Euler(megaphoneTreeOverlay.rotation.x, rotateOverlay, megaphoneTreeOverlay.rotation.z);

            timer += Time.deltaTime;
            if (timer >= this.moveTime) break;

            yield return 0;
        }

        this.background.position = new Vector3(this.backLeftPosX, this.background.position.y);
        this.megaphoneTree.position = new Vector3(this.mtLeftPosX, this.megaphoneTree.position.y);

        this.megaphoneTreeUI.rotation = Quaternion.Euler(this.megaphoneTreeUI.rotation.x, this.mtSideRotateX, this.megaphoneTreeUI.rotation.z);

        this.state = State.Left;
        this.selected = 0;
        ChangeSign();
    }

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
