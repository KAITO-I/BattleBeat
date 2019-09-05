using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Gauge : MonoBehaviour
{
    public enum Direction
    {
        RightToLeft,
        LeftToRight
    }
    [SerializeField]
    Image image;
    [SerializeField]
    Direction direction;
    [SerializeField]
    float MaxValue;
    [SerializeField]
    float CurrentValue;
    [SerializeField]
    bool ReduceByTime;
    [SerializeField]
    float WaitTime;
    [SerializeField]
    float Speed;
    Coroutine coroutineReduce;
    float CurrentValueR;
    float CurrentValueR2;
    private void Start()
    {
        image.type = Image.Type.Filled;
        SetDirection();
    }

    private void SetDirection()
    {
        image.fillMethod = Image.FillMethod.Horizontal;
        switch (direction)
        {
            case Direction.LeftToRight:
                image.fillOrigin = 0;
                break;
            case Direction.RightToLeft:
                image.fillOrigin = 1;
                break;
        }
    }

    public void Init(float MaxValue,float StartPercent)
    {
        this.MaxValue = MaxValue;
        CurrentValueR = this.MaxValue * StartPercent;
        CurrentValue = CurrentValueR;
        image.fillAmount = Mathf.Clamp(CurrentValue / MaxValue, 0, 1);
    }

    public void SetCurrentValue(float value)
    {
        CurrentValue = Mathf.Clamp(value, 0, MaxValue);
        UpDateGauge();
    }
    void UpDateGauge()
    {
        if (ReduceByTime)
        {
            if (coroutineReduce != null)
            {
                StopCoroutine(coroutineReduce);
            }
            CurrentValueR2 = CurrentValue;
            coroutineReduce = StartCoroutine(Reduce());
        }
        else
        {
            image.fillAmount = Mathf.Clamp(CurrentValue / MaxValue, 0, 1);
        }
    }
    IEnumerator Reduce()
    {
        yield return new WaitForSeconds(WaitTime);
        while (CurrentValueR > CurrentValueR2)
        {
            CurrentValueR -= Speed * MaxValue *Time.deltaTime;
            image.fillAmount = Mathf.Clamp(CurrentValueR / MaxValue, 0, 1);
            if (CurrentValueR < CurrentValueR2)
            {
                CurrentValueR = CurrentValueR2;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}