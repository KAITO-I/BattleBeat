using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSkillMask : MonoBehaviour
{
    [SerializeField]
    GameObject Mask;
    public void setAvailable(bool Availability)
    {
        if (Availability)
        {
            Mask.SetActive(true);
        }
        else
        {
            Mask.SetActive(false);
        }
    }
}
