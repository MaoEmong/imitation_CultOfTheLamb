using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
	public GameObject render;

	// ��ǥ������ ���� ������ �˾Ƴ� ��ǥ
	Vector3 dir;
	Vector3 oriVec;

	// ����
	float z;

	// ����� ����
	float Height = 9;

	// ��ġ ����
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

	// ��ǥ���� ����
	public void shootTarget(Vector3 pos)
	{
		// ������� ������ ��ǥ
		dir = pos - transform.position;
		// ������� ��� ��ǥ
		oriVec = transform.position;

		StartCoroutine(shoot());
	}

	IEnumerator shoot()
	{
		attackCol[0].enabled = true;

		ShootStart = true;

		while (ShootStart)
		{
			// ��ǥ ���� ���̿� ���� �Ÿ� ���ϱ�
			// ������ ���� = �غ� ���� + ���� ����
			float distance = (dir.x * dir.x) + (dir.z * dir.z);
			// �������� ���ϴ� MathF.Sqrt �Լ�
			float RealDis = MathF.Sqrt(distance);

			// ���� ������ ���ϱ�
			// ������� �߻�� ��ǥ���� ���� ���� ��ǥ�� ���� ������ ��ǥ���� ������ �Ÿ����� ���� ��ġ�� ���Ѵ�
			// ������ ���� = �غ� ���� + ���� ����
			float nowdis = ((transform.position.x - oriVec.x) * (transform.position.x - oriVec.x)) +
						   ((transform.position.z - oriVec.z) * (transform.position.z - oriVec.z));
			// �������� ���ϴ� MathF.Sqrt �Լ�
			float nowRealDis = MathF.Sqrt(nowdis);

			// 2���Լ� ������ ���� ���� ��ġ���� ��ǥ���� ���̸� ����ġ���� ���� ��ǥ�� ���Ѵ�.
			float y = -((nowRealDis) *
					  (nowRealDis) / ((RealDis / 2) - 0.5f)) +
					  Height;

			// �̵��� ���̸� ���Ѵ�
			dir.y = y;

			// ���̿� ���� �������� ���Ѵ�
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

			// ���̿� ���� ������ ����
			transform.rotation = Quaternion.Euler(0, 0, z);

			// ����� �̵�
			transform.position += dir * Time.deltaTime;

			// ������� ���鿡 ��� ���� �Ѿ��� �����Ѵ�
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

	// ������Ʈ Ǯ �ȿ� �Ѿ��� �������Ѵ�
	void Remove()
	{
		Bullet.SetActive(true);
		Fire.SetActive(true);
		Boom.SetActive(false);
		ShootStart = true;

		ObjectPool_Bullet.instance.reloadBullet(gameObject);
	}
}
