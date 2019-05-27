using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNum
{
    oneP,
    twoP,
}
public class PlayerData : MonoBehaviour
{
    [SerializeField]PlayerNum Num;
    public PlayerNum myPl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("A_1P") && myPl == PlayerNum.oneP)
        {
            Debug.Log("plessd" + ((Num == PlayerNum.oneP) ? "1P" : "2P"));
        }
        if (Input.GetButtonDown("A_2P") && myPl == PlayerNum.twoP)
        {
            Debug.Log("plessd" + ((Num == PlayerNum.oneP) ? "1P" : "2P"));
        }
    }
}
