using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowImage : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    float interval = 0.2f;
    float displayTime = 1f;
    bool isEnd = false;
    int index;
    Coroutine coroutine=null;
    List<Sprite> sprites=new List<Sprite>();
    public static ShowImage _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void ShowImages(string[] textureNames,float time = 0.5f,float time2=0.1f)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        displayTime = time;
        interval = time2;
        index = 0;
        sprites.Clear();
        Load(textureNames);
        coroutine = StartCoroutine(ShowImageMain());
    }

    public bool IsEnd()
    {
        return isEnd;
    }
    IEnumerator ShowImageMain()
    {
        isEnd = false;
        float zeroTime = 0f;
        float time = displayTime;
        bool changeFlag = true;
        while (true)
        {
            while (zeroTime > time)
            {
                zeroTime -= time;
                index++;
                if (index % 2 == 0)
                {
                    time = displayTime;
                }
                else
                {
                    time = interval;
                }
                Debug.Log(Time.time);
                changeFlag = true;
            }
            if(index / 2 >= sprites.Count)
            {
                image.gameObject.SetActive(false);
                break;
            }
            if (changeFlag)
            {
                if (index % 2 == 0)
                {
                    image.rectTransform.sizeDelta = new Vector2(sprites[index / 2].rect.width, sprites[index / 2].rect.height);
                    image.sprite = sprites[index / 2];
                    image.gameObject.SetActive(true);
                }
                else
                {
                    image.gameObject.SetActive(false);
                }
                changeFlag = false;
            }
            zeroTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isEnd = true;
    }

    private void Load(string[] textureName)
    {
        foreach(string str in textureName)
        {
            Texture2D texture = Resources.Load("Images/" + str) as Texture2D;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            sprites.Add(sprite);
        }
    }
}
