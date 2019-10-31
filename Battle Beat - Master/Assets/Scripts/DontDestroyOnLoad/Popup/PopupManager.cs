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

        Image      hotbar;
        Text       message;

        (ControllerManager.Button button, PopupProcess process)[] processes;

        internal PopupManager(GameObject popupObject)
        {
            if (instance != null) return;
            instance = this;

            this.controllerManager = ControllerManager.Instance;

            (this.popupObject = popupObject).SetActive(false);
            this.isDisplaying = false;

            this.message = popupObject.transform.Find("Background").Find("Message").GetComponent<Text>();
            this.hotbar  = popupObject.transform.Find("Background").Find("Hotbar").GetComponent<Image>();
        }

        public void Display(Sprite hotbar, string message, params (ControllerManager.Button button, PopupProcess contents)[] processes)
        {
            this.hotbar.sprite = hotbar;
            this.message.text  = message;

            this.processes = processes;

            this.popupObject.SetActive(this.isDisplaying = true);
        }

        //==============================
        // Update
        //==============================
        internal void Update()
        {
            if (!this.isDisplaying) return;

            // 表示中の処理
            foreach ((ControllerManager.Button button, PopupProcess process) in processes)
            {
                if (!this.controllerManager.GetButtonDown_Menu(button)) continue;

                this.popupObject.SetActive(this.isDisplaying = false);
                process();
                break;
            }
        }
    }
}
