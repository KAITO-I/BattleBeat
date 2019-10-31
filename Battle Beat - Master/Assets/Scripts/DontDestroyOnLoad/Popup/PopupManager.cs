using UnityEngine;
using UnityEngine.UI;

namespace CoreManager
{
    delegate void PopupProcess();

    class PopupManager
    {
        static PopupManager instance;
        public static PopupManager Instance
        {
            get {
                if (instance == null) Debug.LogError("PopupManagerがインスタンスされていません。");
                return instance;
            }
        }

        ControllerManager controllerManager;

        GameObject popupObject;
        bool       isDisplaying;

        Text message;

        (ControllerManager.Button, PopupButton)[] popupButtons;
        int selectedButtonNum;

        internal PopupManager(GameObject popupObject)
        {
            if (instance != null) return;
            instance = this;

            this.controllerManager = ControllerManager.Instance;

            (this.popupObject = popupObject).SetActive(false);
            this.isDisplaying = false;

            this.message = popupObject.transform.Find("Background").Find("Message").GetComponent<Text>();

            Transform parentButton = popupObject.transform.Find("Background").Find("Buttons");
            this.popupButtons = new (ControllerManager.Button, PopupButton)[2];
            this.popupButtons[0].Item2 = new PopupButton(parentButton.Find("LeftButton").gameObject);
            this.popupButtons[1].Item2 = new PopupButton(parentButton.Find("RightButton").gameObject);

            this.selectedButtonNum = 0;
        }

        public void Display(string message, (ControllerManager.Button button, string text, PopupProcess process) leftButtons, (ControllerManager.Button button, string text, PopupProcess process) rightButtons)
        {
            this.message.text = message;

            this.popupButtons[0].Item1 = leftButtons.button;
            this.popupButtons[0].Item2.SetButtonData(leftButtons.text, leftButtons.process);

            this.popupButtons[1].Item1 = rightButtons.button;
            this.popupButtons[1].Item2.SetButtonData(rightButtons.text, rightButtons.process);

            this.popupObject.SetActive(this.isDisplaying = true);
            this.popupButtons[0].Item2.Selected(true);
        }

        //==============================
        // Update
        //==============================
        internal void Update()
        {
            if (!this.isDisplaying) return;

            // 表示中の処理
            // 左右の入力
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.selectedButtonNum++;
                if (this.selectedButtonNum > 1) this.selectedButtonNum = 1;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.selectedButtonNum--;
                if (this.selectedButtonNum < 0) this.selectedButtonNum = 0;
            }
            Debug.Log(this.selectedButtonNum);
            for (int i = 0; i < 2; i++)
            {
                this.popupButtons[i].Item2.Selected(this.selectedButtonNum == i);
            }

            // ボタン判定
            /*foreach ((ControllerManager.Button button, PopupButton popupButton) in this.popupButtons)
            {
                if (this.controllerManager.GetButtonDown_Menu(button))
                {
                    this.popupObject.SetActive(this.isDisplaying = false);
                    popupButton.Press();
                    break;
                }
            }*/
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.popupObject.SetActive(this.isDisplaying = false);
                this.popupButtons[this.selectedButtonNum].Item2.Press();
            }
        }
    }

    
    class PopupButton
    {
        Image        buttonImage;
        Text         buttonText;
        PopupProcess buttonProcess;

        internal PopupButton(GameObject buttonObject)
        {
            this.buttonImage = buttonObject.transform.Find("Image").GetComponent<Image>();
            this.buttonText  = buttonObject.transform.Find("Text").GetComponent<Text>();
        }

        internal void SetButtonData(string text, PopupProcess buttonProcess)
        {
            this.buttonText.text = text;
            this.buttonProcess   = buttonProcess;
        }

        internal void Selected(bool selected)
        {
            this.buttonImage.color = selected ? Color.red : Color.white;
        }

        internal void Press()
        {
            this.buttonImage.color = Color.yellow;
            this.buttonProcess();
        }
    }
}
