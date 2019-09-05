using UnityEngine;

public class SoundEffect : MonoBehaviour {
    private AudioSource audio;

    private void Start() {
        this.audio = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip) {
        this.audio.clip = clip;
        this.audio.Play();
        Invoke("EndCheck", clip.length);
    }

    private void EndCheck() {
        
    }
}