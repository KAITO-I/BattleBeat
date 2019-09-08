//==============================
// Created by akiirohappa
// Customized by KAITO-I
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//==============================
// Config操作／管理
//==============================
public class ConfigController : MonoBehaviour
{
    [SerializeField]
    private EventSystem es;
    [SerializeField]
    private Button button0;

    public void Show()
    {
        this.gameObject.SetActive(true);
        this.button0.Select();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ConfigUpdate()
    {
        switch (int.Parse(this.es.currentSelectedGameObject.name))
        {
            case 0:
        }
    }
}
