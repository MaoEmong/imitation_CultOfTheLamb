using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    private ItemCode objectCode;

    private Rigidbody rigid;
    bool isGround = false;

    [SerializeField]
    private Sprite[] itemSprite;

    public ItemCode ObjectCode
    {
        get { return objectCode; }
        set { objectCode = value; }
    }

    [SerializeField]
    private int count;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }
    private BoxCollider collider;

    private void Start()
    {
        SetItem();
        collider= GetComponent<BoxCollider>();
        rigid = transform.GetComponent<Rigidbody>();        
        rigid.AddForce(new Vector3(0,7, 0), ForceMode.Impulse);

        SoundManager.Sound.Play("Object/Chest/Master.assets [740]");
    }

    private void Update()
    {
        if (isGround)
            return;

        if(transform.position.y<=0)
        {
            transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            isGround = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어가 아니면 return
        if (collision.gameObject.tag != "Player")
            return;

        GetItem(objectCode, count);
    }

    private void GetItem(ItemCode code,int count)
    {
        var pm = ksjPlayerManager._instance;
        pm.AddItemToInven(objectCode, count);

        Destroy(gameObject);
    }

    private void SetItem()
    {
        switch(ObjectCode)
        {
            case ItemCode.Gold:
                transform.GetComponent<SpriteRenderer>().sprite = itemSprite[(int)ItemCode.Gold];
               break;
            case ItemCode.Grass:
                transform.GetComponent<SpriteRenderer>().sprite = itemSprite[(int)ItemCode.Grass];
                break;
        }
    }
}
