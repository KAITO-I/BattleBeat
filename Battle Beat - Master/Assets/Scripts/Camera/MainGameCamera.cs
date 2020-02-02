using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameCamera : MonoBehaviour
{
    public static MainGameCamera _instance;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera2;

    Cinemachine.CinemachineTrackedDolly trackedDolly;
    Cinemachine.CinemachineBasicMultiChannelPerlin noise;
    [SerializeField]
    float startTime = 0.8f * 4;
    float time0 = 0f;

    [SerializeField]
    float zoomTime=0.5f;
    [SerializeField]
    float zoomDistance=10f;
    [SerializeField]
    float shakeTimeAmp = 2f;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        trackedDolly = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
        noise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
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
    public void ChangeAndZoomUp(int playerId){
        Vector3 originPos = Camera.main.transform.position;
        GameObject loserObj = AttackManager._instance.GetPlayer(playerId).gameObject;
        GameObject loserCenter = new GameObject();
        loserCenter.transform.position = loserObj.transform.position + new Vector3(0f, 1f, 0f);
        virtualCamera2.LookAt = loserCenter.transform;
        virtualCamera2.transform.position = virtualCamera.transform.position;
        virtualCamera2.Priority = virtualCamera.Priority + 1;
        StartCoroutine(track1(originPos, loserCenter.transform.position-Vector3.Normalize(loserCenter.transform.position-originPos)* zoomDistance));
    }
    IEnumerator track1(Vector3 pos,Vector3 pos2)
    {
        time0 = 0;
        while (time0 < zoomTime)
        {
            
            yield return new WaitForFixedUpdate();
            time0 += Time.deltaTime;
            float x = Mathf.Clamp(time0 / zoomTime, 0, 1);
            Vector3 newPos = Vector3.Lerp(pos, pos2, x);
            virtualCamera2.transform.position = newPos;
        }
    }
    public void ShakeCamera()
    {
        StartCoroutine(shake());
    }
    public void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
    public IEnumerator shake()
    {
        Noise(1f, noise.m_FrequencyGain);
        yield return new WaitForSeconds(RythmManager.instance.getbps/ shakeTimeAmp);
        Noise(0f, noise.m_FrequencyGain);
    }
}
