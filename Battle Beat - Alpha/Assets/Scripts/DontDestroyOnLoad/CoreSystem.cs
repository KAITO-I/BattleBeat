using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystem : MonoBehaviour
{
    private static CoreSystem instance;

    [SerializeField] SoundManager soundManager;
    [SerializeField] SceneLoader sceneLoader;

    private void Awake()
    {
        if (CoreSystem.instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        CoreSystem.instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.soundManager.Init();
        this.sceneLoader.Init();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) ;
    }
}