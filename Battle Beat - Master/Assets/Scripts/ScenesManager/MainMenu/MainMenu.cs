using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenu {
    //==============================
    // UI情報
    //==============================
    [Serializable]
    class MegaphoneTreeUI {
        [SerializeField]
        private MegaphoneTreeParts[] parts;
        public MegaphoneTreeParts[] Parts { get { return this.parts; } }
    }

    //==============================
    // 部品情報
    //==============================
    [Serializable]
    class MegaphoneTreeParts {
        [SerializeField]
        private PartType type;
        public PartType Type { get { return this.type; } }
        [SerializeField]
        private GameObject ui;
        public GameObject UI { get { return this.ui; } }
    }

    //==============================
    // 部品の種類
    //==============================
    [Serializable]
    enum PartType {
        Pillar,
        Sign,
        Overlay
    }
}