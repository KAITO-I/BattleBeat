//==============================
// Created by akiirohappa
// Customized by KAITO-I
//==============================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

//==============================
// Config操作／管理
//==============================
public class ConfigManager : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField]
    private AudioMixer gameAudioMixer;
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider seSlider;

    int nowSelect;
    private void Start()
    {
        /*nowSelect = 0;
        SoundVolume.MasterVolume.Select();*/
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
    }

    private void Update()
    {
        /*controller = ControllerManager.Instance;
        float v = controller.GetAxis_Menu(ControllerManager.Axis.DpadY);
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
        if(controller.GetButtonDown_Menu(ControllerManager.Button.B))
        {
            LoadMainMenu();
        }*/
    }
    //------------------------------
    // 音量値設定反映
    //------------------------------
    // [引数]
    // float value : スライダーの値
    //------------------------------
    public void SetMasterVol(float value)
    {
        this.gameAudioMixer.SetFloat("MasterVol", Mathf.Lerp(0f, -80f, value / 100f));
    }

    public void SetBGMVol(float value)
    {
        this.gameAudioMixer.SetFloat("BGMVol", Mathf.Lerp(0f, -80f, value / 100f));
    }

    public void SetSEVol(float value)
    {
        this.gameAudioMixer.SetFloat("SEVol", Mathf.Lerp(0f, -80f, value / 100f));
    }
}

