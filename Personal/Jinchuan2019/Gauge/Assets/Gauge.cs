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

    public void SetMaxValue(float value)
    {
        MaxValue = value;
        CurrentValueR = MaxValue;
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
    public void Reset100()
    {
        SetMaxValue(100);
        SetCurrentValue(100);
    }
    public void Reset0()
    {
        SetMaxValue(100);
        SetCurrentValue(0);
    }
    public void Reduce10()
    {
        SetCurrentValue(CurrentValue - 10);
    }
    public void Add10()
    {
        SetCurrentValue(CurrentValue + 10);
    }
}