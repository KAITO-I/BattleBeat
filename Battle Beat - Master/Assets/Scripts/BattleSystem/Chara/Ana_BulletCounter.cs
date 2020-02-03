using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ana_BulletCounter : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;

    Transform ExSlot;
    [SerializeField]
    float offset;
    [SerializeField]
    string[] exSlotName;
    [SerializeField]
    GameObject backGround;
    List<GameObject> bullets;

    int PlayerId;
    float _offset;
    // Start is called before the first frame update
    public void Init(int PlayerId,float offset,int maxBullets)
    {
        this.PlayerId = PlayerId;
        string ObjName = exSlotName[PlayerId - 1];
        GameObject exSlotObj = GameObject.Find(ObjName);
        GameObject backGroundObj = Instantiate<GameObject>(backGround);
        
        ExSlot = exSlotObj.transform;
        backGroundObj.transform.parent = ExSlot;
        backGroundObj.transform.localPosition = Vector3.zero;
        if (PlayerId == 2)
        {
            backGroundObj.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        _offset = PlayerId==1?offset:-offset;
        bullets = new List<GameObject>();
        for(int i = 0; i < maxBullets; i++)
        {
            AddBullet();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddBullet()
    {
        GameObject b = Instantiate<GameObject>(bullet, ExSlot);
        b.transform.localPosition += new Vector3(_offset*bullets.Count, 0, 0);
        bullets.Add(b);
    }
    public void SubBullet()
    {
        int idx = bullets.Count - 1;
        GameObject bullet = bullets[idx];
        bullets.RemoveAt(idx);
        Destroy(bullet);
    }
}
