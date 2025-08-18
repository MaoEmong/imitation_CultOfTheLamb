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
    

    // 마우스가 현재 오브젝트의 onEnter상황에 호출되는 이벤트 메서드
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetInfo();

        SoundManager.Sound.Play("KSJ/UI/PointerOnUI");
    }


    /// <summary>
    /// 유저인벤토리에서 받은 아이템의 정보에맞춰 재화와 수량을 표시하는 메서드
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
    ///  아이템의 정보를 초기화시켜 인포카드에 반영하는 메서드
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
