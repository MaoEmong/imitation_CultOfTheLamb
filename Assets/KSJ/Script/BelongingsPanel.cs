using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelongingsPanel : MonoBehaviour
{
    // 각 자원타입의 패널
    [SerializeField]
    private GameObject resourcePanel;
    [SerializeField]
    private GameObject foodPanel;
    [SerializeField]
    private GameObject objectPanel;
    [SerializeField]
    private GameObject infoCard;
    public GameObject InfoCard
    {
        get { return infoCard; }
    }


    // 인벤토리에서 표시될 아이템
    [SerializeField]
    private GameObject itemPrefab;

    private void Start()
    {
    }


    /// <summary>
    /// 소지품의 아이템을 갱신하는 호출 메서드
    /// </summary>dd
    public void SetItemList()
    {
        var pmInstance = ksjPlayerManager._instance;

        // 인벤토리 전체를 조회해서 각 인덱스의 필드를 조회하여 해당하는 
        // 목록에 맞춰서 배치를 해준다.
        for (int i = 0; i < pmInstance.inventory.Count; i++)
        {
            Debug.Log("리스트 인덱스 : "+i + "추가");
            GameObject obj;
            obj = Instantiate(itemPrefab);            
            switch (pmInstance.inventory[i].ItemType)
            {
                case ItemType.Resource:
                    obj.transform.parent = resourcePanel.transform;                    
                    break;
                case ItemType.Food:
                    obj.transform.parent = foodPanel.transform;
                    break; 
                case ItemType.Object:
                    obj.transform.parent = objectPanel.transform;
                    break;
            }
            // 생성한 아이템 오브젝트에 조회한 인덱스의 아이템 정보를 전달
            var item = obj.GetComponent<ksjItem>();
            item.ItemInfo = pmInstance.inventory[i];
            item.SetItem();
        }
    }


    /// <summary>
    /// 인벤토리 아이템의 최신화를 위한 클리어 메서드. 갱신할 때 먼저 호출
    /// </summary>
    public void ClearItemList()
    {
        var child = resourcePanel.GetComponentsInChildren<Transform>();
        var child2 = foodPanel.GetComponentsInChildren<Transform>();
        var child3 = objectPanel.GetComponentsInChildren<Transform>();

        foreach(var iter in child)
        {
            if(iter != resourcePanel.transform)
            {
                Destroy(iter.gameObject);
            }
        }

        foreach(var iter in child2)
        {
            if(iter != foodPanel.transform)
            {
                Destroy(iter.gameObject);
            }
        }

        foreach(var iter in child3)
        {
            if(iter != objectPanel.transform)
            {
                Destroy(iter.gameObject);
            }
        }

    }


}
