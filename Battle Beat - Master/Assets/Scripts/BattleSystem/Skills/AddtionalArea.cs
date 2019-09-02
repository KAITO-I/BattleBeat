using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AttackItemBase))]
public class AddtionalArea : MonoBehaviour
{
    public int ColIdx;

    private void Awake()
    {
        var area = GetComponent<AttackItemBase>().Area;
        
        for (int i = -1; i < 4; i++)
        {
            if(area.IndexOf(new Vector2Int(ColIdx, i)) == -1)
            {
                area.Add(new Vector2Int(ColIdx, i));
            }
        }
    }
    
}
