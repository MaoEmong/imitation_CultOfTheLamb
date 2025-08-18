using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 제작진 메뉴
public class CreditMenu : MonoBehaviour
{
	// 부모 객체
	public Menu MyParant;
   
	// 이니셜라이징
	public void Init(Menu _parant)
	{
		// 부모객체 받아오기
		MyParant = _parant;
	}

	// F키 입력 시 메뉴로 돌아가기
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			MyParant.CloseCreditMenu();
		}
	}

}
