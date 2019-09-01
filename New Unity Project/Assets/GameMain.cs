using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public GameObject cursor;
    LayerMask mask;
    void Start()
    {
        mask = LayerMask.GetMask(new string[] { "Board" });
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            cursor.transform.position = hit.point + Vector3.up * cursorOffset;

        }
    }
    BoardPos World2Board(Vector3 pos)
    {
        BoardPos dpos = new BoardPos();
        dpos.x = Mathf.FloorToInt(pos.x + 4f);

    }


}