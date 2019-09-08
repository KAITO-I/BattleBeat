using System;
using UnityEngine;

namespace MainMenu
{
    enum DisplayState {
        Menu,
        Config,
        Tutorial,
        Credit
    }

    enum MegaphoneTreeState
    {
        Left,
        Right,
        Changing
    }

    enum MTUIType
    {
        Tree,
        Sign,
        Overlay
    }

    [Serializable]
    class MegaphoneTreeUI
    {
        [SerializeField]
        private MTUIType type;

        [SerializeField]
        private GameObject gameObject;
    }
}
