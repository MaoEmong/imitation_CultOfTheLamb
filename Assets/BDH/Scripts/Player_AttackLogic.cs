using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackLogic : MonoBehaviour
{
	// ���콺 ��ǥ
	Vector3 MousePos;

	// ������ ��ǥ ���ϱ�
	public GameObject player;

	public MeshCollider atkRange;
	public SkeletonAnimation skeletonAnimation;

	// ������ x, z
	float x;
	float z;

	// ������
	float dgr;

	// ���� ���� Ȯ�� ����
	bool nowAttack;

	public bool onRight;

	void Update()
	{
		moveAttackPos();
		if (!player.GetComponent<PlayerMove>().isdash)
			AttackNow();
	}

	// ���ݹ��� ���� �޼���
	void moveAttackPos()
	{
		// ȭ����� ���콺 ��ǥ ȹ��
		MousePos = Input.mousePosition;

		// ȭ�� ��ǥ�� ������� ȭ�� ���߾ӿ��� ���� ���콺 ��ġ�� ���� ��ǥ��
		x = MousePos.x - 960;
		z = MousePos.y - 540;

		if (player.GetComponent<PlayerMove>().isFlip)
		{
			// ���Լ��� �̿��� ���콺 ����Ʈ ��ġ Ư��
			dgr = Mathf.Atan2(-z, -x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(90, -dgr, 180);
		}
		else
		{
			// ���Լ��� �̿��� ���콺 ����Ʈ ��ġ Ư��
			dgr = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(90, -dgr, 0);
		}

		// ���콺 ��ġ�� ������� ������ǥ�� ����� ������Ʈ�� ��ġ�� ����
		transform.localPosition = new Vector3(
						MathF.Cos(dgr * Mathf.Deg2Rad) * 2f,
						0,
						MathF.Sin(dgr * Mathf.Deg2Rad) * 2f);
	}
	
	// ���� �޼���
	void AttackNow()
	{
		// ��Ŭ�� ��
		if (Input.GetMouseButtonDown(0) && !nowAttack)
		{
			// ���� ����
			nowAttack = true;

			// ���� ���⿡ ���缭 �÷��̾� �þ� ���� ����
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
			// ���� ��ȿ�ð� �˻�
			if (nowAttack && (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.2f &&
							  skeletonAnimation.state.GetCurrent(0).AnimationTime < 0.3f))
			{
				if (!atkRange.enabled)
					// ���� �ݶ��̴� Ȱ��ȭ
					atkRange.enabled = true;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.3f)
			{
				if (atkRange.enabled)
					// ��ȿ�ð��� ������ ���� ���¸� ����
					atkRange.enabled = false;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime ==
							 skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				nowAttack = false;
		}
		else
		{
			// ���� ��ȿ�ð� �˻�
			if (nowAttack && (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.3f &&
							  skeletonAnimation.state.GetCurrent(0).AnimationTime < 0.4f))
			{
				if (!atkRange.enabled)
					// ���� �ݶ��̴� Ȱ��ȭ
					atkRange.enabled = true;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.4f)
			{
				if (atkRange.enabled)
					// ��ȿ�ð��� ������ ���� ���¸� ����
					atkRange.enabled = false;
			}
			if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime ==
							 skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				nowAttack = false;
		}
	}
}
