using UnityEngine;
using UnityEngine.UI;

namespace CoreManager {
    class ModeDisplayer
    {
        static ModeDisplayer instance;
        public static ModeDisplayer Instance
        {
            get
            {
                if (instance == null) Debug.LogError("ModeDisplayerがインスタンスされていません。");
                return instance;
            }
        }

        GameObject go;
        Image      image;
        Text       text;

        internal ModeDisplayer(GameObject modeDisplayer)
        {
            if (ModeDisplayer.instance != null) return;
            ModeDisplayer.instance = this;

            this.go    = modeDisplayer;
            this.image = modeDisplayer.transform.Find("Image").GetComponent<Image>();
            this.text  = this.image.transform.Find("Text").GetComponent<Text>();

            SetActive(false);
        }

        public void SetActive(bool active)
        {
            this.go.SetActive(active);
        }

        public void SetImage(Sprite sprite)
        {
            this.image.sprite = sprite;
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}