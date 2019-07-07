using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    Renderer renderer;
    public enum Colors
    {
        deeppink = 1,
        skyblue = 2,
        red = 4,
        blue = 8
    }
    uint Flags = 0x00;
    Color[] ColorTable =
    {
         new Color(0xff/255f, 0x14/255f, 0x93/255f),
         new Color(0x87/255f,0xce/255f,0xeb/255f),
         Color.red,
         Color.blue
    };
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        Flags = 0x00;
    }
    public void AddColor(Colors colors)
    {
        Flags = Flags | (uint)colors;
        AppleChange();
    }
    public void SubColor(Colors colors)
    {
        Flags = Flags & (~(uint)colors);
        AppleChange();
    }
    void AppleChange()
    {
        int c_idx = 0;
        int c_cnt = 0;
        Color temp = Color.black;
        renderer.material.color = Color.white;
        for (uint i = 1; i < 9; i *= 2)
        {
            if ((i & Flags) == i)
            {
                temp = temp + ColorTable[c_idx];
                c_cnt++;
            }
            c_idx++;
        }
        if (c_cnt == 0)
        {
            temp = Color.white;
        }
        else
        {
            temp /= c_cnt;
        }
        renderer.material.color = temp;
    }
}
