using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private Panel[,] stage;
    PanelType panelType;

    private void Start()
    {
        this.stage = new Panel[3,3];
        for (int j = 0; j < 3; j++)
        {
            Transform tf = transform.Find(j.ToString());
            for (int i = 0; i < 3; i++)
                this.stage[j, i] = tf.Find(i.ToString()).gameObject.GetComponent<Panel>();
        }

        this.panelType = PanelType.PanelA;
    }

    public void TurnOn(PanelType panelType)
    {
        for (int j = 0; j < 3; j++) {
            for (int i = 0; i < 3; i++)
            {
                Panel panel = this.stage[j, i];
                panel.SetEnable(panel.PanelType == panelType);
            }
        }
    }

    public void Change()
    {
        TurnOn(this.panelType = (this.panelType == PanelType.PanelA ? PanelType.PanelB : PanelType.PanelA));
    }
}
