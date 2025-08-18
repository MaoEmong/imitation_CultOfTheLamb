using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Chest_AnimScript anim;

	private void Start()
	{
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Y))
		{
			anim.animState = Chest_AnimScript.AnimState.REVEAL;
			anim.isHide = false;
		}

	}
}
