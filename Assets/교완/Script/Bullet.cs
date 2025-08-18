using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletspeed;



    GameObject player;

    Enemy enemy;

    Rigidbody bulletRigidbody;

    Vector3 dir;

    void Start()
    {
        player = GameObject.Find("Player");

        enemy = GetComponentInParent<Enemy>();

        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = bulletspeed * transform.right;
        //transform.right�� bullet�� ������ �������� ����� �ǹ�

        dir = player.transform.position - transform.position;

    }

    // Update is called once per frame

    private void Update()
    {


        transform.position += transform.forward * bulletspeed * Time.deltaTime;
        Destroy(gameObject, 5.0f);




    }

    public void OneShot(Vector3 dir)
    {

    }
}
