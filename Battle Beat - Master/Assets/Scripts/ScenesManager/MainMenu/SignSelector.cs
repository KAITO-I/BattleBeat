using UnityEngine;

public class SignSelector : MonoBehaviour {
    private SpriteRenderer sign;

    const string selectedLayer = "HighlightUI";
    const string unselectedLayer = "Default";

    public void Init() {
        this.sign = this.transform.Find("Sign").GetComponent<SpriteRenderer>();
    }

    public void SignSelected() {
        this.sign.sortingLayerName = selectedLayer;
    }

    public void SignUnselected() {
        this.sign.sortingLayerName = unselectedLayer;
    }
}