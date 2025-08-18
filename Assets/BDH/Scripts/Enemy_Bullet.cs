using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
	public GameObject render;

	// 목표지점과 현재 지점을 알아낼 좌표
	Vector3 dir;
	Vector3 oriVec;

	// 각도
	float z;

	// 곡사포 높이
	float Height = 9;

	// 위치 고정
	bool ShootStart;

	[SerializeField]
	GameObject Bullet;
	[SerializeField]
	GameObject Fire;
	[SerializeField]
	GameObject Boom;

	[SerializeField]
	AnimationReferenceAsset boomClip;

	[SerializeField]
	Collider[] attackCol;

	// 목표지점 설정
	public void shootTarget(Vector3 pos)
	{
		// 곡사포가 도달할 좌표
		dir = pos - transform.position;
		// 곡사포를 쏘는 좌표
		oriVec = transform.position;

		StartCoroutine(shoot());
	}

	IEnumerator shoot()
	{
		attackCol[0].enabled = true;

		ShootStart = true;

		while (ShootStart)
		{
			// 목표 지점 사이에 직선 거리 구하기
			// 빗변의 제곱 = 밑변 제곱 + 높이 제곱
			float distance = (dir.x * dir.x) + (dir.z * dir.z);
			// 제곱근을 구하느 MathF.Sqrt 함수
			float RealDis = MathF.Sqrt(distance);

			// 현재 지점을 구하기
			// 곡사포가 발사된 좌표에서 부터 현재 좌표를 빼는 것으로 목표지점 사이의 거리에서 현재 위치를 구한다
			// 빗변의 제곱 = 밑변 제곱 + 높이 제곱
			float nowdis = ((transform.position.x - oriVec.x) * (transform.position.x - oriVec.x)) +
						   ((transform.position.z - oriVec.z) * (transform.position.z - oriVec.z));
			// 제곱근을 구하느 MathF.Sqrt 함수
			float nowRealDis = MathF.Sqrt(nowdis);

			// 2차함수 공식을 통해 현재 위치에서 목표지점 사이를 지나치도록 높이 좌표를 구한다.
			float y = -((nowRealDis) *
					  (nowRealDis) / ((RealDis / 2) - 0.5f)) +
					  Height;

			// 이동할 높이를 구한다
			dir.y = y;

			// 높이에 따른 각도값을 구한다
			if (dir.x > 0)
			{
				z = -270 + ((dir.y / Height) * 90);
				if (z < -360)
					z = 0;
			}
			else if (dir.x < 0)
			{
				z = 270 - ((dir.y / Height) * 90);
				if (z > 360)
					z = 0;
			}

			// 높이에 따라 각도를 조절
			transform.rotation = Quaternion.Euler(0, 0, z);

			// 곡사포 이동
			transform.position += dir * Time.deltaTime;

			// 곡사포가 지면에 닿는 순간 총알을 제거한다
			if (transform.position.y <= 0.1f)
			{
				attackCol[0].enabled = false;
				attackCol[1].enabled = true;

				Bullet.SetActive(false);
				Fire.SetActive(false);
				StartCoroutine(boomOn());
				ShootStart = false;
			}

			yield return null;
		}
	}

	IEnumerator boomOn()
	{
		Boom.SetActive(true);

		bool boom = true;

		SkeletonAnimation anim = GetComponentInChildren<SkeletonAnimation>();
		anim.state.SetAnimation(0, boomClip, false).TimeScale = 1.0f;

		while (boom)
		{
			if (anim.state.GetCurrent(0).AnimationTime > 0.5f)
				attackCol[1].enabled = false;

			if (anim.state.GetCurrent(0).AnimationTime ==
				anim.state.GetCurrent(0).AnimationEnd)
				boom = false;

			yield return null;
		}

		Remove();
	}

	// 오브젝트 풀 안에 총알을 재장전한다
	void Remove()
	{
		Bullet.SetActive(true);
		Fire.SetActive(true);
		Boom.SetActive(false);
		ShootStart = true;

		ObjectPool_Bullet.instance.reloadBullet(gameObject);
	}
}
