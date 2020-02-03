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
        [SerializeField]
        Vector2 upCalloutSelectLocalPos;
        [SerializeField]
        Vector2 upCalloutUnelectLocalPos;
        [SerializeField]
        Vector2 downCalloutSelectLocalPos;
        [SerializeField]
        Vector2 downCalloutUnelectLocalPos;
        [SerializeField]
        Vector2 unselectLocalScale;

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


            PopupManager.Init(
                this.popup,
                (this.upCalloutSelectLocalPos, this.upCalloutUnelectLocalPos),
                (this.downCalloutSelectLocalPos, this.downCalloutUnelectLocalPos),
                this.unselectLocalScale
            );


            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

        }

        void LateUpdate()
        {
            PopupManager.LateUpdate();
        }
    }
}