using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteResult : MonoBehaviour
{
    public enum type
    {
        GREAT,
        MISS,
        NONE
    }
    Image image;
    [SerializeField]
    Sprite greatSprite;
    [SerializeField]
    Sprite missSprite;
    [SerializeField]
    float t0;
    [SerializeField]
    float t1;
    [SerializeField]
    float height;

    Transform tPlayer;
    public void Show(type _type,Transform player)
    {
        Image image = GetComponent<Image>();
        switch (_type)
        {
            case type.GREAT:
                image.sprite = greatSprite;
                break;
            case type.MISS:
                image.sprite = missSprite;
                break;
            case type.NONE:
                break;
        }
        transform.parent = GameObject.Find("Canvas").transform;
        tPlayer = player;
        StartCoroutine(enumerator());

    }
    IEnumerator enumerator()
    {
        Image image = GetComponent<Image>(); 
        float t = 0;
        Color c = image.color;
        c.a = 0;
        image.color = c;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            t += Time.deltaTime;
            if (t < t0)
            {
                transform.position = Camera.main.WorldToScreenPoint(new Vector3(0f, 1f, 0f) + tPlayer.position) + (t / t0) * Vector3.up*height;
                c.a = t / t0;
                image.color = c;
            }
            else
            {
                yield return new WaitForSeconds(t1);
                break;
            }

        }
        Destroy(gameObject);
        
    }
}
