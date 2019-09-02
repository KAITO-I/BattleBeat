using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystem : MonoBehaviour
{
    private static CoreSystem instance;

    [SerializeField] ControllerManager controllerManager;
    [SerializeField] SceneLoader       sceneLoader;
    [SerializeField] SoundManager      soundManager;

    private void Awake()
    {
        if (CoreSystem.instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        CoreSystem.instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.controllerManager.Init();
        this.sceneLoader.Init();
        this.soundManager.Init();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}