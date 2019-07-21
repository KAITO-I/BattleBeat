using UnityEngine;

public class ModeDescription : MonoBehaviour
{
    [SerializeField]
    private string title;
    public  string Title { get { return this.title; } }

    [SerializeField]
    private string description;
    public  string Description { get { return this.description; } }
}
