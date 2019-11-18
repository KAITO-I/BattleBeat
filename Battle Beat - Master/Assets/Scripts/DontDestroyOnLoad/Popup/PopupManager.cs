using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace CoreManager
{
    delegate void CalloutProcess();

    class PopupManager
    {
        //==============================
        // static
        //==============================
        static PopupManager instance;
        
        public static bool IsActive { get { return PopupManager.instance.isDisplaying; } }

        //==============================
        // 表示
        //==============================
        public static void Display((string text, CalloutProcess process) upCallout, (string text, CalloutProcess) downCallout)
        {
            if (PopupManager.instance == null)
            {
                Debug.LogError("instanceされていないため、実行できません。");
                return;
            }

            // 表示
            PopupManager.instance.DisplayPopup(upCallout, downCallout);
        }

        //==============================
        // class
        //==============================
        ControllerManager controllerManager;

        // ポップアップ本体
        GameObject popupObject;
        bool       isDisplaying;

        // 吹き出し情報と選択状態
        PopupCollout[] collouts;
        int selectedButtonNum;

        //==============================
        // コンストラクタ
        //==============================
        public PopupManager(
            GameObject popupObject,
            (Vector2 selectLocalPos, Vector2 unselectLocalPos) upCallout,
            (Vector2 selectLocalPos, Vector2 unselectLocalPos) downCallout,
            Vector2 unselectLocalScale
        )
        {
            if (instance != null) return;
            instance = this;

            this.controllerManager = ControllerManager.Instance;

            (this.popupObject = popupObject).SetActive(false);
            this.isDisplaying = false;


            Transform parentButton = popupObject.transform.Find("Background").Find("Callouts");
            this.collouts = new PopupCollout[2];
            this.collouts[0] = new PopupCollout(parentButton.Find("Up").gameObject, upCallout, unselectLocalScale);
            this.collouts[1] = new PopupCollout(parentButton.Find("Down").gameObject, downCallout, unselectLocalScale);

            this.selectedButtonNum = 0;
        }

        //==============================
        // Update
        //==============================
        public void Update()
        {
            if (!this.isDisplaying) return;

            // 表示中の処理
            // 上下の入力
            float dpadY = this.controllerManager.GetAxis_Menu(ControllerManager.Axis.DpadY);
            if (Mathf.Abs(dpadY) > 0.5)
            {
                if (dpadY > 0)
                {
                    // 上入力
                    this.selectedButtonNum = 0;
                    this.collouts[0].Select(true);
                    this.collouts[1].Select(false);
                }
                else
                {
                    // 下入力
                    this.selectedButtonNum = 1;
                    this.collouts[0].Select(false);
                    this.collouts[1].Select(true);
                }
            }

            if (this.controllerManager.GetButtonDown_Menu(ControllerManager.Button.A))
            {
                this.popupObject.SetActive(this.isDisplaying = false);
                this.collouts[this.selectedButtonNum].Selected();
            }

            if (this.controllerManager.GetButtonDown_Menu(ControllerManager.Button.B)) this.popupObject.SetActive(this.isDisplaying = false);
        }

        //==============================
        // 表示
        //==============================
        void DisplayPopup((string text, CalloutProcess process) upCollout, (string text, CalloutProcess process) downCallout)
        {
            this.collouts[0].SetButtonData(upCollout.text, upCollout.process);
            this.collouts[1].SetButtonData(downCallout.text, downCallout.process);

            this.popupObject.SetActive(this.isDisplaying = true);
            this.collouts[0].Select(true);
            this.collouts[1].Select(false);
        }
    }

    //==============================
    // 各吹き出し
    //==============================
    class PopupCollout
    {
        Image          image;
        Text           text;
        CalloutProcess process;

        Vector2 selectLocalPos;
        Vector2 unselectLocalPos;
        Vector2 unselectLocalScale;

        //==============================
        // コンストラクタ
        //==============================
        public PopupCollout(GameObject calloutObject, (Vector2 selectLocalPos, Vector2 unselectLocalPos) pos, Vector2 unselectLocalScale)
        {
            this.image = calloutObject.GetComponent<Image>();
            this.text  = calloutObject.transform.Find("Text").GetComponent<Text>();

            this.selectLocalPos     = pos.selectLocalPos;
            this.unselectLocalPos   = pos.unselectLocalPos;
            this.unselectLocalScale = unselectLocalScale;
        }

        //==============================
        // データ設定
        //==============================
        public void SetButtonData(string text, CalloutProcess buttonProcess)
        {
            this.text.text = text;
            this.process   = buttonProcess;
        }

        //==============================
        // 選択
        //==============================
        public void Select(bool select)
        {
            this.image.rectTransform.localPosition = select ? this.selectLocalPos : this.unselectLocalPos;
            this.image.rectTransform.localScale = select ? new Vector2(1f, 1f) : this.unselectLocalScale;
        }

        //==============================
        // 決定
        //==============================
        public void Selected()
        {
            // アニメーション
            /*Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
            
            }*/

            this.process();
        }
    }
}
