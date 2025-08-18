using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackLogic : MonoBehaviour
{
	// 마우스 좌표
	Vector3 MousePos;

	// 뒤집힌 좌표 구하기
	public GameObject player;

	public MeshCollider atkRange;
	public SkeletonAnimation skeletonAnimation;

	// 벡터의 x, z
	float x;
	float z;

	// 각도값
	float dgr;

	// 공격 상태 확인 변수
	bool nowAttack;

	public bool onRight;

	void Update()
	{
		moveAttackPos();
		if (!player.GetComponent<PlayerMove>().isdash)
			AttackNow();
	}

	// 공격방향 제어 메서드
	void moveAttackPos()
	{
		// 화면상의 마우스 좌표 획득
		MousePos = Input.mousePosition;

		// 화면 좌표를 기반으로 화면 정중앙에서 현재 마우스 위치에 대한 좌표값
		x = MousePos.x - 960;
		z = MousePos.y - 540;

		if (player.GetComponent<PlayerMove>().isFlip)
		{
			// 역함수를 이용해 마우스 포인트 위치 특정
			dgr = Mathf.Atan2(-z, -x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(90, -dgr, 180);
		}
		else
		{
			// 역함수를 이용해 마우스 포인트 위치 특정
			dgr = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(90, -dgr, 0);
		}

		// 마우스 위치를 기반으로 공격좌표를 담당할 오브젝트의 위치를 변경
		transform.localPosition = new Vector3(
						MathF.Cos(dgr * Mathf.Deg2Rad) * 2f,
						0,
						MathF.Sin(dgr * Mathf.Deg2Rad) * 2f);
	}
	
	// 공격 메서드
	void AttackNow()
	{
		// 좌클릭 시
		if (Input.GetMouseButtonDown(0) && !nowAttack)
		{
			// 공격 시작
			nowAttack = true;

			// 공격 방향에 맞춰서 플레이어 시야 방향 조정
			if (x > 0)
			{
				gameObject.transform.parent.transform.rotation = Quaternion.Euler(0, 180, 0);
				gameObject.transform.parent.GetComponent<PlayerMove>().isFlip = true;
				onRight = true;
			}
			else if (x < 0)
			{
				gameObject.transform.parent.transform.rotation = Quaternion.Euler(0, 0, 0);
				gameObject.transform.parent.GetComponent<PlayerMove>().isFlip = false;
				onRight = false;
			}
		}

		if (player.GetComponentInChildren<Player_AnimScript>().myWeapon != WeaponType.AXE)
		{
			// 공격 유효시간 검사
			if (nowAttack && (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.2f &&
							  skeletonAnimation.state.GetCurrent(0).AnimationTime < 0.3f))
			{
				if (!atkRange.enabled)
					// 공격 콜라이더 활성화
					atkRange.enabled = true;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.3f)
			{
				if (atkRange.enabled)
					// 유효시간이 지나면 공격 상태를 해제
					atkRange.enabled = false;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime ==
							 skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				nowAttack = false;
		}
		else
		{
			// 공격 유효시간 검사
			if (nowAttack && (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.3f &&
							  skeletonAnimation.state.GetCurrent(0).AnimationTime < 0.4f))
			{
				if (!atkRange.enabled)
					// 공격 콜라이더 활성화
					atkRange.enabled = true;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.4f)
			{
				if (atkRange.enabled)
					// 유효시간이 지나면 공격 상태를 해제
					atkRange.enabled = false;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime ==
							 skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				nowAttack = false;
		}
	}
}
