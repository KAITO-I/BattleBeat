using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeDescription : MonoBehaviour
{
    [SerializeField] string text;
    public string Text { get { return this.text; } }
}
