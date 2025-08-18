using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public Vector3 CameraStaticPos;
    public GameObject Camera;

    float hAxis;
    float vAxis;
    public float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
		hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

		gameObject.transform.position += new Vector3(hAxis, 0, vAxis) * moveSpeed * Time.deltaTime;

	}

	private void LateUpdate()
    {
        CameraMovement();
    }

    void CameraMovement()
    {
        Camera.transform.position = transform.position + CameraStaticPos;
    }

}
