//==============================
// Created by akiirohappa
// Customized by KAITO-I
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
    [SerializeField] VolumeConfig SoundVolume;
    private ControllerManager controller;
    int nowSelect;
    private void Start()
    {
        this.SoundVolume.Init();
        nowSelect = 0;
        SoundVolume.MasterVolume.Select();
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
    }

    private void Update()
    {
        controller = ControllerManager.Instance;
        float v = controller.GetAxis(ControllerManager.Axis.DpadY);
        if(v >= 0)
        {
            if(nowSelect != 2)nowSelect++;
            Debug.Log("↑");
        }
        if (v <= 0)
        {
            if (nowSelect != 0) nowSelect--;
            Debug.Log("↓");
        }
        switch (nowSelect)
        {
            case 0:
                SoundVolume.MasterVolume.Select();
                break;
            case 1:
                SoundVolume.BGMVolume.Select();
                break;
            case 2:
                SoundVolume.SEVolume.Select();
                break;
            default:
                break;
        }
        if(controller.GetButtonDown(ControllerManager.Button.B))
        {
            LoadMainMenu();
        }
    }
    //------------------------------
    // 音量値設定反映
    //------------------------------
    // [引数]
    // float value : スライダーの値
    //------------------------------
    public void SetMasterVol(float value)
    {
        this.SoundVolume.SoundManager.MasterVolume = value;
        this.SoundVolume.MasterText.text = this.SoundVolume.VolumeDisplayCalc(value).ToString();
    }

    public void SetBGMVol(float value)
    {
        this.SoundVolume.SoundManager.BGM.Volume = value;
        this.SoundVolume.BGMText.text = this.SoundVolume.VolumeDisplayCalc(value).ToString();
    }

    public void SetSEVol(float value)
    {
        this.SoundVolume.SoundManager.SE.Volume = value;
        this.SoundVolume.SEText.text = this.SoundVolume.VolumeDisplayCalc(value).ToString();
    }

    //==============================
    // Volume値設定
    //==============================
    [System.Serializable]
    class VolumeConfig
    {
        public SoundManager SoundManager { get; private set; }
        [SerializeField] public Slider MasterVolume;
        [SerializeField] public Slider BGMVolume;
        [SerializeField] public Slider SEVolume;
        public Text MasterText;
        public Text BGMText;
        public Text SEText;
        //------------------------------
        // 初期化
        //------------------------------
        public void Init()
        {
            this.SoundManager = SoundManager.Instance;
            this.MasterVolume.value = this.SoundManager.MasterVolume;
            this.BGMVolume.value    = this.SoundManager.BGM.Volume;
            this.SEVolume.value     = this.SoundManager.SE.Volume;
            this.MasterText.text = VolumeDisplayCalc(this.SoundManager.MasterVolume).ToString(); ;
            this.BGMText.text = VolumeDisplayCalc(this.SoundManager.BGM.Volume).ToString(); ;
            this.SEText.text = VolumeDisplayCalc(this.SoundManager.SE.Volume).ToString(); ;
        }
        public int VolumeDisplayCalc(float vol)
        {
            return (int)(vol * 100);
        }
    }


}

