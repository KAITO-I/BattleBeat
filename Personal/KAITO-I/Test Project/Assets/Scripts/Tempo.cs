using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tempo : MonoBehaviour
{
    [SerializeField] float bpm;
    public float BPM { get { return bpm; } }
    private float bps;

    private float time;

    [SerializeField] StageManager stagemgr;
    PanelType panelType;

    private Coroutine coroutine;
    private bool enable;

    private void Start()
    {
        this.bps = 60f / bpm;
        this.time = 0f;

        coroutine = null;
        panelType = PanelType.PanelA;
        stagemgr.TurnOn(panelType);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*if (this.coroutine == null)
            {
                this.coroutine = StartCoroutine(Rythm());
            } else
            {
                StopCoroutine(this.coroutine);
                this.coroutine = null;
                panelType = PanelType.PanelA;
                stagemgr.TurnOn(PanelType.PanelA);
            }*/


            this.enable = !this.enable;

            if (!this.enable)
            {
                panelType = PanelType.PanelA;
                stagemgr.TurnOn(PanelType.PanelA);
            }
        }

        if (this.enable)
        {
            this.time += Time.deltaTime;
            if (this.time >= this.bps)
            {
                this.time -= this.bps;

                panelType = panelType == PanelType.PanelA ? PanelType.PanelB : PanelType.PanelA;
                stagemgr.TurnOn(panelType);
            }
        }
    }

    private IEnumerator Rythm()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.bps);

            panelType = panelType == PanelType.PanelA ? PanelType.PanelB : PanelType.PanelA;
            stagemgr.TurnOn(panelType);
        }
    }
}
