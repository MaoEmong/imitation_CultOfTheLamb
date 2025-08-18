using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelongingsPanel : MonoBehaviour
{
    // �� �ڿ�Ÿ���� �г�
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


    // �κ��丮���� ǥ�õ� ������
    [SerializeField]
    private GameObject itemPrefab;

    private void Start()
    {
    }


    /// <summary>
    /// ����ǰ�� �������� �����ϴ� ȣ�� �޼���
    /// </summary>dd
    public void SetItemList()
    {
        var pmInstance = ksjPlayerManager._instance;

        // �κ��丮 ��ü�� ��ȸ�ؼ� �� �ε����� �ʵ带 ��ȸ�Ͽ� �ش��ϴ� 
        // ��Ͽ� ���缭 ��ġ�� ���ش�.
        for (int i = 0; i < pmInstance.inventory.Count; i++)
        {
            Debug.Log("����Ʈ �ε��� : "+i + "�߰�");
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
            // ������ ������ ������Ʈ�� ��ȸ�� �ε����� ������ ������ ����
            var item = obj.GetComponent<ksjItem>();
            item.ItemInfo = pmInstance.inventory[i];
            item.SetItem();
        }
    }


    /// <summary>
    /// �κ��丮 �������� �ֽ�ȭ�� ���� Ŭ���� �޼���. ������ �� ���� ȣ��
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
