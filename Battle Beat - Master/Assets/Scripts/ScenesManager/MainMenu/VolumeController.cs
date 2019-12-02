using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MainMenu
{
    class VolumeController : MonoBehaviour
    {
        public Slider Slider { get; private set; }

        string     volumeName;
        AudioMixer audioMixer;
        Color      selectColor;
        Color      unselectColor;

        Text  percentText;
        Image sliderImage;

        public VolumeController(string volumeName, AudioMixer audioMixer, Slider slider, Color selectColor, Color unselectColor)
        {
            this.Slider = slider;

            this.volumeName    = volumeName;
            this.audioMixer    = audioMixer;
            this.selectColor   = selectColor;
            this.unselectColor = unselectColor;
            this.percentText   = slider.transform.Find("Handle Slide Area").Find("Handle").Find("Text").GetComponent<Text>();
            this.sliderImage   = slider.transform.Find("Handle Slide Area").Find("Handle").GetComponent<Image>();

            Select(false);
            SetVolume(PlayerPrefs.GetFloat("AudioVolume." + this.volumeName, 1f));
        }

        public void Select(bool select)
        {
            this.sliderImage.color = select ? this.selectColor : this.unselectColor;
        }

        public void AddVolume(float value)
        {
            SetVolume(this.Slider.value + value);
        }

        public void SubVolume(float value)
        {
            SetVolume(this.Slider.value - value);
        }

        void SetVolume(float linear)
        {
            var decibel = (linear > 0f) ? Mathf.Max(20 * Mathf.Log10(linear), -80) : 0f;

            this.audioMixer.SetFloat(this.volumeName, decibel);
            this.Slider.value = linear;
            this.percentText.text = ((int)Mathf.Lerp(0, 100, (this.Slider.value + 80) / 80)).ToString();
        }

        public void Save()
        {
            PlayerPrefs.GetFloat("AudioVolume." + this.volumeName, this.Slider.value);
        }
    }
}
