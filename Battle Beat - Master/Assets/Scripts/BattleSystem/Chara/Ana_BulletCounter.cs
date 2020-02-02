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
    List<GameObject> bullets;

    int PlayerId;
    float _offset;
    // Start is called before the first frame update
    public void Init(int PlayerId,float offset,int maxBullets)
    {
        this.PlayerId = PlayerId;
        string ObjName = exSlotName[PlayerId - 1];
        GameObject exSlotObj = GameObject.Find(ObjName);
        ExSlot = exSlotObj.transform;
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
