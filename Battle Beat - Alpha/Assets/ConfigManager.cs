//==============================
// Created    by akiirohappa (木原)
// Customized by KAITO-I     (稲福)
//==============================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==============================
// Config操作／管理
//==============================
public class ConfigManager : MonoBehaviour
{
    [SerializeField] float defaultSoundVolume;
    [SerializeField] Slider[] Sliders;
    float BGM;
    float SE;

    void Start()
    {

        BGM = LoadVol("BGM");
        SE = LoadVol("SE");
        Sliders[0].value = BGM;
        Sliders[1].value = SE;
    }

    public void SetBGMVol(float f)
    {
        BGM = f;
        SaveVol(BGM, "BGM");
    }
    public void SetSEVol(float f)
    {
        SE = f;
        SaveVol(SE, "SE");
    }
    public void SaveVol(float val,string key)
    {
        PlayerPrefs.SetFloat(key,val);
        PlayerPrefs.Save();
    }
    public float LoadVol(string key)
    {
        return PlayerPrefs.GetFloat(key, 0.5f);
    }
}
