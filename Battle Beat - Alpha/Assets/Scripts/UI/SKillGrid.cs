using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SKillGrid : MonoBehaviour
{
    [SerializeField]
    Image imageCoolDown;
    [SerializeField]
    Image imageOnUse;
    [SerializeField]
    Image image;

    int TurnMax = 1;
    private void Start()
    {
        imageCoolDown.type = Image.Type.Filled;
        imageCoolDown.fillMethod = Image.FillMethod.Vertical;
        imageCoolDown.fillOrigin = 0;
    }


    public void Init(int Turn,Sprite mainSprite)
    {
        imageOnUse.gameObject.SetActive(false);
        imageCoolDown.fillAmount = 0f;
        image.sprite = mainSprite;
        TurnMax = Turn;
    }

    public void SetTurn(int Turn)
    {
        if (Turn >= 0 && TurnMax >= 0)
        {
            imageCoolDown.fillAmount = ((float)Turn) / ((float)TurnMax);
        }
    }
    public void SetOnUse(bool active)
    {
        if (active == imageOnUse.gameObject.active)
        {
            return;
        }
        if (active)
        {
            imageOnUse.gameObject.SetActive(true);
        }
        else
        {
            imageOnUse.gameObject.SetActive(false);
        }
    }
}
