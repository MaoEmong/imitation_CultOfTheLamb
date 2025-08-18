using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ksjInfoCard : MonoBehaviour
{
    private Item itemInfo;
    public Item ItemInfo
    {
        get { return itemInfo; }
        set { itemInfo = value; }
    }
    [SerializeField]
    private GameObject itemImage;
    [SerializeField]
    private GameObject itemName;
    [SerializeField]
    private GameObject itemDescription;
    [SerializeField]
    private GameObject itemRefer;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Sprite[] infoItemRect;

    [SerializeField]
    private GameObject infoRect;

    [SerializeField]
    private GameObject infoCrown;

    private void Start()
    {
        var image = itemImage.GetComponent<UnityEngine.UI.Image>();
        image.enabled = false;
    }

    public void UpdateInfo()
    {
        var image = itemImage.GetComponent<UnityEngine.UI.Image>();
        image.enabled = true;
        var name = itemName.GetComponent<TextMeshProUGUI>();
        var description = itemDescription.GetComponent<TextMeshProUGUI>();
        var refer = itemRefer.GetComponent<TextMeshProUGUI>();

        image.sprite = sprites[(int)ItemInfo.Code];
        name.text = ItemInfo.Name;
        description.text = ItemInfo.Description;
        refer.text = ItemInfo.Refer;
    }

    public void RefreshInfoRect(TapNum num)
    {
        if (num == TapNum.Tap1)
            infoRect.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 45);
        else
            infoRect.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
    }

    public void RefreshInfoCrown()
    { 

    }

}
