using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameCamera : MonoBehaviour
{
    public static MainGameCamera _instance;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    Cinemachine.CinemachineTrackedDolly trackedDolly;
    [SerializeField]
    float startTime = 0.8f * 4;
    float time0 = 0f;
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        trackedDolly = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
    }
    public void GameStart()
    {
        trackedDolly.m_PathPosition = 0;
        StartCoroutine(track0());
    }
    IEnumerator track0()
    {
        while (time0 < startTime)
        {
            yield return new WaitForFixedUpdate();
            time0 += Time.deltaTime;
            float x = Mathf.Clamp(time0 / startTime,0,1);
            trackedDolly.m_PathPosition = x;
        }
    }
}
