using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] AudioClip sound;
    private void Start()
    {
        SoundManager.Instance.PlayBGM(sound);
    }
}
