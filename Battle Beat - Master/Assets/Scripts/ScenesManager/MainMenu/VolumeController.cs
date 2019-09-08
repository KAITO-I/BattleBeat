using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MainMenu
{
    class VolumeController : MonoBehaviour
    {
        public Slider Slider { get; private set; }

        private string     volumeName;
        private AudioMixer audioMixer;
        private Text       percentText;
        private Image      sliderImage;

        public VolumeController(string volumeName, AudioMixer audioMixer, Slider slider, Color unselectColor)
        {
            this.Slider = slider;

            this.volumeName  = volumeName;
            this.audioMixer  = audioMixer;
            this.percentText = slider.transform.Find("Handle Slide Area").Find("Handle").Find("Text").GetComponent<Text>();
            this.sliderImage = slider.transform.Find("Handle Slide Area").Find("Handle").GetComponent<Image>();

            SetColor(unselectColor);
        }

        public void SetVolume(float volume)
        {
            this.audioMixer.SetFloat(this.volumeName, volume);
            this.Slider.value     = volume;
            this.percentText.text = ((int)Mathf.Lerp(0, 100, (this.Slider.value + 80) / 80)).ToString();
        }

        public void SetColor(Color color)
        {
            this.sliderImage.color = color;
        }
    }
}
