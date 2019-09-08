using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    class ElectricBoard
    {
        public Text Title       { get; private set; }
        public Text Description { get; private set; }

        public ElectricBoard(Text title, Text description)
        {
            this.Title       = title;
            this.Description = description;
        }

        public void Set(ModeDescription md)
        {
            this.Title.text       = md.Title;
            this.Description.text = md.Description;
        }
    }

    [Serializable]
    class ModeDescription
    {
        [SerializeField]
        private string title;
        public string Title { get { return this.title; } }
        [SerializeField]
        [TextArea]
        private string description;
        public string Description { get { return this.description; } }
    }
}