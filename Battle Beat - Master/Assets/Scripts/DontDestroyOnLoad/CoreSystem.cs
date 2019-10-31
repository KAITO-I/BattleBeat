using UnityEngine;

namespace CoreManager
{
    public class CoreSystem : MonoBehaviour
    {
        private static CoreSystem instance;

        [SerializeField] ControllerManager controllerManager;
        [SerializeField] SceneLoader sceneLoader;

        [Header("PopupManager")]
        [SerializeField]
        GameObject popup;

        PopupManager popupManager;

        void Awake()
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

            this.popupManager = new PopupManager(this.popup);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        void Update()
        {
            this.popupManager.Update();
        }
    }
}