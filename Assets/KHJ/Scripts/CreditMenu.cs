using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ �޴�
public class CreditMenu : MonoBehaviour
{
	// �θ� ��ü
	public Menu MyParant;
   
	// �̴ϼȶ���¡
	public void Init(Menu _parant)
	{
		// �θ�ü �޾ƿ���
		MyParant = _parant;
	}

	// FŰ �Է� �� �޴��� ���ư���
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			MyParant.CloseCreditMenu();
		}
	}

}
