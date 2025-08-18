using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ksjItem : MonoBehaviour , IPointerEnterHandler
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
    private GameObject itemCount;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private GameObject info;
    

    // ���콺�� ���� ������Ʈ�� onEnter��Ȳ�� ȣ��Ǵ� �̺�Ʈ �޼���
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetInfo();

        SoundManager.Sound.Play("KSJ/UI/PointerOnUI");
    }


    /// <summary>
    /// �����κ��丮���� ���� �������� ���������� ��ȭ�� ������ ǥ���ϴ� �޼���
    /// </summary>
    public void SetItem()
    {
        var image = itemImage.GetComponent<UnityEngine.UI.Image>();
        var count = itemCount.GetComponent<TextMeshProUGUI>();

        switch(itemInfo.Code)
        {
            case ItemCode.Gold:
                image.sprite = sprites[(int)ItemCode.Gold];                
                break;
            case ItemCode.Grass:
                image.sprite = sprites[(int)ItemCode.Grass];
                break;

        }
     
        count.text = itemInfo.Count.ToString();
    }

    /// <summary>
    ///  �������� ������ �ʱ�ȭ���� ����ī�忡 �ݿ��ϴ� �޼���
    /// </summary>
    public void SetInfo()
    {
        var parent = transform.GetComponentInParent<BelongingsPanel>();
        if (parent == null)
            Debug.Log("nul");
        var infoCard = parent.InfoCard.GetComponent<ksjInfoCard>();

        infoCard.ItemInfo = itemInfo;

        infoCard.UpdateInfo();

    }
   

}
