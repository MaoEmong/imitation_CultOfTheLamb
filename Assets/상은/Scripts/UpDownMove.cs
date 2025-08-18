using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownMove : MonoBehaviour
{
    float moveTime;

    Vector3 moveVec;

	private void Start()
	{
		moveVec = new Vector3(transform.position.x, 1, transform.position.z);
	}

	void Update()
    {
		moveTime += Time.deltaTime;

		transform.position = Vector3.Lerp(transform.position, moveVec, 0.3f * Time.deltaTime);

        if (moveTime > 1.5f && moveVec.y == -1)
		{
			moveVec = new Vector3(transform.position.x, 1, transform.position.z);
			moveTime = 0;
		}
		else if (moveTime > 1.5f && moveVec.y == 1)
		{
			moveVec = new Vector3(transform.position.x, -1, transform.position.z);
			moveTime = 0;
		}
	}
}
