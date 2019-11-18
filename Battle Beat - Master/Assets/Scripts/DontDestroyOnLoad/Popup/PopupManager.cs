using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace CoreManager
{
    delegate void CalloutProcess();

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

        PopupCollout[] collouts;
        int selectedButtonNum;

        public PopupManager(GameObject popupObject)
        {
            if (instance != null) return;
            instance = this;

            this.controllerManager = ControllerManager.Instance;

            (this.popupObject = popupObject).SetActive(false);
            this.isDisplaying = false;

            this.message = popupObject.transform.Find("Background").Find("Message").GetComponent<Text>();

            Transform parentButton = popupObject.transform.Find("Background").Find("Callouts");
            this.collouts = new PopupCollout[2];
            this.collouts[0] = new PopupCollout(parentButton.Find("Up").gameObject);
            this.collouts[1] = new PopupCollout(parentButton.Find("Down").gameObject);

            this.selectedButtonNum = 0;
        }

        void Display(string message, (string text, CalloutProcess process) leftButtons, (string text, CalloutProcess process) rightButtons)
        {
            this.message.text = message;

            this.collouts[0].SetButtonData(leftButtons.text, leftButtons.process);
            this.collouts[1].SetButtonData(rightButtons.text, rightButtons.process);

            this.popupObject.SetActive(this.isDisplaying = true);
            this.collouts[0].Selected(true);
            this.collouts[1].Selected(false);
        }

        //==============================
        // Update
        //==============================
        internal void Update()
        {
            if (!this.isDisplaying) return;

            // 表示中の処理
            // 上下の入力
            float dpadY = ControllerManager.Instance.GetAxis_Menu(ControllerManager.Axis.DpadY);
            if (Mathf.Abs(dpadY) > 0.5)
            {
                if (dpadY > 0)
                {
                    // 上入力
                    this.selectedButtonNum = 0;
                    this.collouts[0].Selected(true);
                    this.collouts[1].Selected(false);
                } else
                {
                    // 下入力
                    this.selectedButtonNum = 1;
                    this.collouts[0].Selected(false);
                    this.collouts[1].Selected(true);
                }
            }

            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.A))
            {
                this.popupObject.SetActive(this.isDisplaying = false);
                this.collouts[this.selectedButtonNum].Press();
            }

            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.B)) this.popupObject.SetActive(this.isDisplaying = false);
        }
    }
    
    class PopupCollout
    {
        Image          image;
        Text           text;
        CalloutProcess process;

        internal PopupCollout(GameObject buttonObject)
        {
            this.image = buttonObject.transform.Find("Image").GetComponent<Image>();
            this.text  = buttonObject.transform.Find("Text").GetComponent<Text>();
        }

        internal void SetButtonData(string text, CalloutProcess buttonProcess)
        {
            this.text.text = text;
            this.process   = buttonProcess;
        }

        internal void Selected(bool selected)
        {
            this.image.rectTransform.localScale = selected ? new Vector2(1.5f, 1.5f) : new Vector2(1f, 1f);
        }

        internal void Press()
        {
            Timer timer = new Timer(1000);
            timer += ()
            this.process();
        }
    }
}
