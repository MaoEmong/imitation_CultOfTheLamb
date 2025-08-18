using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjPlayerMove : MonoBehaviour
{

    [SerializeField]
    float moveSpeed = 5;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); 
        float y = Input.GetAxisRaw("Vertical");

        var dir = new Vector3(x, y, 0);
        dir = dir.normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

    }


}
