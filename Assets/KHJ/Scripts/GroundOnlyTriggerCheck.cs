using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 필드로 들어왔는지 체크하기 위한 Trigger
public class GroundOnlyTriggerCheck : MonoBehaviour
{
    public Field Parant;

	private void Start()
	{
		// Field 정보 가져오기
		Parant = transform.GetComponentInParent<Field>();
	}

	// Field의 문 제어
	void Update()
    {
        //if (Parant.isCleared)
           // Parant.OpenDoor();
    }

	// 충돌 검사
	private void OnTriggerEnter(Collider other)
	{
		// 플레이어 충돌 시
		if (other.CompareTag("Player"))
		{
			Debug.Log($"{Parant.isCleared}");
			// 현재 방이 클리어 되지 않았다면
			if (!Parant.isCleared)
			{
				// 문닫기
				Parant.CloseDoor();
				Parant.isStartBattle = true;
				// 몬스터 소환 //=========== 수정 - Field에서 소환할 것
				// StartCoroutine(Parant.SummonEnemy());
			}
		}
	}
}
