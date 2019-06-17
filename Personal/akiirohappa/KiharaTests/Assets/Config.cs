using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    [SerializeField] float BGM;
    [SerializeField] float SE;
    [SerializeField] Slider[] Sliders;
    // Start is called before the first frame update
    void Start()
    {
        BGM = LoadVol("BGM");
        SE = LoadVol("SE");
        Sliders[0].value = BGM;
        Sliders[1].value = SE;
    }

    // Update is called once per frame
    void Update()
    {
        
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
