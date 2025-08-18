using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartObject : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Rigidbody rigid;

    bool isGround=false;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rigid = GetComponent<Rigidbody>();

        rigid.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
    }

    private void Update()
    {
        if (isGround)
            return;

        if (transform.position.y <= 0)
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


        Debug.Log("enter");

        var pm = ksjPlayerManager.Instance;
        pm.PlayerDamagedProcess(0.5f);

        Destroy(gameObject);
    }


}
