using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType{
    PanelA,
    PanelB
}

public class Panel : MonoBehaviour
{
    [SerializeField] private PanelType panelType;
    public PanelType PanelType { get { return this.panelType; } }
    [SerializeField] private Material on;
    [SerializeField] private Material off;
    public void SetEnable(bool enable)
    {
        GetComponent<Renderer>().material = enable ? on : off;
    }
}
