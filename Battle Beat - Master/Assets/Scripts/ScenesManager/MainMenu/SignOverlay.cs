using UnityEngine;

public class SignOverlay : MonoBehaviour {
    private GameObject overlay;

    public void Init() {
        this.overlay = this.transform.Find("Overlay").gameObject;
    }

    public void SignSelected() {
        this.overlay.SetActive(false);
    }

    public void SignUnselected() {
        this.overlay.SetActive(true);
    }
}