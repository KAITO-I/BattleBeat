//==============================
// Created by KAITO-I.
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TitleState
{
    MainMenu, // モード選択画面
    Config,   // コンフィグ
    Credit    // クレジット
}

//==============================
// タイトル管理
//==============================
public class TitleManager : MonoBehaviour
{
    private TitleState state;

    void Start()
    {
        
    }

    void Update()
    {
        switch (state)
        {
            case TitleState.MainMenu:

        }
    }
}
