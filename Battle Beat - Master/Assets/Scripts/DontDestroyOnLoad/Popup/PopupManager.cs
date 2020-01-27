using System.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace CoreManager
{
    public delegate void CalloutProcess();

    public class PopupManager
    {
        //==============================
        // static
        //==============================
        static GameObject     popupObject;
        static PopupCollout[] callouts;

        static int selectedCalloutNum;

        static bool isActive;
        public static bool IsActive { get; private set; }

        //==============================
        // 初期化
        //==============================
        public static void Init(
            GameObject popupObject,
            (Vector2 selectLocalPos, Vector2 unselectLocalPos) upCallout,
            (Vector2 selectLocalPos, Vector2 unselectLocalPos) downCallout,
            Vector2 unselectLocalScale
        )
        {
            if (PopupManager.popupObject != null) return;
            (PopupManager.popupObject = popupObject).SetActive(false);

            Transform calloutsTF = popupObject.transform.Find("Background").Find("Callouts");
            PopupManager.callouts    = new PopupCollout[2];
            PopupManager.callouts[0] = new PopupCollout(calloutsTF.Find("Up").gameObject, upCallout, unselectLocalScale);
            PopupManager.callouts[1] = new PopupCollout(calloutsTF.Find("Down").gameObject, downCallout, unselectLocalScale);

            PopupManager.selectedCalloutNum = 0;

            PopupManager.isActive = false;
            PopupManager.IsActive = false;
        }

        //==============================
        // Update
        //==============================
        public static void LateUpdate()
        {
            if (!PopupManager.isActive) return;

            if (!PopupManager.IsActive)
            {
                PopupManager.IsActive = PopupManager.isActive;
                return;
            }

            //===== 表示中の処理 =====
            // 上下入力
            float dpadY = ControllerManager.Instance.GetAxis_Menu(ControllerManager.Axis.DpadY);
            if (Mathf.Abs(dpadY) > 0.5)
            {
                if (dpadY > 0)
                {
                    if (PopupManager.selectedCalloutNum == 0) return;

                    SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
                    PopupManager.selectedCalloutNum = 0;
                    PopupManager.callouts[0].Select(true);
                    PopupManager.callouts[1].Select(false);
                }
                else
                {
                    if (PopupManager.selectedCalloutNum == 1) return;

                    SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
                    PopupManager.selectedCalloutNum = 1;
                    PopupManager.callouts[0].Select(false);
                    PopupManager.callouts[1].Select(true);
                }
            }

            // A入力
            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.A))
                PopupManager.callouts[PopupManager.selectedCalloutNum].Selected();

            // B入力
            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.B))
            {
                SoundManager.Instance.PlaySE(SEID.General_Controller_Back);
                Hide();
            }
        }

        //==============================
        // 表示
        //==============================
        public static void Show((string text, CalloutProcess process) upCallout, (string text, CalloutProcess process) downCallout)
        {
            PopupManager.callouts[0].SetButtonData(upCallout.text, upCallout.process);
            PopupManager.callouts[1].SetButtonData(downCallout.text, downCallout.process);

            PopupManager.popupObject.SetActive(PopupManager.isActive = true);
            PopupManager.selectedCalloutNum = 0;
            PopupManager.callouts[0].Select(true);
            PopupManager.callouts[1].Select(false);
        }

        //==============================
        // 非表示
        //==============================
        internal static void Hide()
        {
            PopupManager.popupObject.SetActive(PopupManager.IsActive = PopupManager.isActive = false);
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
            /*
            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {

            };
            timer.Start();
            */
            PopupManager.Hide();
            this.process();
        }
    }
}
