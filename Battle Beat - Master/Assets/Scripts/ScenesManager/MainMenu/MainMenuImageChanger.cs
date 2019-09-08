using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    [Serializable]
    class MainMenuImageChanger
    {
        public GameObject parentGameObject { get { return this.image.transform.parent.gameObject; } }

        [SerializeField]
        private Image image;
        [SerializeField]
        private Sprite[] sprites;

        private int spriteNum;

        public void Init()
        {
            this.spriteNum = 0;
            UpdateImage();
        }

        private void UpdateImage()
        {
            this.image.sprite = this.sprites[this.spriteNum];
        }

        public void Next()
        {
            this.spriteNum++;
            if (this.spriteNum > this.sprites.Length - 1)
            {
                this.spriteNum = this.sprites.Length - 1;
                return;
            }

            SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
            UpdateImage();
        }

        public void Back()
        {
            this.spriteNum--;
            if (this.spriteNum < 0)
            {
                this.spriteNum = 0;
                return;
            }

            SoundManager.Instance.PlaySE(SEID.General_Controller_Select);
            UpdateImage();
        }
    }
}
